using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TrayIcon
{
    /// <summary>
    /// Tray notification message type
    /// </summary>
    public enum TrayInfoMessageType
    {
        /// <summary>
        /// No type
        /// </summary>
        None,

        /// <summary>
        /// Informational message
        /// </summary>
        Info,

        /// <summary>
        /// Warning message
        /// </summary>
        Warning,

        /// <summary>
        /// Error message
        /// </summary>
        Error
    }
}
