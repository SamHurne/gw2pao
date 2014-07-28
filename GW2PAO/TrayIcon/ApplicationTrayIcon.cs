using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using GW2PAO.Utility;
using Hardcodet.Wpf.TaskbarNotification;
using NLog;

namespace GW2PAO.TrayIcon
{
    /// <summary>
    /// Wrapper class for the main application's tray icon
    /// </summary>
    public class ApplicationTrayIcon : IApplicationTrayIcon
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The taskbar tray icon object
        /// </summary>
        private TaskbarIcon taskbarIcon;

        /// <summary>
        /// The tray icon's main context menu
        /// </summary>
        public object ContextMenu
        {
            get { return this.taskbarIcon.ContextMenu; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="icon">The tray icon object to use as the main application's tray icon</param>
        public ApplicationTrayIcon(TaskbarIcon icon)
        {
            this.taskbarIcon = icon;
        }

        /// <summary>
        /// Displays a pop-up notification near the tray icon
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="text">Main text for the notification</param>
        /// <param name="messageType">Type of message/notification</param>
        public void DisplayNotification(string title, string text, TrayInfoMessageType messageType)
        {
            logger.Info("Displaying notification bubble. Title: \"{0}\" Text: \"{1}\" MessageType: {2}", title, text, messageType);
            BalloonIcon icon;
            switch (messageType)
            {
                case TrayInfoMessageType.None:
                    icon = BalloonIcon.None;
                    break;
                case TrayInfoMessageType.Info:
                    icon = BalloonIcon.Info;
                    break;
                case TrayInfoMessageType.Warning:
                    icon = BalloonIcon.Warning;
                    break;
                case TrayInfoMessageType.Error:
                    icon = BalloonIcon.Error;
                    break;
                default:
                    icon = BalloonIcon.None;
                    break;
            }

            Threading.BeginInvokeOnUI(() => this.taskbarIcon.ShowBalloonTip(title, text, icon));
        }

        /// <summary>
        /// Displays a custom pop-up notification
        /// </summary>
        /// <param name="notificationView">Control to use for the pop-up notification</param>
        /// <param name="animation">Animation to use when displaying the notification</param>
        /// <param name="timeout">Timeout for how long the notification should remain displayed, in ms</param>
        public void DisplayCustomNotification(UIElement notificationView, PopupAnimation animation = PopupAnimation.Fade, int? timeout = 10000)
        {
            Threading.BeginInvokeOnUI(() => this.taskbarIcon.ShowCustomBalloon(notificationView, animation, timeout));
        }

        /// <summary>
        /// Closes any displayed custom notifications
        /// </summary>
        public void CloseCustomNotification()
        {
            Threading.BeginInvokeOnUI(() => this.taskbarIcon.CloseBalloon());
        }
    }
}
