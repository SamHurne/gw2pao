using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Data.UserData
{
    /// <summary>
    /// Base class for user data classes
    /// </summary>
    /// <typeparam name="T">Type of user data class, used when loading/saving the data</typeparam>
    public class UserData<T> : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings directory
        /// </summary>
        public static string DataDirectory
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "UserData" + Path.DirectorySeparatorChar; }
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public virtual void EnableAutoSave()
        {
        }

        /// <summary>
        /// Loads the user settings
        /// </summary>
        /// <returns>The loaded settings, or null if the load fails</returns>
        public static T LoadData(string fileName)
        {
            logger.Debug("Loading user settings");

            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            object loadedSettings = null;

            string fullPath = DataDirectory + fileName;

            try
            {
                if (File.Exists(fullPath))
                {
                    using (TextReader reader = new StreamReader(fullPath))
                    {
                        loadedSettings = deserializer.Deserialize(reader);
                    }
                }

                if (loadedSettings != null)
                {
                    logger.Info("Settings successfully loaded");
                    return (T)loadedSettings;
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Unable to load user settings! Exception: ", ex);
                return default(T);
            }
        }

        /// <summary>
        /// Saves the user settings
        /// </summary>
        /// <param name="settings">The user settings to save</param>
        public static void SaveData(T settings, string fileName)
        {
            logger.Debug("Saving user data");
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }

            string fullPath = DataDirectory + fileName;

            using (TextWriter writer = new StreamWriter(fullPath))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}
