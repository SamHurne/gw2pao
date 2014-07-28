using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace GW2PAO.TrayIcon
{
    /// <summary>
    /// Interface for the main application tray icon
    /// </summary>
    public interface IApplicationTrayIcon
    {
        /// <summary>
        /// The tray icon's main context menu
        /// </summary>
        object ContextMenu { get; }

        /// <summary>
        /// Displays a pop-up notification near the tray icon
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="text">Main text for the notification</param>
        /// <param name="messageType">Type of message/notification</param>
        void DisplayNotification(string title, string text, TrayInfoMessageType messageType);

        /// <summary>
        /// Displays a custom pop-up notification
        /// </summary>
        /// <param name="notificationView">Control to use for the pop-up notification</param>
        /// <param name="animation">Animation to use when displaying the notification</param>
        /// <param name="timeout">Timeout for how long the notification should remain displayed, in ms</param>
        void DisplayCustomNotification(UIElement notificationView, PopupAnimation animation = PopupAnimation.Fade, int? timeout = 10000);

        /// <summary>
        /// Closes any displayed custom notifications
        /// </summary>
        void CloseCustomNotification();
    }
}
