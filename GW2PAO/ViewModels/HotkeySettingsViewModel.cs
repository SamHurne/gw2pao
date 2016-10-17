using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using GW2PAO.Infrastructure;
using GW2PAO.Infrastructure.Hotkeys;
using GW2PAO.Infrastructure.Hotkeys.Interfaces;
using GW2PAO.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json;
using NLog;

namespace GW2PAO.ViewModels
{
    [Export(typeof(HotkeySettingsViewModel))]
    public class HotkeySettingsViewModel : BindableBase, ISettingsViewModel
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Hotkey toggleAllWindowsHotkey;
        private Hotkey toggleInteractiveWindowsHotkey;
        private Hotkey toggleNotificationWindowBordersHotkey;
        private Hotkey toggleAutoFadeBordersHotkey;
        private Hotkey toggleOverlayMenuIconHotkey;
        private Hotkey toggleWorldBossTimerHotkey;
        private Hotkey toggleMetaEventTimerHotkey;
        private Hotkey toggleDungeonsTrackerHotkey;
        private Hotkey toggleDungeonTimerHotkey;
        private Hotkey togglePriceTrackerHotkey;
        private Hotkey toggleWvWTrackerHotkey;
        private Hotkey toggleZoneAssistantHotkey;
        private Hotkey toggleTaskTrackerHotkey;
        private Hotkey toggleTeamspeakTrackerHotkey;
        private Hotkey toggleWebBrowserHotkey;
        private Hotkey toggleMapHotkey;
        private Hotkey toggleDayNightTimerHotkey;

        /// <summary>
        /// Header for the settings
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public string SettingsHeader
        {
            get { return Properties.Resources.Hotkeys; }
        }

        /// <summary>
        /// The global hotkey manager
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public IHotkeyManager GlobalHotkeyManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Hotkey to toggle all windows as visible/hidden
        /// </summary>
        public Hotkey ToggleAllWindowsHotkey
        {
            get { return this.toggleAllWindowsHotkey; }
            set { this.SetProperty(ref this.toggleAllWindowsHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle intractive windows on/off
        /// </summary>
        public Hotkey ToggleInteractiveWindowsHotkey
        {
            get { return this.toggleInteractiveWindowsHotkey; }
            set { this.SetProperty(ref this.toggleInteractiveWindowsHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle notification window borders on/off
        /// </summary>
        public Hotkey ToggleNotificationWindowBordersHotkey
        {
            get { return this.toggleNotificationWindowBordersHotkey; }
            set { this.SetProperty(ref this.toggleNotificationWindowBordersHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the auto-fade borders feature on/off
        /// </summary>
        public Hotkey ToggleAutoFadeBordersHotkey
        {
            get { return this.toggleAutoFadeBordersHotkey; }
            set { this.SetProperty(ref this.toggleAutoFadeBordersHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the overlay menu icon on/off
        /// </summary>
        public Hotkey ToggleOverlayMenuIconHotkey
        {
            get { return this.toggleOverlayMenuIconHotkey; }
            set { this.SetProperty(ref this.toggleOverlayMenuIconHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the world boss timers on/off
        /// </summary>
        public Hotkey ToggleWorldBossTimerHotkey
        {
            get { return this.toggleWorldBossTimerHotkey; }
            set { this.SetProperty(ref this.toggleWorldBossTimerHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the meta event timers on/off
        /// </summary>
        public Hotkey ToggleMetaEventTimerHotkey
        {
            get { return this.toggleMetaEventTimerHotkey; }
            set { this.SetProperty(ref this.toggleMetaEventTimerHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the dungeons tracker on/off
        /// </summary>
        public Hotkey ToggleDungeonsTrackerHotkey
        {
            get { return this.toggleDungeonsTrackerHotkey; }
            set { this.SetProperty(ref this.toggleDungeonsTrackerHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the dungeon timer on/off
        /// </summary>
        public Hotkey ToggleDungeonTimerHotkey
        {
            get { return this.toggleDungeonTimerHotkey; }
            set { this.SetProperty(ref this.toggleDungeonTimerHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the price tracker on/off
        /// </summary>
        public Hotkey TogglePriceTrackerHotkey
        {
            get { return this.togglePriceTrackerHotkey; }
            set { this.SetProperty(ref this.togglePriceTrackerHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the WvW tracker on/off
        /// </summary>
        public Hotkey ToggleWvWTrackerHotkey
        {
            get { return this.toggleWvWTrackerHotkey; }
            set { this.SetProperty(ref this.toggleWvWTrackerHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the zone assistant on/off
        /// </summary>
        public Hotkey ToggleZoneAssistantHotkey
        {
            get { return this.toggleZoneAssistantHotkey; }
            set { this.SetProperty(ref this.toggleZoneAssistantHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the task tracker on/off
        /// </summary>
        public Hotkey ToggleTaskTrackerHotkey
        {
            get { return this.toggleTaskTrackerHotkey; }
            set { this.SetProperty(ref this.toggleTaskTrackerHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the teamspeak overlay on/off
        /// </summary>
        public Hotkey ToggleTeamspeakTrackerHotkey
        {
            get { return this.toggleTeamspeakTrackerHotkey; }
            set { this.SetProperty(ref this.toggleTeamspeakTrackerHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the web browser on/off
        /// </summary>
        public Hotkey ToggleWebBrowserHotkey
        {
            get { return this.toggleWebBrowserHotkey; }
            set { this.SetProperty(ref this.toggleWebBrowserHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the map overlay on/off
        /// </summary>
        public Hotkey ToggleMapHotkey
        {
            get { return this.toggleMapHotkey; }
            set { this.SetProperty(ref this.toggleMapHotkey, value); }
        }

        /// <summary>
        /// Hotkey to toggle the day/night timer on/off
        /// </summary>
        public Hotkey ToggleDayNightTimerHotkey
        {
            get { return this.toggleDayNightTimerHotkey; }
            set { this.SetProperty(ref this.toggleDayNightTimerHotkey, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="globalHotkeyManager">The global hotkey manager</param>
        [ImportingConstructor]
        public HotkeySettingsViewModel(IHotkeyManager globalHotkeyManager)
        {
            this.GlobalHotkeyManager = globalHotkeyManager;
        }

        /// <summary>
        /// Initializes all hotkeys
        /// </summary>
        public void InitializeHotkeys()
        {
            // Initialize as blank at first
            this.ToggleWorldBossTimerHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleMetaEventTimerHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleDungeonsTrackerHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleDungeonTimerHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.TogglePriceTrackerHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleWvWTrackerHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleZoneAssistantHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleTaskTrackerHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleTeamspeakTrackerHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleWebBrowserHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleInteractiveWindowsHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleAllWindowsHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleNotificationWindowBordersHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleAutoFadeBordersHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleOverlayMenuIconHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleMapHotkey = new Hotkey(Key.None, KeyModifier.None);
            this.ToggleDayNightTimerHotkey = new Hotkey(Key.None, KeyModifier.None);

            // Try to load the hotkeys from user settings
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Hotkeys))
            {
                logger.Debug("Loading hotkeys");
                try
                {
                    var loadedHotkeys = JsonConvert.DeserializeObject<HotkeySettingsViewModel>(Properties.Settings.Default.Hotkeys);
                    if (loadedHotkeys != null)
                    {
                        if (loadedHotkeys.ToggleAllWindowsHotkey != null)
                            this.ToggleAllWindowsHotkey = loadedHotkeys.ToggleAllWindowsHotkey;

                        if (loadedHotkeys.ToggleInteractiveWindowsHotkey != null)
                            this.ToggleInteractiveWindowsHotkey = loadedHotkeys.ToggleInteractiveWindowsHotkey;

                        if (loadedHotkeys.ToggleNotificationWindowBordersHotkey != null)
                            this.ToggleNotificationWindowBordersHotkey = loadedHotkeys.ToggleNotificationWindowBordersHotkey;

                        if (loadedHotkeys.ToggleAutoFadeBordersHotkey != null)
                            this.ToggleAutoFadeBordersHotkey = loadedHotkeys.ToggleAutoFadeBordersHotkey;

                        if (loadedHotkeys.ToggleOverlayMenuIconHotkey != null)
                            this.ToggleOverlayMenuIconHotkey = loadedHotkeys.ToggleOverlayMenuIconHotkey;

                        if (loadedHotkeys.ToggleWorldBossTimerHotkey != null)
                            this.ToggleWorldBossTimerHotkey = loadedHotkeys.ToggleWorldBossTimerHotkey;

                        if (loadedHotkeys.ToggleMetaEventTimerHotkey != null)
                            this.ToggleMetaEventTimerHotkey = loadedHotkeys.ToggleMetaEventTimerHotkey;

                        if (loadedHotkeys.ToggleDungeonsTrackerHotkey != null)
                            this.ToggleDungeonsTrackerHotkey = loadedHotkeys.ToggleDungeonsTrackerHotkey;

                        if (loadedHotkeys.ToggleDungeonTimerHotkey != null)
                            this.ToggleDungeonTimerHotkey = loadedHotkeys.ToggleDungeonTimerHotkey;

                        if (loadedHotkeys.TogglePriceTrackerHotkey != null)
                            this.TogglePriceTrackerHotkey = loadedHotkeys.TogglePriceTrackerHotkey;

                        if (loadedHotkeys.ToggleWvWTrackerHotkey != null)
                            this.ToggleWvWTrackerHotkey = loadedHotkeys.ToggleWvWTrackerHotkey;

                        if (loadedHotkeys.ToggleZoneAssistantHotkey != null)
                            this.ToggleZoneAssistantHotkey = loadedHotkeys.ToggleZoneAssistantHotkey;

                        if (loadedHotkeys.ToggleTaskTrackerHotkey != null)
                            this.ToggleTaskTrackerHotkey = loadedHotkeys.ToggleTaskTrackerHotkey;

                        if (loadedHotkeys.ToggleTeamspeakTrackerHotkey != null)
                            this.ToggleTeamspeakTrackerHotkey = loadedHotkeys.ToggleTeamspeakTrackerHotkey;

                        if (loadedHotkeys.ToggleWebBrowserHotkey != null)
                            this.ToggleWebBrowserHotkey = loadedHotkeys.ToggleWebBrowserHotkey;

                        if (loadedHotkeys.ToggleMapHotkey != null)
                            this.ToggleMapHotkey = loadedHotkeys.ToggleMapHotkey;

                        if (loadedHotkeys.ToggleDayNightTimerHotkey != null)
                            this.ToggleDayNightTimerHotkey = loadedHotkeys.ToggleDayNightTimerHotkey;
                    }
                    else
                    {
                        logger.Warn("Unable to load all user hotkeys!");
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Unable to load user hotkeys! Exception: ", ex);
                }
            }

            // Register for the pause/resume commands
            HotkeyCommands.PauseHotkeys.RegisterCommand(new DelegateCommand(this.GlobalHotkeyManager.PauseHotkeys));
            HotkeyCommands.ResumeHotkeys.RegisterCommand(new DelegateCommand(this.GlobalHotkeyManager.ResumeHotkeys));

            // Wire the hotkeys up
            this.ToggleAllWindowsHotkey.Pressed += (o, e) => HotkeyCommands.ToggleAllWindowsCommand.Execute(null);
            this.ToggleInteractiveWindowsHotkey.Pressed += (o, e) => HotkeyCommands.ToggleInteractiveWindowsCommand.Execute(null);
            this.ToggleNotificationWindowBordersHotkey.Pressed += (o, e) => HotkeyCommands.ToggleNotificationWindowBordersCommand.Execute(null);
            this.ToggleAutoFadeBordersHotkey.Pressed += (o, e) => HotkeyCommands.ToggleAutoFadeBordersCommand.Execute(null);
            this.ToggleOverlayMenuIconHotkey.Pressed += (o, e) => HotkeyCommands.ToggleOverlayMenuIconCommand.Execute(null);
            this.ToggleWorldBossTimerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleWorldBossTimersCommand.Execute(null);
            this.ToggleMetaEventTimerHotkey.Pressed += (e, r) => HotkeyCommands.ToggleMetaEventTimersCommand.Execute(null);
            this.ToggleDungeonsTrackerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleDungeonsTrackerCommand.Execute(null);
            this.ToggleDungeonTimerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleDungeonTimerCommand.Execute(null);
            this.TogglePriceTrackerHotkey.Pressed += (o, e) => HotkeyCommands.TogglePriceTrackerCommand.Execute(null);
            this.ToggleWvWTrackerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleWvWTrackerCommmand.Execute(null);
            this.ToggleZoneAssistantHotkey.Pressed += (o, e) => HotkeyCommands.ToggleZoneAssistantCommand.Execute(null);
            this.ToggleTaskTrackerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleTaskTrackerCommand.Execute(null);
            this.ToggleTeamspeakTrackerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleTeamspeakOverlayCommand.Execute(null);
            this.ToggleWebBrowserHotkey.Pressed += (o, e) => HotkeyCommands.ToggleWebBrowserCommand.Execute(null);
            this.ToggleMapHotkey.Pressed += (o, e) => HotkeyCommands.ToggleMapOverlayCommand.Execute(null);
            this.ToggleDayNightTimerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleDayNightTimerCommand.Execute(null);

            // Register all hotkeys that are enabled
            if (this.ToggleAllWindowsHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleAllWindowsHotkey);
            if (this.ToggleInteractiveWindowsHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleInteractiveWindowsHotkey);
            if (this.ToggleNotificationWindowBordersHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleNotificationWindowBordersHotkey);
            if (this.ToggleAutoFadeBordersHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleAutoFadeBordersHotkey);
            if (this.ToggleOverlayMenuIconHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleOverlayMenuIconHotkey);
            if (this.ToggleWorldBossTimerHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleWorldBossTimerHotkey);
            if (this.ToggleMetaEventTimerHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleMetaEventTimerHotkey);
            if (this.ToggleDungeonsTrackerHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleDungeonsTrackerHotkey);
            if (this.ToggleDungeonTimerHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleDungeonTimerHotkey);
            if (this.TogglePriceTrackerHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.TogglePriceTrackerHotkey);
            if (this.ToggleWvWTrackerHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleWvWTrackerHotkey);
            if (this.ToggleZoneAssistantHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleZoneAssistantHotkey);
            if (this.ToggleZoneAssistantHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleZoneAssistantHotkey);
            if (this.ToggleTaskTrackerHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleTaskTrackerHotkey);
            if (this.ToggleTeamspeakTrackerHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleTeamspeakTrackerHotkey);
            if (this.ToggleWebBrowserHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleWebBrowserHotkey);
            if (this.ToggleMapHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleMapHotkey);
            if (this.ToggleDayNightTimerHotkey.Key != Key.None)
                this.GlobalHotkeyManager.Register(this.ToggleDayNightTimerHotkey);
        }

        /// <summary>
        /// Saves all hotkeys in the user settings
        /// </summary>
        public void SaveHotkeys()
        {
            string hotkeyData = JsonConvert.SerializeObject(this);
            Properties.Settings.Default.Hotkeys = hotkeyData;
            Properties.Settings.Default.Save();
        }
    }
}
