using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.Data.UserData;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Modules.Teamspeak
{
    /// <summary>
    /// User settings for the Teamspeak Overlay
    /// </summary>
    [Serializable]
    public class TeamspeakUserData : UserData<TeamspeakUserData>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public const string Filename = "TeamspeakUserData.xml";

        private bool showChatEntryBox;
        private bool showChannelName;

        /// <summary>
        /// True to show the chat entry box, else false
        /// </summary>
        public bool ShowChatEntryBox
        {
            get { return this.showChatEntryBox; }
            set { this.SetProperty(ref this.showChatEntryBox, value); }
        }

        /// <summary>
        /// True to show the channel name box, else false
        /// </summary>
        public bool ShowChannelName
        {
            get { return this.showChannelName; }
            set { this.SetProperty(ref this.showChannelName, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TeamspeakUserData()
        {
            this.ShowChatEntryBox = true;
            this.ShowChannelName = true;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => TeamspeakUserData.SaveData(this, TeamspeakUserData.Filename);
        }
    }
}
