using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.PresentationCore;
using GW2PAO.ViewModels.PriceNotification;
using GW2PAO.ViewModels.TradingPost;
using NLog;

namespace GW2PAO.Models
{
    /// <summary>
    /// User settings for Commerce overlays, like the price notifications
    /// </summary>
    [Serializable]
    public class CommerceSettings : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public static string Filename { get { return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".CommerceSettings.xml"; } }

        private bool areBuyOrderPriceNotificationsEnabled;
        private bool areSellListingPriceNotificationsEnabled;
        private ObservableCollection<PriceWatch> priceWatches = new ObservableCollection<PriceWatch>();

        /// <summary>
        /// True if buy order price notifications are enabled, else false
        /// </summary>
        public bool AreBuyOrderPriceNotificationsEnabled
        {
            get { return areBuyOrderPriceNotificationsEnabled; }
            set { this.SetField(ref this.areBuyOrderPriceNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if sell listing price notifications are enabled, else false
        /// </summary>
        public bool AreSellListingPriceNotificationsEnabled
        {
            get { return areSellListingPriceNotificationsEnabled; }
            set { this.SetField(ref this.areSellListingPriceNotificationsEnabled, value); }
        }

        /// <summary>
        /// Collection of price watches for the price watch notifications
        /// </summary>
        public ObservableCollection<PriceWatch> PriceWatches { get { return this.priceWatches; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CommerceSettings()
        {
            // Defaults:
            this.AreBuyOrderPriceNotificationsEnabled = true;
            this.AreSellListingPriceNotificationsEnabled = true;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => CommerceSettings.SaveSettings(this);
            this.PriceWatches.CollectionChanged += PriceWatches_CollectionChanged;

            foreach (var pw in this.PriceWatches)
            {
                pw.PropertyChanged += (o, arg) => CommerceSettings.SaveSettings(this);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the PriceWatches collection
        /// </summary>
        private void PriceWatches_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // If an item is added, register for it's property changed event so we can save settings when it changes
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PriceWatch newItem in e.NewItems)
                {
                    newItem.PropertyChanged += (o, arg) => CommerceSettings.SaveSettings(this);
                }
            }

            CommerceSettings.SaveSettings(this);
        }

        /// <summary>
        /// Loads the user settings
        /// </summary>
        /// <returns>The loaded DungeonSettings, or null if the load fails</returns>
        public static CommerceSettings LoadSettings()
        {
            logger.Debug("Loading user settings");

            XmlSerializer deserializer = new XmlSerializer(typeof(CommerceSettings));
            object loadedSettings = null;

            if (File.Exists(Filename))
            {
                try
                {
                    using (TextReader reader = new StreamReader(Filename))
                    {
                        loadedSettings = deserializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Unable to load user settings! Exception: ", ex);
                }
            }

            if (loadedSettings != null)
            {
                logger.Info("Settings successfully loaded");
                return loadedSettings as CommerceSettings;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Saves the user settings
        /// </summary>
        /// <param name="settings">The user settings to save</param>
        public static void SaveSettings(CommerceSettings settings)
        {
            logger.Debug("Saving user settings");
            XmlSerializer serializer = new XmlSerializer(typeof(CommerceSettings));
            using (TextWriter writer = new StreamWriter(Filename))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}
