using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Data;
using GW2PAO.Data.UserData;
using GW2PAO.Utility;
using GW2PAO.ViewModels.Dungeon;
using NLog;

namespace GW2PAO.Controllers
{
    public class DungeonsController : IDungeonsController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The dungeons service object
        /// </summary>
        private IDungeonsService dungeonsService;

        /// <summary>
        /// The zone service object
        /// </summary>
        private IZoneService zoneService;

        /// <summary>
        /// Browser controller. Currently just passed through to DungeonViewModels
        /// </summary>
        private IBrowserController browserController;

        /// <summary>
        /// Keeps track of how many times Start() has been called in order
        /// to support reuse of a single object
        /// </summary>
        private int startCallCount;

        /// <summary>
        /// The primary reset timer object
        /// </summary>
        private Timer dungeonsRefreshTimer;

        /// <summary>
        /// Locking object for operations performed with the reset timer
        /// </summary>
        private readonly object resetTimerLock = new object();

        /// <summary>
        /// User settings for dungeons
        /// </summary>
        private DungeonUserData userData;

        /// <summary>
        /// The interval by which to refresh the dungeon reset state (in ms)
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// The dungeon user settings
        /// </summary>
        public DungeonUserData UserData { get { return this.userData; } }

        /// <summary>
        /// Backing store of the Dungeons collection
        /// </summary>
        private ObservableCollection<DungeonViewModel> dungeons = new ObservableCollection<DungeonViewModel>();

        /// <summary>
        /// The collection of Dungeons
        /// </summary>
        public ObservableCollection<DungeonViewModel> Dungeons { get { return this.dungeons; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dungeonsService">The dungeons service object</param>
        /// <param name="userData">The dungeons user data object</param>
        public DungeonsController(IDungeonsService dungeonsService, IZoneService zoneService, IBrowserController browserController, DungeonUserData userData)
        {
            logger.Debug("Initializing Dungeons Controller");
            this.dungeonsService = dungeonsService;
            this.zoneService = zoneService;
            this.browserController = browserController;
            this.userData = userData;

            // Initialize the refresh timer
            this.dungeonsRefreshTimer = new Timer(this.RefreshDungeons);
            this.RefreshInterval = 1000;

            // Initialize the start call count to 0
            this.startCallCount = 0;

            // Initialize the WorldEvents collection
            this.InitializeDungeons();

            logger.Info("Dungeons Controller initialized");
        }

        /// <summary>
        /// Starts the automatic refresh
        /// </summary>
        public void Start()
        {
            logger.Debug("Start called");
            Task.Factory.StartNew(() =>
            {
                // Start the timer if this is the first time that Start() has been called
                if (this.startCallCount == 0)
                {
                    logger.Debug("Starting refresh timers");
                    this.RefreshDungeons();
                }

                this.startCallCount++;
                logger.Debug("startCallCount = {0}", this.startCallCount);

            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Stops the automatic refresh
        /// </summary>
        public void Stop()
        {
            this.startCallCount--;
            logger.Debug("Stop called - startCallCount = {0}", this.startCallCount);

            // Stop the refresh timer if all calls to Start() have had a matching call to Stop()
            if (this.startCallCount == 0)
            {
                logger.Debug("Stopping refresh timers");
                lock (this.resetTimerLock)
                {
                    this.dungeonsRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Initializes the collection of dungeons
        /// </summary>
        private void InitializeDungeons()
        {
            logger.Debug("Initializing dungeons");
            this.dungeonsService.LoadTable();

            Threading.InvokeOnUI(() =>
                {
                    foreach (var dungeon in this.dungeonsService.DungeonsTable.Dungeons)
                    {
                        logger.Debug("Initializing localized strings for {0}", dungeon.ID);
                        dungeon.Name = this.dungeonsService.GetLocalizedName(dungeon.ID);
                        dungeon.MapName = this.zoneService.GetZoneName(dungeon.MapID);
                        foreach (var path in dungeon.Paths)
                            path.Nickname = this.dungeonsService.GetLocalizedName(path.ID);

                        logger.Debug("Initializing view model for {0}", dungeon.Name);
                        this.Dungeons.Add(new DungeonViewModel(dungeon, this.browserController, this.userData));
                    }
                });
        }

        /// <summary>
        /// Refreshes all dungeons within the dungeons collection
        /// This is the primary function of the DungeonsController
        /// </summary>
        private void RefreshDungeons(object state = null)
        {
            lock (this.resetTimerLock)
            {
                // Refresh state of path completions
                if (DateTime.UtcNow.Date.CompareTo(this.userData.LastResetDateTime.Date) != 0)
                {
                    logger.Info("Resetting path completions state");
                    this.userData.LastResetDateTime = DateTime.UtcNow;
                    Threading.BeginInvokeOnUI(() =>
                    {
                        foreach (var dungeon in this.Dungeons)
                        {
                            foreach (var path in dungeon.Paths)
                            {
                                path.IsCompleted = false;
                            }
                        }
                    });
                }

                this.dungeonsRefreshTimer.Change(this.RefreshInterval, Timeout.Infinite);
            }
        }
    }
}
