using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Constants;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Util;
using NLog;

namespace GW2PAO.API.Providers
{
    public class WvWObjectiveNamesProvider : IStringProvider<int, bool>
    {

        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loaded world event names
        /// </summary>
        private List<ObjectiveNames> objectives;

        /// <summary>
        /// Locking object for accessing the loadedNames list
        /// </summary>
        private readonly object objectivesLock = new object();

        /// <summary>
        /// Default constructor
        /// </summary>
        public WvWObjectiveNamesProvider()
        {
            // By default, load the CurrentUICulture table of event names
            lock (this.objectivesLock)
            {
                try
                {
                    this.objectives = this.LoadNames(CultureInfo.CurrentUICulture);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex);
                }

                if (this.objectives == null)
                {
                    this.GenerateFiles();
                    this.objectives = this.LoadNames(CultureInfo.CurrentUICulture);
                }
            }
        }

        /// <summary>
        /// Changes the culture used for localization of strings
        /// </summary>
        /// <param name="culture">The culture to use for localization</param>
        public void SetCulture(CultureInfo culture)
        {
            var loadedNames = this.LoadNames(culture);
            if (loadedNames != null)
            {
                lock (this.objectivesLock)
                {
                    this.objectives = loadedNames;
                }
            }
        }

        /// <summary>
        /// Retrieves a string using the given identifier
        /// </summary>
        /// <param name="id">The ID of the WvW objective</param>
        /// <param name="shortName">Set to true if the shortname should be retrieved, else false for the full name</param>
        /// <returns>The localized name of the WvW objective</returns>
        public string GetString(int id, bool shortName)
        {
            var result = string.Empty;
            lock (this.objectivesLock)
            {
                var match = this.objectives.FirstOrDefault(obj => obj.ID == id);
                if (match != null)
                {
                    if (shortName)
                        result = match.ShortName;
                    else
                        result = match.Name;
                }
            }
            return result;
        }

        /// <summary>
        /// Loads the collection of objective names from file
        /// </summary>
        /// <param name="culture">The culture to load</param>
        /// <returns>The loaded collection of event names</returns>
        private List<ObjectiveNames> LoadNames(CultureInfo culture)
        {
            var lang = culture.TwoLetterISOLanguageName;

            var supported = new[] { "en", "es", "fr", "de" };
            if (!supported.Contains(lang))
                lang = "en"; // Default to english if not supported

            var filename = this.GetFilePath(lang);

            List<ObjectiveNames> loadedData = null;
            if (File.Exists(filename))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<ObjectiveNames>));
                TextReader reader = new StreamReader(filename);
                try
                {
                    object obj = deserializer.Deserialize(reader);
                    loadedData = (List<ObjectiveNames>)obj;
                }
                finally
                {
                    reader.Close();
                }
            }

            return loadedData;
        }

        /// <summary>
        /// Creates the objective names files
        /// </summary>
        /// <returns></returns>
        private void GenerateFiles()
        {
            // English
            List<ObjectiveNames> english = new List<ObjectiveNames>()
            {
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Keep_Overlook, Name = "Overlook", ShortName = "Overlook" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Keep_Valley, Name = "Valley", ShortName = "Valley" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Keep_Lowlands, Name = "Lowlands", ShortName = "Lowlands" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Golanta, Name = "Golanta Clearing", ShortName = "Golanta" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Pangloss, Name = "Pangloss Rise", ShortName = "Pangloss" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Speldan, Name = "Speldan Clearcut", ShortName = "Speldan" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Danelon, Name = "Danelon Passage", ShortName = "Danelon" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Umberglade, Name = "Umberglade Woods", ShortName = "Umberglade" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Castle_Stonemist, Name = "Stonemist Castle", ShortName = "Stonemist" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Rogues, Name = "Rogue's Quarry", ShortName = "Rogue's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Aldons, Name = "Aldon's Ledge", ShortName = "Aldon's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Wildcreek, Name = "Wildcreek Run", ShortName = "Wildcreek" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Jerrifers, Name = "Jerrifer's Slough", ShortName = "Jerrifer's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Klovan, Name = "Klovan Gully", ShortName = "Klovan" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Langor, Name = "Langor Gulch", ShortName = "Langor" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Quentin, Name = "Quentin Lake", ShortName = "Quentin" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Mendons, Name = "Mendon's Gap", ShortName = "Mendon's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Anzalias, Name = "Anzalias Pass", ShortName = "Anzalias" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Ogrewatch, Name = "Ogrewatch Cut", ShortName = "Ogrewatch" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Veloka, Name = "Veloka Slope", ShortName = "Veloka" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Durios, Name = "Durios Gulch", ShortName = "Durios" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Bravost, Name = "Bravost Escarpment", ShortName = "Bravost" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Keep_Garrison, Name = "Garrison", ShortName = "Garrison" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Orchard, Name = "Champion's Demense", ShortName = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Redbriar, Name = "Redbriar", ShortName = "Redbriar" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Greenlake, Name = "Greenlake", ShortName = "Greenlake" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Keep_Bay, Name = "Ascension Bay", ShortName = "Bay" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Dawns, Name = "Dawn's Eyrie", ShortName = "Dawn's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Spiritholme, Name = "The Spiritholme", ShortName = "Spiritholme" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Woodhaven, Name = "Woodhaven", ShortName = "Woodhaven" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Keep_Hills, Name = "Askalion Hills", ShortName = "Hills" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Keep_Hills, Name = "Etheron Hills", ShortName = "Hills" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Keep_Bay, Name = "Dreaming Bay", ShortName = "Bay" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Orchard, Name = "Victors's Lodge", ShortName = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Greenbriar, Name = "Greenbriar", ShortName = "Greenbriar" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Bluelake, Name = "Bluelake", ShortName = "Bluelake" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Keep_Garrison, Name = "Garrison", ShortName = "Garrison" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Longview, Name = "Longview", ShortName = "Longview" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Godsword, Name = "The Godsword", ShortName = "Godsword" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Cliffside, Name = "Cliffside", ShortName = "Cliffside" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Keep_Hills, Name = "Shadaran Hills", ShortName = "Hills" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Redlake, Name = "Redlake", ShortName = "Redlake" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Orchard, Name = "Hero's Lodge", ShortName = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Keep_Bay, Name = "Dreadfall Bay", ShortName = "Bay" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Bluebriar, Name = "Bluebriar", ShortName = "Bluebriar" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Keep_Garrison, Name = "Garrison", ShortName = "Garrison" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Sunnyhill, Name = "Sunnyhill", ShortName = "Sunnyhill" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Faithleap, Name = "Faithleap", ShortName = "Faithleap" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Bluevale, Name = "Bluevale Refuge", ShortName = "Bluevale" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Bluewater, Name = "Bluewater Lowlands", ShortName = "Bluewater" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Astralholme, Name = "Astralholme", ShortName = "Astralholme" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Arahs, Name = "Arah's Hope", ShortName = "Arah's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Greenvale, Name = "Greenvale Refuge", ShortName = "Greenvale" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Foghaven, Name = "Foghaven", ShortName = "Foghaven" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Redwater, Name = "Redwater Lowlands", ShortName = "Redwater" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Titanpaw, Name = "The Titanpaw", ShortName = "Titanpaw" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Cragtop, Name = "Cragtop", ShortName = "Cragtop" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Godslore, Name = "Godslore", ShortName = "Godslore" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Redvale, Name = "Redvale Refuge", ShortName = "Redvale" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Stargrove, Name = "Stargrove", ShortName = "Stargrove" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Greenwater, Name = "Greenwater Lowlands", ShortName = "Greenwater" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Temple, Name = "Temple of Lost Prayers", ShortName = "Temple" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Hollow, Name = "Battle's Hollow", ShortName = "Hollow" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Estate, Name = "Bauer's Estate", ShortName = "Estate" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Orchard, Name = "Orchard Overlook", ShortName = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Carvers, Name = "Carver's Ascent", ShortName = "Carver's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Carvers, Name = "Carver's Ascent", ShortName = "Carver's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Orchard, Name = "Orchard Overlook", ShortName = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Estate, Name = "Bauer's Estate", ShortName = "Estate" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Hollow, Name = "Battle's Hollow", ShortName = "Hollow" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Temple, Name = "Temple of Lost Prayers", ShortName = "Temple" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Carvers, Name = "Carver's Ascent", ShortName = "Carver's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Orchard, Name = "Orchard Overlook", ShortName = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Estate, Name = "Bauer's Estate", ShortName = "Estate" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Hollow, Name = "Battle's Hollow", ShortName = "Hollow" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Temple, Name = "Temple of Lost Prayers", ShortName = "Temple" }
            };

            // Spanish TODO
            List<ObjectiveNames> spanish = new List<ObjectiveNames>();

            // French TODO
            List<ObjectiveNames> french = new List<ObjectiveNames>();

            // German TODO
            List<ObjectiveNames> german = new List<ObjectiveNames>();

            Serializer.SerializeToXml(english, this.GetFilePath("en"));
            Serializer.SerializeToXml(english, this.GetFilePath("es"));
            Serializer.SerializeToXml(english, this.GetFilePath("fr"));
            Serializer.SerializeToXml(english, this.GetFilePath("de"));
        }

        /// <summary>
        /// Retrieves the full path of the stored names file using the given culture
        /// </summary>
        private string GetFilePath(string twoLetterIsoLangId)
        {
            string filename = Paths.LocalizationFolder + "WvWObjectiveNames";

            if (twoLetterIsoLangId != "en")
                filename += "." + twoLetterIsoLangId;

            filename += ".xml";

            return filename;
        }

        /// <summary>
        /// Container class for objective names
        /// </summary>
        public class ObjectiveNames
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
        }
    }
}
