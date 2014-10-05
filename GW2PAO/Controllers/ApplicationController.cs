using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Services;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Models;
using GW2PAO.TS3.Services;
using GW2PAO.TS3.Services.Interfaces;
using GW2PAO.Utility;
using GW2PAO.ViewModels.EventNotification;
using GW2PAO.ViewModels.Interfaces;
using GW2PAO.ViewModels.Teamspeak;
using GW2PAO.ViewModels.TrayIcon;
using GW2PAO.ViewModels.WvWTracker;
using GW2PAO.ViewModels.ZoneCompletion;
using GW2PAO.Views;
using GW2PAO.Views.DungeonTracker;
using GW2PAO.Views.EventNotification;
using GW2PAO.Views.EventTracker;
using GW2PAO.Views.PriceNotification;
using GW2PAO.Views.Teamspeak;
using GW2PAO.Views.TradingPost;
using GW2PAO.Views.WebBrowser;
using GW2PAO.Views.WvWNotification;
using GW2PAO.Views.WvWTracker;
using GW2PAO.Views.ZoneCompletion;
using NLog;

namespace GW2PAO.Controllers
{
    /// <summary>
    /// The primary application controller
    /// This class is responsible for glueing the program together, created the various services, controllers, views, and view models
    /// </summary>
    public class ApplicationController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Service responsible for Event information
        /// </summary>
        public EventsService EventsService { get; private set; }

        /// <summary>
        /// Service responsible for Player information
        /// </summary>
        public PlayerService PlayerService { get; private set; }

        /// <summary>
        /// Service responsible for System information
        /// </summary>
        public SystemService SystemService { get; private set; }

        /// <summary>
        /// Service responsible for Zone information
        /// </summary>
        public ZoneService ZoneService { get; private set; }

        /// <summary>
        /// Service responsible for Dungeon information
        /// </summary>
        public DungeonsService DungeonsService { get; private set; }

        /// <summary>
        /// The Teampseak Service that provides events and methods for
        /// interacting with teamspeak
        /// </summary>
        public TeamspeakService TeamspeakService { get; private set; }

        /// <summary>
        /// Service responsible for WvW information
        /// </summary>
        public WvWService WvWService { get; private set; }

        /// <summary>
        /// Service responsible for retrieving Guild information
        /// </summary>
        public GuildService GuildService { get; private set; }

        /// <summary>
        /// Service responsible for commerce/trade/item information
        /// </summary>
        public CommerceService CommerceService { get; private set; }

        /// <summary>
        /// Events controller
        /// </summary>
        public IEventsController EventsController { get; private set; }

        /// <summary>
        /// Zone completion controller
        /// </summary>
        public IZoneCompletionController ZoneCompletionController { get; private set; }

        /// <summary>
        /// Dungeons controller
        /// </summary>
        public IDungeonsController DungeonsController { get; private set; }

        /// <summary>
        /// WvW controller
        /// </summary>
        public IWvWController WvWController { get; private set; }

        /// <summary>
        /// Object that displays the current zone's name (used for the Zone Completion Assistant)
        /// </summary>
        public IHasZoneName ZoneName { get; private set; }

        /// <summary>
        /// Object that displays the current WvW Map (used for the WvW Tracker)
        /// </summary>
        public IHasWvWMap WvWMap { get; private set; }

        /// <summary>
        /// Controller for the browser. Responsible for showing the browser, or going to a specific URL
        /// </summary>
        public IBrowserController BrowserController { get; private set; }

        /// <summary>
        /// Commerce Controller
        /// </summary>
        public ICommerceController CommerceController { get; private set; }

        /// <summary>
        /// Event settings
        /// </summary>
        public EventSettings EventSettings { get; private set; }

        /// <summary>
        /// Zone completion settings
        /// </summary>
        public ZoneCompletionSettings ZoneCompletionSettings { get; private set; }

        /// <summary>
        /// Dungeon settings
        /// </summary>
        public DungeonSettings DungeonSettings { get; private set; }

        /// <summary>
        /// WvW Settings
        /// </summary>
        public WvWSettings WvWSettings { get; private set; }

        /// <summary>
        /// User settings for teamspeak
        /// </summary>
        public TeamspeakSettings TeamspeakSettings { get; private set; }

        /// <summary>
        /// User settings for commerce-focused overlays (like price notifications)
        /// </summary>
        public CommerceSettings CommerceSettings { get; private set; }

        /// <summary>
        /// Main functionality menu items, including those for the Event Tracker 
        /// and the Zone Completion Assistant
        /// </summary>
        private List<MenuItemViewModel> menuItems = new List<MenuItemViewModel>();

        /// <summary>
        /// The Event Tracker view
        /// </summary>
        private EventTrackerView eventTrackerView;

        /// <summary>
        /// The Zone Completion Assistant view
        /// </summary>
        private ZoneCompletionView zoneCompletionView;

        /// <summary>
        /// The event notifications window containing all event notifications
        /// </summary>
        private EventNotificationWindow eventNotificationsView;

        /// <summary>
        /// The Dungeon Tracker view
        /// </summary>
        private DungeonTrackerView dungeonTrackerView;

        /// <summary>
        /// The WvW Tracker view
        /// </summary>
        private WvWTrackerView wvwTrackerView;

        /// <summary>
        /// The WvW notifications window containing all WvW notifications
        /// </summary>
        private WvWNotificationWindow wvwNotificationsView;

        /// <summary>
        /// The TP Calculator utility window
        /// </summary>
        private TPCalculatorView tpCalculatorView;

        /// <summary>
        /// The Teamspeak View
        /// </summary>
        private TeamspeakView teamspeakView;

        /// <summary>
        /// Window containing price notifications
        /// </summary>
        private PriceNotificationWindow priceNotificationsView;

        /// <summary>
        /// Window used for configuring price notifications
        /// </summary>
        private PriceNotificationConfigView priceNotificationsConfigView;

        /// <summary>
        /// View used when rebuilding the item names database
        /// </summary>
        private RebuildNamesDatabaseView rebuildItemNamesView;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationController()
        {
            // Create services
            logger.Debug("Creating services");
            this.EventsService = new EventsService();
            this.PlayerService = new PlayerService();
            this.SystemService = new SystemService();
            this.ZoneService = new ZoneService();
            this.DungeonsService = new DungeonsService();
            this.WvWService = new WvWService();
            this.GuildService = new GuildService();
            this.TeamspeakService = new TeamspeakService();
            this.CommerceService = new CommerceService();

            // Create ZoneName view model for the Zone Completion Assistant
            this.ZoneName = new ZoneNameViewModel();

            // Create WvWMap view model for the WvW Tracker
            this.WvWMap = new WvWMapViewModel();

            // Load user settings
            logger.Debug("Loading event user settings");
            this.EventSettings = EventSettings.LoadSettings(EventSettings.Filename);
            if (this.EventSettings == null)
                this.EventSettings = new EventSettings();

            logger.Debug("Loading zone completion assistant user settings");
            this.ZoneCompletionSettings = ZoneCompletionSettings.LoadSettings(ZoneCompletionSettings.Filename);
            if (this.ZoneCompletionSettings == null)
                this.ZoneCompletionSettings = new ZoneCompletionSettings();

            logger.Debug("Loading dungeon user settings");
            this.DungeonSettings = DungeonSettings.LoadSettings(DungeonSettings.Filename);
            if (this.DungeonSettings == null)
                this.DungeonSettings = new DungeonSettings();

            logger.Debug("Loading wvw user settings");
            this.WvWSettings = WvWSettings.LoadSettings(WvWSettings.Filename);
            if (this.WvWSettings == null)
                this.WvWSettings = new WvWSettings();

            logger.Debug("Loading teamspeak settings");
            this.TeamspeakSettings = TeamspeakSettings.LoadSettings(TeamspeakSettings.Filename);
            if (this.TeamspeakSettings == null)
                this.TeamspeakSettings = new TeamspeakSettings();

            logger.Debug("Loading commerce settings");
            this.CommerceSettings = CommerceSettings.LoadSettings(CommerceSettings.Filename);
            if (this.CommerceSettings == null)
                this.CommerceSettings = new CommerceSettings();

            // Enable autosave on the user settings
            logger.Debug("Enabling autosave of user settings");
            this.EventSettings.EnableAutoSave();
            this.ZoneCompletionSettings.EnableAutoSave();
            this.DungeonSettings.EnableAutoSave();
            this.WvWSettings.EnableAutoSave();
            this.TeamspeakSettings.EnableAutoSave();
            this.CommerceSettings.EnableAutoSave();

            // Create the controllers
            logger.Debug("Creating browser controller");
            this.BrowserController = new BrowserController();

            logger.Debug("Creating events controller");
            this.EventsController = new EventsController(this.EventsService, this.EventSettings);
            this.EventsController.Start(); // Get it started for event notifications

            logger.Debug("Creating zone completion assistant controller");
            this.ZoneCompletionController = new ZoneCompletionController(this.ZoneService, this.PlayerService, this.SystemService, this.ZoneName, this.ZoneCompletionSettings);

            logger.Debug("Creating dungeons controller");
            this.DungeonsController = new DungeonsController(this.DungeonsService, this.BrowserController, this.DungeonSettings);

            logger.Debug("Creating wvw controller");
            this.WvWController = new WvWController(this.WvWService, this.PlayerService, this.GuildService, this.WvWMap, this.WvWSettings);
            this.WvWController.Start(); // Get it started for wvw notifications

            logger.Debug("Creating commerce controller");
            this.CommerceController = new CommerceController(this.CommerceService, this.CommerceSettings);
            this.CommerceController.Start(); // Get it started for price-watch notifications

            // Create the event notifications view
            logger.Debug("Initializing event notifications");
            this.eventNotificationsView = new EventNotificationWindow(this.EventsController);
            this.eventNotificationsView.Show(); // Transparent window, just go ahead and show it

            // Create the wvw notifications view
            logger.Debug("Initializing WvW notifications");
            this.wvwNotificationsView = new WvWNotificationWindow(this.WvWController);
            this.wvwNotificationsView.Show(); // Transparent window, just go ahead and show it

            logger.Debug("Initializing price notifications");
            this.priceNotificationsView = new PriceNotificationWindow(this.CommerceController);
            this.priceNotificationsView.Show(); // Transparent window, just go ahead and show it

            // Initialize the menu items
            logger.Debug("Initializing application menu items");
            this.BuildMenuItems();

            logger.Info("Application controller initialized");
        }

        /// <summary>
        /// Forces the application controller to open windows based on the user settings
        /// </summary>
        public void ReopenWindowsFromSettings()
        {
            logger.Info("Reopening windows based on settings.");
            Task.Factory.StartNew(() =>
                {
                    Threading.BeginInvokeOnUI(() =>
                        {
                            if (Properties.Settings.Default.IsEventTrackerOpen && this.CanDisplayEventTracker())
                                this.DisplayEventTracker();

                            if (Properties.Settings.Default.IsZoneAssistantOpen && this.CanDisplayZoneAssistant())
                                this.DisplayZoneAssistant();

                            if (Properties.Settings.Default.IsDungeonTrackerOpen && this.CanDisplayDungeonTracker())
                                this.DisplayDungeonTracker();

                            if (Properties.Settings.Default.IsWvWTrackerOpen && this.CanDisplayWvWTracker())
                                this.DisplayWvWTracker();

                            if (Properties.Settings.Default.IsTPCalculatorOpen && this.CanDisplayTPCalculator())
                                this.DisplayTPCalculator();

                            if (Properties.Settings.Default.IsTeamspeakOpen && this.CanDisplayTeamspeakOverlay())
                                this.DisplayTeamspeakOverlay();
                        });
                });
        }

        /// <summary>
        /// Returns the main application menu items
        /// </summary>
        /// <returns>The main application menu items</returns>
        public IEnumerable<MenuItemViewModel> GetMenuItems()
        {
            return this.menuItems;
        }

        /// <summary>
        /// Shuts down all controllers, views, and viewmodels
        /// </summary>
        public void Shutdown()
        {
            logger.Info("Shutting down application controller");

            logger.Debug("Closing views");
            if (this.eventTrackerView != null)
            {
                Properties.Settings.Default.IsEventTrackerOpen = this.eventTrackerView.IsVisible;
                Threading.InvokeOnUI(() => this.eventTrackerView.Close());
            }

            if (this.zoneCompletionView != null)
            {
                Properties.Settings.Default.IsZoneAssistantOpen = this.zoneCompletionView.IsVisible;
                Threading.InvokeOnUI(() => this.zoneCompletionView.Close());
            }

            if (this.eventNotificationsView != null)
            {
                Threading.InvokeOnUI(() => this.eventNotificationsView.Close());
            }

            if (this.dungeonTrackerView != null)
            {
                Properties.Settings.Default.IsDungeonTrackerOpen = this.dungeonTrackerView.IsVisible;
                Threading.InvokeOnUI(() => this.dungeonTrackerView.Close());
            }

            if (this.wvwTrackerView != null)
            {
                Properties.Settings.Default.IsWvWTrackerOpen = this.wvwTrackerView.IsVisible;
                Threading.InvokeOnUI(() => this.wvwTrackerView.Close());
            }

            if (this.wvwNotificationsView != null)
            {
                Threading.InvokeOnUI(() => this.wvwNotificationsView.Close());
            }

            if (this.tpCalculatorView != null)
            {
                Properties.Settings.Default.IsTPCalculatorOpen = this.tpCalculatorView.IsVisible;
                Threading.InvokeOnUI(() => this.tpCalculatorView.Close());
            }

            if (this.teamspeakView != null)
            {
                Properties.Settings.Default.IsTeamspeakOpen = this.teamspeakView.IsVisible;
                Threading.InvokeOnUI(() => this.teamspeakView.Close());
            }

            Threading.InvokeOnUI(() => this.BrowserController.CloseBrowser());

            Properties.Settings.Default.Save();

            logger.Debug("Stopping controllers");
            this.EventsController.Stop();
            this.ZoneCompletionController.Stop();
            this.DungeonsController.Stop();
            this.WvWController.Stop();
        }

        /// <summary>
        /// Initializes the menuItems list
        /// </summary>
        private void BuildMenuItems()
        {
            // Event Menu Items
            var eventMenu = new MenuItemViewModel("Events", null);

            var eventTimeTableMenus = new MenuItemViewModel("Event Time Table", null);
            var standardEvents = new MenuItemViewModel("Standard", null, true,
                () => { return !this.EventSettings.UseAdjustedTimeTable; },
                (selected) => this.EventSettings.UseAdjustedTimeTable = !selected,
                this.EventSettings, "UseAdjustedTimeTable");
            eventTimeTableMenus.SubMenuItems.Add(standardEvents);
            var adjustedEvents = new MenuItemViewModel("Adjusted", null, true,
                () => { return this.EventSettings.UseAdjustedTimeTable; },
                (selected) => this.EventSettings.UseAdjustedTimeTable = selected,
                this.EventSettings, "UseAdjustedTimeTable");
            eventTimeTableMenus.SubMenuItems.Add(adjustedEvents);
            eventMenu.SubMenuItems.Add(eventTimeTableMenus);
            eventMenu.SubMenuItems.Add(new MenuItemViewModel("Open Events Tracker", this.DisplayEventTracker, this.CanDisplayEventTracker));
            eventMenu.SubMenuItems.Add(new MenuItemViewModel("Event Notifications", null, true, () => { return this.EventSettings.AreEventNotificationsEnabled; }, (enabled) => this.EventSettings.AreEventNotificationsEnabled = enabled));
            this.menuItems.Add(eventMenu);

            // Zone Completion Assistant
            this.menuItems.Add(new MenuItemViewModel("Zone Completion Assistant", this.DisplayZoneAssistant, this.CanDisplayZoneAssistant));

            // Dungeons Tracker
            this.menuItems.Add(new MenuItemViewModel("Dungeons Tracker", this.DisplayDungeonTracker, this.CanDisplayDungeonTracker));

            // WvW menus (these are a bit more complicated)
            var wvwMenu = new MenuItemViewModel("World vs World", null);

            // World Selection Menu
            var wvwWorldSelectionMenu = new MenuItemViewModel("World Selection", null);
            var naWorlds = new MenuItemViewModel("NA", null);
            foreach (var world in this.WvWService.Worlds.Worlds.Where(wld => wld.ID < 2000))
            {
                var worldMenuItem = new MenuItemViewModel(world.Name, null, true,
                    () => { return this.WvWSettings.WorldSelection.ID == world.ID; },
                    (selected) => { if (selected) this.WvWSettings.WorldSelection = world; },
                    this.WvWSettings, "WorldSelection");
                naWorlds.SubMenuItems.Add(worldMenuItem);
            }
            wvwWorldSelectionMenu.SubMenuItems.Add(naWorlds);
            var euWorlds = new MenuItemViewModel("EU", null);
            foreach (var world in this.WvWService.Worlds.Worlds.Where(wld => wld.ID > 2000))
            {
                var worldMenuItem = new MenuItemViewModel(world.Name, null, true,
                    () => { return this.WvWSettings.WorldSelection.ID == world.ID; },
                    (selected) => { if (selected) this.WvWSettings.WorldSelection = world; },
                    this.WvWSettings, "WorldSelection");
                euWorlds.SubMenuItems.Add(worldMenuItem);
            }
            wvwWorldSelectionMenu.SubMenuItems.Add(euWorlds);

            wvwMenu.SubMenuItems.Add(wvwWorldSelectionMenu);

            // WvW Tracker view
            wvwMenu.SubMenuItems.Add(new MenuItemViewModel("Open WvW Tracker", this.DisplayWvWTracker, this.CanDisplayWvWTracker));

            // WvW Notifications Menu
            var wvwNotificationsMenu = new MenuItemViewModel("WvW Notifications", null);

            var wvwNotificationsMaps = new MenuItemViewModel("Maps", null);
            // WvW Notifications Maps Menu
            wvwNotificationsMaps.SubMenuItems.Add(new MenuItemViewModel("Enable All", () =>
            {
                this.WvWSettings.AreEternalBattlegroundsNotificationsEnabled = true;
                this.WvWSettings.AreBlueBorderlandsNotificationsEnabled = true;
                this.WvWSettings.AreGreenBorderlandsNotificationsEnabled = true;
                this.WvWSettings.AreRedBorderlandsNotificationsEnabled = true;
            }));
            wvwNotificationsMaps.SubMenuItems.Add(new MenuItemViewModel("Disable All", () =>
            {
                this.WvWSettings.AreEternalBattlegroundsNotificationsEnabled = false;
                this.WvWSettings.AreBlueBorderlandsNotificationsEnabled = false;
                this.WvWSettings.AreGreenBorderlandsNotificationsEnabled = false;
                this.WvWSettings.AreRedBorderlandsNotificationsEnabled = false;
            }));
            wvwNotificationsMaps.SubMenuItems.Add(null); // Null for a seperator
            wvwNotificationsMaps.SubMenuItems.Add(new MenuItemViewModel("Eternal Battlegrounds", null, true, () => { return this.WvWSettings.AreEternalBattlegroundsNotificationsEnabled; }, (enabled) => this.WvWSettings.AreEternalBattlegroundsNotificationsEnabled = enabled, this.WvWSettings, "AreEternalBattlegroundsNotificationsEnabled"));
            wvwNotificationsMaps.SubMenuItems.Add(new MenuItemViewModel("Blue Borderlands", null, true, () => { return this.WvWSettings.AreBlueBorderlandsNotificationsEnabled; }, (enabled) => this.WvWSettings.AreBlueBorderlandsNotificationsEnabled = enabled, this.WvWSettings, "AreBlueBorderlandsNotificationsEnabled"));
            wvwNotificationsMaps.SubMenuItems.Add(new MenuItemViewModel("Green Borderlands", null, true, () => { return this.WvWSettings.AreGreenBorderlandsNotificationsEnabled; }, (enabled) => this.WvWSettings.AreGreenBorderlandsNotificationsEnabled = enabled, this.WvWSettings, "AreGreenBorderlandsNotificationsEnabled"));
            wvwNotificationsMaps.SubMenuItems.Add(new MenuItemViewModel("Red Borderlands", null, true, () => { return this.WvWSettings.AreRedBorderlandsNotificationsEnabled; }, (enabled) => this.WvWSettings.AreRedBorderlandsNotificationsEnabled = enabled, this.WvWSettings, "AreRedBorderlandsNotificationsEnabled"));

            wvwNotificationsMenu.SubMenuItems.Add(new MenuItemViewModel("When Home World Takes Objective", null, true, () => { return this.WvWSettings.NotifyWhenHomeTakesObjective; }, (enabled) => this.WvWSettings.NotifyWhenHomeTakesObjective = enabled, this.WvWSettings, "NotifyWhenHomeTakesObjective"));
            wvwNotificationsMenu.SubMenuItems.Add(new MenuItemViewModel("When Home World Loses Objective", null, true, () => { return this.WvWSettings.NotifyWhenHomeLosesObjective; }, (enabled) => this.WvWSettings.NotifyWhenHomeLosesObjective = enabled, this.WvWSettings, "NotifyWhenHomeLosesObjective"));
            wvwNotificationsMenu.SubMenuItems.Add(new MenuItemViewModel("When Other World Takes Other World's Objective", null, true, () => { return this.WvWSettings.NotifyWhenOtherTakesOtherObjective; }, (enabled) => this.WvWSettings.NotifyWhenOtherTakesOtherObjective = enabled, this.WvWSettings, "NotifyWhenOtherTakesOtherObjective"));
            wvwNotificationsMenu.SubMenuItems.Add(wvwNotificationsMaps);

            wvwMenu.SubMenuItems.Add(wvwNotificationsMenu);

            this.menuItems.Add(wvwMenu);

            // Commerce/Trade menus
            var commerceMenu = new MenuItemViewModel("Commerce", null);

            // TP Calculator
            commerceMenu.SubMenuItems.Add(new MenuItemViewModel("TP Calculator", this.DisplayTPCalculator, this.CanDisplayTPCalculator));

            // Price Notifications
            var priceNotificationsMenu = new MenuItemViewModel("Price Notifications", null);
            priceNotificationsMenu.SubMenuItems.Add(new MenuItemViewModel("Configure", this.DisplayPriceNotificationsConfig, this.CanDisplayPriceNotificationsConfig));
            priceNotificationsMenu.SubMenuItems.Add(new MenuItemViewModel("Rebuild Item Names Database", this.DisplayRebuildItemNamesView, this.CanDisplayRebuildItemNamesView));
            priceNotificationsMenu.SubMenuItems.Add(null); // Null for a seperator
            priceNotificationsMenu.SubMenuItems.Add(new MenuItemViewModel("Buy Order Price Notifications", null, true, () => { return this.CommerceSettings.AreBuyOrderPriceNotificationsEnabled; }, (enabled) => this.CommerceSettings.AreBuyOrderPriceNotificationsEnabled = enabled, this.CommerceSettings, "AreBuyOrderPriceNotificationsEnabled"));
            priceNotificationsMenu.SubMenuItems.Add(new MenuItemViewModel("Sell Listing Price Notifications", null, true, () => { return this.CommerceSettings.AreSellListingPriceNotificationsEnabled; }, (enabled) => this.CommerceSettings.AreSellListingPriceNotificationsEnabled = enabled, this.CommerceSettings, "AreSellListingPriceNotificationsEnabled"));
            commerceMenu.SubMenuItems.Add(priceNotificationsMenu);

            this.menuItems.Add(commerceMenu);

            // Teamspeak Overlay
            this.menuItems.Add(new MenuItemViewModel("Teamspeak Overlay", this.DisplayTeamspeakOverlay, this.CanDisplayTeamspeakOverlay));

#if !NO_BROWSER
            // Web Browser
            this.menuItems.Add(new MenuItemViewModel("Web Browser", this.DisplayWebBrowser, this.CanDisplayWebBrowser));
#endif
        }

        /// <summary>
        /// Displays the Event Tracker window, or, if already displayed, sets
        /// focus to the window
        /// </summary>
        private void DisplayEventTracker()
        {
            if (this.eventTrackerView == null || !this.eventTrackerView.IsVisible)
            {
                this.EventsController.Start();
                this.eventTrackerView = new EventTrackerView(this.EventsController);
                this.eventTrackerView.Show();
            }
            else
            {
                this.eventTrackerView.Focus();
            }
        }

        /// <summary>
        /// Determines if the event tracker can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        private bool CanDisplayEventTracker()
        {
            return true;
        }

        /// <summary>
        /// Displays the Zone Completion Assistant window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        private void DisplayZoneAssistant()
        {
            if (this.zoneCompletionView == null || !this.zoneCompletionView.IsVisible)
            {
                this.ZoneCompletionController.Start();
                this.zoneCompletionView = new ZoneCompletionView(this.ZoneCompletionController, this.ZoneName);
                this.zoneCompletionView.Show();
            }
            else
            {
                this.zoneCompletionView.Focus();
            }
        }

        /// <summary>
        /// Determines if the zone assistant can be displayed
        /// </summary>
        /// <returns></returns>
        private bool CanDisplayZoneAssistant()
        {
            return true;
        }

        /// <summary>
        /// Displays the Dungeon Tracker window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        private void DisplayDungeonTracker()
        {
            if (this.dungeonTrackerView == null || !this.dungeonTrackerView.IsVisible)
            {
                this.DungeonsController.Start();
                this.dungeonTrackerView = new DungeonTrackerView(this.DungeonsController);
                this.dungeonTrackerView.Show();
            }
            else
            {
                this.dungeonTrackerView.Focus();
            }
        }

        /// <summary>
        /// Determines if the dungeon tracker can be displayed
        /// </summary>
        /// <returns></returns>
        private bool CanDisplayDungeonTracker()
        {
            return true;
        }

        /// <summary>
        /// Displays the WvW Tracker window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        private void DisplayWvWTracker()
        {
            if (this.wvwTrackerView == null || !this.wvwTrackerView.IsVisible)
            {
                this.wvwTrackerView = new WvWTrackerView(this.WvWController, this.WvWMap);
                this.wvwTrackerView.Show();
            }
            else
            {
                this.wvwTrackerView.Focus();
            }
        }

        /// <summary>
        /// Determines if the wvw tracker can be displayed
        /// </summary>
        /// <returns></returns>
        private bool CanDisplayWvWTracker()
        {
            return true;
        }

        /// <summary>
        /// Displays the TP Calcualtor window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        private void DisplayTPCalculator()
        {
            if (this.tpCalculatorView == null || !this.tpCalculatorView.IsVisible)
            {
                this.tpCalculatorView = new TPCalculatorView();
                this.tpCalculatorView.Show();
            }
            else
            {
                this.tpCalculatorView.Focus();
            }
        }

        /// <summary>
        /// Determines if the TP Calculator can be displayed
        /// </summary>
        /// <returns></returns>
        private bool CanDisplayTPCalculator()
        {
            return true;
        }

        /// <summary>
        /// Displays the Web Browser window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        private void DisplayWebBrowser()
        {
            this.BrowserController.OpenBrowser();
        }

        /// <summary>
        /// Determines if the web browser can be displayed
        /// </summary>
        /// <returns></returns>
        private bool CanDisplayWebBrowser()
        {
            return true;
        }

        /// <summary>
        /// Displays the Teamspeak overlay window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        private void DisplayTeamspeakOverlay()
        {
            if (this.teamspeakView == null || !this.teamspeakView.IsVisible)
            {
                this.teamspeakView = new TeamspeakView(new TeamspeakViewModel(this.TeamspeakService, this.TeamspeakSettings));
                this.teamspeakView.Show();
            }
            else
            {
                this.teamspeakView.Focus();
            }
        }

        /// <summary>
        /// Determines if the Teamspeak overlay window can be displayed
        /// </summary>
        /// <returns></returns>
        private bool CanDisplayTeamspeakOverlay()
        {
            return true;
        }

        /// <summary>
        /// Displays the Price Notifications Configuration window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        private void DisplayPriceNotificationsConfig()
        {
            if (this.priceNotificationsConfigView == null || !this.priceNotificationsConfigView.IsVisible)
            {
                this.priceNotificationsConfigView = new PriceNotificationConfigView(this.CommerceService, this.CommerceController);
                this.priceNotificationsConfigView.Show();
            }
            else
            {
                this.priceNotificationsConfigView.Focus();
            }
        }

        /// <summary>
        /// Determines if the Price Notifications Configuration window can be displayed
        /// </summary>
        private bool CanDisplayPriceNotificationsConfig()
        {
            return true;
        }

        /// <summary>
        /// Displays the Price Rebuild Item Names Database window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        private void DisplayRebuildItemNamesView()
        {
            if (this.rebuildItemNamesView == null || !this.rebuildItemNamesView.IsVisible)
            {
                this.rebuildItemNamesView = new RebuildNamesDatabaseView(this.CommerceService);
                this.rebuildItemNamesView.Show();
            }
            else
            {
                this.priceNotificationsConfigView.Focus();
            }
        }

        /// <summary>
        /// Determines if the Rebuild Item Names Database window can be displayed
        /// </summary>
        private bool CanDisplayRebuildItemNamesView()
        {
            return true;
        }
    }
}
