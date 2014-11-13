using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.Dungeons.Interfaces;
using GW2PAO.Modules.Dungeons.ViewModels;
using GW2PAO.Modules.WebBrowser.Interfaces;
using GW2PAO.Utility;
using NLog;

namespace GW2PAO.Modules.Dungeons
{
    [Export(typeof(IDungeonsController))]
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
        private IWebBrowserController browserController;

        /// <summary>
        /// Keeps track of how many times Start() has been called in order
        /// to support reuse of a single object
        /// </summary>
        private int startCallCount;

        /// <summary>
        /// True if the controller's timers are no longer running, else false
        /// </summary>
        private bool isStopped;

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
        private DungeonsUserData userData;

        /// <summary>
        /// The interval by which to refresh the dungeon reset state (in ms)
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// The dungeon user settings
        /// </summary>
        public DungeonsUserData UserData { get { return this.userData; } }

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
        [ImportingConstructor]
        public DungeonsController(IDungeonsService dungeonsService, IZoneService zoneService, IWebBrowserController browserController, DungeonsUserData userData)
        {
            logger.Debug("Initializing Dungeons Controller");
            this.dungeonsService = dungeonsService;
            this.zoneService = zoneService;
            this.browserController = browserController;
            this.userData = userData;
            this.isStopped = false;

            // Make sure the dungeons service has loaded the dungeons table
            this.dungeonsService.LoadTable();

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
                    this.isStopped = false;
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
                    this.isStopped = true;
                    this.dungeonsRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Forces a shutdown of the controller, including all running timers/threads
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutdown called");
            logger.Debug("Stopping refresh timers");
            lock (this.resetTimerLock)
            {
                this.isStopped = true;
                this.dungeonsRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
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
                if (this.isStopped)
                    return; // Immediately return if we are supposed to be stopped

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
