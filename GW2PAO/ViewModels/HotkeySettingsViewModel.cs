using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GW2PAO.Infrastructure;
using GW2PAO.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using NHotkey.Wpf;

namespace GW2PAO.ViewModels
{
    [Export(typeof(HotkeySettingsViewModel))]
    public class HotkeySettingsViewModel : BindableBase, ISettingsViewModel
    {
        private Hotkey toggleAllWindowsHotkey;
        private Hotkey toggleInteractiveWindowsHotkey;
        private Hotkey toggleNotificationWindowBordersHotkey;
        private Hotkey toggleOverlayMenuIconHotkey;
        private Hotkey toggleEventTrackerHotkey;
        private Hotkey toggleDungeonsTrackerHotkey;
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
            get { return "Hotkeys"; }
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
            this.ToggleAllWindowsHotkey = new Hotkey(Key.F9, true, false, false, false);
            this.ToggleAllWindowsHotkey.Pressed += (o, e) => HotkeyCommands.ToggleAllWindowsCommand.Execute(null);
            this.ToggleAllWindowsHotkey.IsRegistered = true;

            this.ToggleInteractiveWindowsHotkey = new Hotkey(Key.F8, true, false, false, false);
            this.ToggleInteractiveWindowsHotkey.Pressed += (o, e) => HotkeyCommands.ToggleInteractiveWindowsCommand.Execute(null);
            this.ToggleInteractiveWindowsHotkey.IsRegistered = true;

            this.ToggleNotificationWindowBordersHotkey = new Hotkey(Key.F10, true, false, false, false);
            this.ToggleNotificationWindowBordersHotkey.Pressed += (o, e) => HotkeyCommands.ToggleNotificationWindowBordersCommand.Execute(null);
            this.ToggleNotificationWindowBordersHotkey.IsRegistered = true;

            this.ToggleOverlayMenuIconHotkey = new Hotkey(Key.F11, true, false, false, false);
            this.ToggleOverlayMenuIconHotkey.Pressed += (o, e) => HotkeyCommands.ToggleOverlayMenuIconCommand.Execute(null);
            this.ToggleOverlayMenuIconHotkey.IsRegistered = true;

            this.ToggleEventTrackerHotkey = new Hotkey(Key.F1, true, false, false, false);
            this.ToggleEventTrackerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleEventTrackerCommand.Execute(null);
            this.ToggleEventTrackerHotkey.IsRegistered = true;

            this.ToggleDungeonsTrackerHotkey = new Hotkey(Key.F2, true, false, false, false);
            this.ToggleDungeonsTrackerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleDungeonsTrackerCommand.Execute(null);
            this.ToggleDungeonsTrackerHotkey.IsRegistered = true;

            this.TogglePriceTrackerHotkey = new Hotkey(Key.F3, true, false, false, false);
            this.TogglePriceTrackerHotkey.Pressed += (o, e) => HotkeyCommands.TogglePriceTrackerCommand.Execute(null);
            this.TogglePriceTrackerHotkey.IsRegistered = true;

            this.ToggleWvWTrackerHotkey = new Hotkey(Key.F4, true, false, false, false);
            this.ToggleWvWTrackerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleWvWTrackerCommmand.Execute(null);
            this.ToggleWvWTrackerHotkey.IsRegistered = true;

            this.ToggleZoneAssistantHotkey = new Hotkey(Key.F5, true, false, false, false);
            this.ToggleZoneAssistantHotkey.Pressed += (o, e) => HotkeyCommands.ToggleZoneAssistantCommand.Execute(null);
            this.ToggleZoneAssistantHotkey.IsRegistered = true;

            this.ToggleTeamspeakTrackerHotkey = new Hotkey(Key.F6, true, false, false, false);
            this.ToggleTeamspeakTrackerHotkey.Pressed += (o, e) => HotkeyCommands.ToggleTeamspeakOverlayCommand.Execute(null);
            this.ToggleTeamspeakTrackerHotkey.IsRegistered = true;

            this.ToggleWebBrowserHotkey = new Hotkey(Key.F7, true, false, false, false);
            this.ToggleWebBrowserHotkey.Pressed += (o, e) => HotkeyCommands.ToggleWebBrowserCommand.Execute(null);
            this.ToggleWebBrowserHotkey.IsRegistered = true;
        }
    }
}
