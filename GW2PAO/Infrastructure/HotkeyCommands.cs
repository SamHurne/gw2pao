using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace GW2PAO.Infrastructure
{
    public static class HotkeyCommands
    {
        public static readonly CompositeCommand ToggleAllWindowsCommand = new CompositeCommand();

        public static readonly CompositeCommand ToggleInteractiveWindowsCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleNotificationWindowBordersCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleAutoFadeBordersCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleOverlayMenuIconCommand = new CompositeCommand();

        public static readonly CompositeCommand ToggleWorldBossTimersCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleMetaEventTimersCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleDungeonsTrackerCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleDungeonTimerCommand = new CompositeCommand();
        public static readonly CompositeCommand TogglePriceTrackerCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleWvWTrackerCommmand = new CompositeCommand();
        public static readonly CompositeCommand ToggleZoneAssistantCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleTaskTrackerCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleTeamspeakOverlayCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleWebBrowserCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleMapOverlayCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleDayNightTimerCommand = new CompositeCommand();

        // This isn't really a hotkey, but is related to hotkeys
        public static readonly CompositeCommand PauseHotkeys = new CompositeCommand();
        public static readonly CompositeCommand ResumeHotkeys = new CompositeCommand();
    }
}
