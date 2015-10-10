using GW2PAO.Data.UserData;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Map
{
    /// <summary>
    /// User settings for the Maps Module
    /// </summary>
    [Serializable]
    public class MapUserData : UserData<MapUserData>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public const string Filename = "MapsUserData.xml";

        /// <summary>
        /// Default constructor
        /// </summary>
        public MapUserData()
        {
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => MapUserData.SaveData(this, MapUserData.Filename);
        }
    }
}
