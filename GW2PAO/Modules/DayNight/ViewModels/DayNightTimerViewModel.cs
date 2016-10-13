using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.API.Services.Interfaces;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.DayNight.ViewModels
{
    [Export(typeof(DayNightTimerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DayNightTimerViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const int REFRESH_INTERVAL = 1000; // Refresh every second
        private IPlayerService playerService;
        private bool isDaytime;
        private TimeSpan timeUntilNight;
        private TimeSpan timeUntilDay;
        private double dayNightCyclePercentage;
        private bool doesCurrentZoneCycle;

        /// <summary>
        /// Collection of zone IDs that are always in nighttime
        /// </summary>
        private static readonly IReadOnlyList<int> ALWAYS_NIGHT_ZONES = new List<int>()
        {
            33, // Ascalonian Catacombs
            36, // Ascalonian Catacombs
            63, // Sorrow's Embrace
            64, // Sorrow's Embrace
            66, // Citadel of Flame
            69, // Citadel of Flame
            67, // Twilight Arbor
            68, // Twilight Arbor
            82, // Crucible of Eternity (explorable mode)
            112, // The Ruined City of Arah
            350, // Heart of the Mists
            549, // Battle of Kyhlo
            554, // Forest of Niflhel
            875, // Temple of the Silent Storm
            984, // Courtyard
            1011, // Battle of Champion's Dusk
            1163, // Revenge of the Capricorn
        }.AsReadOnly();

        /// <summary>
        /// Collection of zone IDs that are always in daytime
        /// </summary>
        private static readonly IReadOnlyList<int> ALWAYS_DAY_ZONES = new List<int>()
        {
            70, // Honor of the Waves
            71, // Honor of the Waves
            75, // Caudecus's Manor
            76, // Caudecus's Manor
            81, // Crucible of Eternity (story mode)
            795, // Legacy of the Foefire
            894, // Spirit Watch
            900, // Skyhammer
            968, // Edge of the Mists
        }.AsReadOnly();

        /// <summary>
        /// Timer for the periodic refresh thread
        /// </summary>
        private Timer refreshTimer;

        /// <summary>
        /// True if the world is currently in daytime, else false
        /// </summary>
        public bool IsDaytime
        {
            get { return this.isDaytime; }
            set { SetProperty(ref this.isDaytime, value); }
        }

        /// <summary>
        /// Time until nighttime begins
        /// </summary>
        public TimeSpan TimeUntilNight
        {
            get { return this.timeUntilNight; }
            set { SetProperty(ref this.timeUntilNight, value); }
        }

        /// <summary>
        /// Time until daytime begins
        /// </summary>
        public TimeSpan TimeUntilDay
        {
            get { return this.timeUntilDay; }
            set { SetProperty(ref this.timeUntilDay, value); }
        }

        /// <summary>
        /// Percentage corresponding to the current day/night stage
        /// </summary>
        public double DayNightCyclePercentage
        {
            get { return this.dayNightCyclePercentage; }
            set { SetProperty(ref this.dayNightCyclePercentage, value); }
        }

        /// <summary>
        /// True if the actual day/night timer value should be shown, else false
        /// </summary>
        public bool ShowDayNightTime
        {
            get { return Properties.Settings.Default.ShowDayNightTime; }
            set
            {
                if (Properties.Settings.Default.ShowDayNightTime != value)
                {
                    Properties.Settings.Default.ShowDayNightTime = value;
                    Properties.Settings.Default.Save();
                    this.OnPropertyChanged(() => this.ShowDayNightTime);
                }
            }
        }

        /// <summary>
        /// True if the current zone the player is in cycles day/night, else false
        /// </summary>
        public bool DoesCurrentZoneCycle
        {
            get { return this.doesCurrentZoneCycle; }
            set { SetProperty(ref this.doesCurrentZoneCycle, value); }
        }

        /// <summary>
        /// Command to shutdown the day night timer
        /// </summary>
        public ICommand ShutdownCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="playerService">The player service</param>
        [ImportingConstructor]
        public DayNightTimerViewModel(IPlayerService playerService)
        {
            this.playerService = playerService;

            this.refreshTimer = new Timer(this.Refresh);
            this.Refresh();
            this.refreshTimer.Change(REFRESH_INTERVAL, REFRESH_INTERVAL);
        }

        /// <summary>
        /// Shuts down the day/night timer
        /// Note: Once shutdown has been called, the timer cannot be started again
        /// </summary>
        private void Shutdown()
        {
            this.refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            this.refreshTimer.Dispose();
        }

        /// <summary>
        /// The main refresh method
        /// </summary>
        private void Refresh(object state = null)
        {
            const int SCHEDULE_OFFSET_MINUTES = 25;
            const int CYCLE_LENGTH_MINUTES = 120;
            const int NIGHT_START_MINUTES = 90;

            // First, check to see if the current zone actually cycles day/night
            if (this.playerService.HasValidMapId)
            {
                if (ALWAYS_NIGHT_ZONES.Contains(this.playerService.MapId))
                {
                    this.IsDaytime = false;
                    this.DoesCurrentZoneCycle = false;
                    this.DayNightCyclePercentage = 0;
                }
                else if (ALWAYS_DAY_ZONES.Contains(this.playerService.MapId))
                {
                    this.IsDaytime = true;
                    this.DoesCurrentZoneCycle = false;
                    this.DayNightCyclePercentage = 0;
                }
                else
                {
                    this.DoesCurrentZoneCycle = true;
                }
            }
            else
            {
                this.DoesCurrentZoneCycle = true;
            }


            if (this.DoesCurrentZoneCycle)
            {
                // Day starts on the quarter of every hour, every other hour, beginning with 00:25
                // Therefore, we can just offset our time and then mod it so we are comparing against 00:00-02:00,
                TimeSpan offsetAdjustedTime = DateTimeOffset.UtcNow.AddMinutes(SCHEDULE_OFFSET_MINUTES * -1).TimeOfDay;
                offsetAdjustedTime = TimeSpan.FromMinutes(offsetAdjustedTime.TotalMinutes % CYCLE_LENGTH_MINUTES);

                // Figure out if we are in day or night by comparing against 01:30
                // > 01:30 means we are in night, otherwise we are in day
                if (offsetAdjustedTime.TotalMinutes > NIGHT_START_MINUTES)
                {
                    // Night-time
                    this.IsDaytime = false;
                    this.TimeUntilDay = TimeSpan.FromMinutes(CYCLE_LENGTH_MINUTES - offsetAdjustedTime.TotalMinutes);
                    this.TimeUntilNight = TimeSpan.FromMinutes(CYCLE_LENGTH_MINUTES + NIGHT_START_MINUTES - offsetAdjustedTime.TotalMinutes);
                    this.DayNightCyclePercentage = ((offsetAdjustedTime.TotalMinutes - NIGHT_START_MINUTES) / (CYCLE_LENGTH_MINUTES - NIGHT_START_MINUTES)) * 100.0;
                }
                else
                {
                    // Day-time
                    this.IsDaytime = true;
                    this.TimeUntilDay = TimeSpan.FromMinutes(CYCLE_LENGTH_MINUTES - offsetAdjustedTime.TotalMinutes);
                    this.TimeUntilNight = TimeSpan.FromMinutes(NIGHT_START_MINUTES - offsetAdjustedTime.TotalMinutes);
                    this.DayNightCyclePercentage = (offsetAdjustedTime.TotalMinutes / NIGHT_START_MINUTES) * 100.0;
                }
            }
        }
    }
}
