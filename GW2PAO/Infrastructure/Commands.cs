using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace GW2PAO.Infrastructure
{
    public static class Commands
    {
        public static readonly CompositeCommand ApplicationShutdownCommand = new CompositeCommand();

        public static readonly CompositeCommand PromptForRestartCommand = new CompositeCommand();

        public static readonly CompositeCommand OpenGeneralSettingsCommand = new CompositeCommand();
        public static readonly CompositeCommand OpenHotkeySettingsCommand = new CompositeCommand();
        public static readonly CompositeCommand OpenEventSettingsCommand = new CompositeCommand();
        public static readonly CompositeCommand OpenDungeonSettingsCommand = new CompositeCommand();
        public static readonly CompositeCommand OpenCommerceSettingsCommand = new CompositeCommand();
        public static readonly CompositeCommand OpenWvWSettingsCommand = new CompositeCommand();

        public static readonly CompositeCommand CleanupTrayIcon = new CompositeCommand();
    }
}
