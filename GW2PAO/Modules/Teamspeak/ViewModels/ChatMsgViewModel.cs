using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Teamspeak.ViewModels
{
    public class ChatMsgViewModel
    {
        /// <summary>
        /// Timestamp of the message
        /// </summary>
        public DateTime Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the sender
        /// </summary>
        public string SenderName
        {
            get;
            private set;
        }

        /// <summary>
        /// The message's text
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="timestamp">Timestamp of the message</param>
        /// <param name="senderName">Name of the sender</param>
        /// <param name="message">The actual message text</param>
        public ChatMsgViewModel(DateTime timestamp, string senderName, string message)
        {
            this.Timestamp = timestamp;
            this.SenderName = senderName;
            this.Message = message;
        }
    }
}
