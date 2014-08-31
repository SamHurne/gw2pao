using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.PresentationCore;

namespace GW2PAO.ViewModels.Teamspeak
{
    public class TSNotificationViewModel : NotifyPropertyChangedBase
    {
        private string user;
        private string message;
        private bool isVisible;

        /// <summary>
        /// Client ID
        /// </summary>
        public uint ClientID
        {
            get;
            private set;
        }

        /// <summary>
        /// Username/nickname of the client
        /// </summary>
        public string User
        {
            get { return this.user; }
            set { this.SetField(ref this.user, value); }
        }

        /// <summary>
        /// The client notification type
        /// </summary>
        public TSNotificationType NotificationType
        {
            get;
            private set;
        }

        /// <summary>
        /// Message (if any) for the notification
        /// </summary>
        public string Message
        {
            get { return this.message; }
            set { this.SetField(ref this.message, value); }
        }

        /// <summary>
        /// True if the notification is visible, else false
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { this.SetField(ref this.isVisible, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="clientId">client ID of the client</param>
        /// <param name="user">The username/nickname of the client</param>
        /// <param name="notificationType">The type of client notification</param>
        /// <param name="message">Message (if any) for the notification</param>
        public TSNotificationViewModel(uint clientId, string user, TSNotificationType notificationType, string message = "")
        {
            this.ClientID = clientId;
            this.User = user;
            this.NotificationType = notificationType;
            this.Message = message;
            this.IsVisible = true;
        }

        /// <summary>
        /// Compares an input object to this object. Value compare
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj != null
                && obj is TSNotificationViewModel)
            {
                TSNotificationViewModel other = obj as TSNotificationViewModel;

                return (other.ClientID == this.ClientID)
                    && (other.User == this.User)
                    && (other.NotificationType == this.NotificationType)
                    && (other.Message == this.Message);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + this.ClientID.GetHashCode();
                hash = hash * 23 + this.User.GetHashCode();
                hash = hash * 23 + this.NotificationType.GetHashCode();
                hash = hash * 23 + this.Message.GetHashCode();
                return hash;
            }
        }
    }

    public enum TSNotificationType
    {
        CannotConnect,
        Speech,
        Text,
        UserEntered,
        UserExited
    }
}
