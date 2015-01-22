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
using GW2PAO.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json;
using NHotkey.Wpf;
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

        [XmlIgnore]
        private bool isPaused;

        private Hotkey toggleAllWindowsHotkey;
        private Hotkey toggleInteractiveWindowsHotkey;
        private Hotkey toggleNotificationWindowBordersHotkey;
        private Hotkey toggleAutoFadeBordersHotkey;
        private Hotkey toggleOverlayMenuIconHotkey;
        private Hotkey toggleEventTrackerHotkey;
        private Hotkey toggleDungeonsTrackerHotkey;
        private Hotkey toggleDungeonTimerHotkey;
        private Hotkey togglePriceTrackerHotkey;
        private Hotkey toggleWvWTrackerHotkey;
        private Hotkey toggleZoneAssistantHotkey;
        private Hotkey toggleTeamspeakTrackerHotkey;
        private Hotkey toggleWebBrowserHotkey;

        /// <summary>
        /// Header for the settings
        /// </summary>
        public string SettingsHeader
        {
            get { return Properties.Resources.Hotkeys; }
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
        /// Hotkey to toggle the event tracker on/off
        /// </summary>
        public Hotkey ToggleEventTrackerHotkey
        {
            get { return this.toggleEventTrackerHotkey; }
            set { this.SetProperty(ref this.toggleEventTrackerHotkey, value); }
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
        /// Initializes all hotkeys
        /// </summary>
        public void InitializeHotkeys()
        {
            // Initialize as blank at first
            this.ToggleEventTrackerHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleDungeonsTrackerHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleDungeonTimerHotkey = new Hotkey(Key.None, false, false, false, false);
            this.TogglePriceTrackerHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleWvWTrackerHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleZoneAssistantHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleTeamspeakTrackerHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleWebBrowserHotkey = new Hotkey(Key.None, false, true, false, false);
            this.ToggleInteractiveWindowsHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleAllWindowsHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleNotificationWindowBordersHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleAutoFadeBordersHotkey = new Hotkey(Key.None, false, false, false, false);
            this.ToggleOverlayMenuIconHotkey = new Hotkey(Key.None, false, false, false, false);

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
                            this.ToggleOverlayMenuIconHotkey = loadedHotkeys.ToggleAutoFadeBordersHotkey;

                        if (loadedHotkeys.ToggleOverlayMenuIconHotkey != null)
                            this.ToggleOverlayMenuIconHotkey = loadedHotkeys.ToggleOverlayMenuIconHotkey;

                        if (loadedHotkeys.ToggleEventTrackerHotkey != null)
                            this.ToggleEventTrackerHotkey = loadedHotkeys.ToggleEventTrackerHotkey;

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

                        if (loadedHotkeys.ToggleTeamspeakTrackerHotkey != null)
                            this.ToggleTeamspeakTrackerHotkey = loadedHotkeys.ToggleTeamspeakTrackerHotkey;

                        if (loadedHotkeys.ToggleWebBrowserHotkey != null)
                            this.ToggleWebBrowserHotkey = loadedHotkeys.ToggleWebBrowserHotkey;
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
            this.isPaused = true; // We are "paused" until we initialize for the first time
            HotkeyCommands.PauseHotkeys.RegisterCommand(new DelegateCommand(this.UnregisterOnPressedHandlers));
            HotkeyCommands.ResumeHotkeys.RegisterCommand(new DelegateCommand(this.RegisterOnPressedHandlers));

            // Wire the hotkeys up
            this.RegisterOnPressedHandlers();
            this.ToggleAllWindowsHotkey.Refresh();
            this.ToggleInteractiveWindowsHotkey.Refresh();
            this.ToggleNotificationWindowBordersHotkey.Refresh();
            this.ToggleAutoFadeBordersHotkey.Refresh();
            this.ToggleOverlayMenuIconHotkey.Refresh();
            this.ToggleEventTrackerHotkey.Refresh();
            this.ToggleDungeonsTrackerHotkey.Refresh();
            this.ToggleDungeonTimerHotkey.Refresh();
            this.TogglePriceTrackerHotkey.Refresh();
            this.ToggleWvWTrackerHotkey.Refresh();
            this.ToggleZoneAssistantHotkey.Refresh();
            this.ToggleTeamspeakTrackerHotkey.Refresh();
            this.ToggleWebBrowserHotkey.Refresh();
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

        /// <summary>
        /// Registers OnPressed handlers for all hotkeys
        /// </summary>
        private void RegisterOnPressedHandlers()
        {
            if (this.isPaused)
            {
                this.isPaused = false;
                this.ToggleAllWindowsHotkey.Pressed += OnPressed;
                this.ToggleInteractiveWindowsHotkey.Pressed += OnPressed;
                this.ToggleNotificationWindowBordersHotkey.Pressed += OnPressed;
                this.ToggleAutoFadeBordersHotkey.Pressed += OnPressed;
                this.ToggleOverlayMenuIconHotkey.Pressed += OnPressed;
                this.ToggleEventTrackerHotkey.Pressed += OnPressed;
                this.ToggleDungeonsTrackerHotkey.Pressed += OnPressed;
                this.ToggleDungeonTimerHotkey.Pressed += OnPressed;
                this.TogglePriceTrackerHotkey.Pressed += OnPressed;
                this.ToggleWvWTrackerHotkey.Pressed += OnPressed;
                this.ToggleZoneAssistantHotkey.Pressed += OnPressed;
                this.ToggleTeamspeakTrackerHotkey.Pressed += OnPressed;
                this.ToggleWebBrowserHotkey.Pressed += OnPressed;
            }
        }

        /// <summary>
        /// Unregisters OnPressed handlers for all hotkeys
        /// </summary>
        private void UnregisterOnPressedHandlers()
        {
            if (!this.isPaused)
            {
                this.isPaused = true;
                this.ToggleAllWindowsHotkey.Pressed -= OnPressed;
                this.ToggleInteractiveWindowsHotkey.Pressed -= OnPressed;
                this.ToggleNotificationWindowBordersHotkey.Pressed -= OnPressed;
                this.ToggleAutoFadeBordersHotkey.Pressed -= OnPressed;
                this.ToggleOverlayMenuIconHotkey.Pressed -= OnPressed;
                this.ToggleEventTrackerHotkey.Pressed -= OnPressed;
                this.ToggleDungeonsTrackerHotkey.Pressed -= OnPressed;
                this.ToggleDungeonTimerHotkey.Pressed -= OnPressed;
                this.TogglePriceTrackerHotkey.Pressed -= OnPressed;
                this.ToggleWvWTrackerHotkey.Pressed -= OnPressed;
                this.ToggleZoneAssistantHotkey.Pressed -= OnPressed;
                this.ToggleTeamspeakTrackerHotkey.Pressed -= OnPressed;
                this.ToggleWebBrowserHotkey.Pressed -= OnPressed;
            }
        }

        /// <summary>
        /// Handles an OnPressed event for a hotkey
        /// </summary>
        /// <param name="sender">The hotkey that sent the Pressed event</param>
        private void OnPressed(object sender, EventArgs e)
        {
            var hotkey = sender as Hotkey;
            if (hotkey != null)
            {
                if (hotkey == this.ToggleAllWindowsHotkey)
                    HotkeyCommands.ToggleAllWindowsCommand.Execute(null);
                else if (hotkey == this.ToggleInteractiveWindowsHotkey)
                    HotkeyCommands.ToggleInteractiveWindowsCommand.Execute(null);
                else if (hotkey == this.ToggleNotificationWindowBordersHotkey)
                    HotkeyCommands.ToggleNotificationWindowBordersCommand.Execute(null);
                else if (hotkey == this.ToggleAutoFadeBordersHotkey)
                    HotkeyCommands.ToggleAutoFadeBordersCommand.Execute(null);
                else if (hotkey == this.ToggleOverlayMenuIconHotkey)
                    HotkeyCommands.ToggleOverlayMenuIconCommand.Execute(null);
                else if (hotkey == this.ToggleEventTrackerHotkey)
                    HotkeyCommands.ToggleEventTrackerCommand.Execute(null);
                else if (hotkey == this.ToggleDungeonsTrackerHotkey)
                    HotkeyCommands.ToggleDungeonsTrackerCommand.Execute(null);
                else if (hotkey == this.ToggleDungeonTimerHotkey)
                    HotkeyCommands.ToggleDungeonTimerCommand.Execute(null);
                else if (hotkey == this.TogglePriceTrackerHotkey)
                    HotkeyCommands.TogglePriceTrackerCommand.Execute(null);
                else if (hotkey == this.ToggleWvWTrackerHotkey)
                    HotkeyCommands.ToggleWvWTrackerCommmand.Execute(null);
                else if (hotkey == this.ToggleZoneAssistantHotkey)
                    HotkeyCommands.ToggleZoneAssistantCommand.Execute(null);
                else if (hotkey == this.ToggleTeamspeakTrackerHotkey)
                    HotkeyCommands.ToggleTeamspeakOverlayCommand.Execute(null);
                else if (hotkey == this.ToggleWebBrowserHotkey)
                    HotkeyCommands.ToggleWebBrowserCommand.Execute(null);
            }
        }
    }
}
