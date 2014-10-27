using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Constants;
using GW2PAO.API.Util;
using NLog;

namespace GW2PAO.API.Providers
{
    public class WorldEventNamesProvider : IStringProvider<Guid>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loaded world event names
        /// </summary>
        private List<EventName> worldEvents;

        /// <summary>
        /// Locking object for accessing the loadedNames list
        /// </summary>
        private readonly object worldEventsLock = new object();

        /// <summary>
        /// Default constructor
        /// </summary>
        public WorldEventNamesProvider()
        {
            // By default, load the CurrentUICulture table of event names
            lock (this.worldEventsLock)
            {
                try
                {
                    this.worldEvents = this.LoadNames(CultureInfo.CurrentUICulture);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex);
                }

                if (this.worldEvents == null)
                {
                    this.GenerateFiles();
                    this.worldEvents = this.LoadNames(CultureInfo.CurrentUICulture);
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
                lock (this.worldEventsLock)
                {
                    this.worldEvents = loadedNames;
                }
            }
        }

        /// <summary>
        /// Retrieves a string using the given identifier
        /// </summary>
        /// <param name="id">The ID of the world event</param>
        /// <returns>The localized name of the world event</returns>
        public string GetString(Guid id)
        {
            var result = string.Empty;
            lock (this.worldEventsLock)
            {
                var match = this.worldEvents.FirstOrDefault(evt => evt.ID == id);
                if (match != null)
                    result = match.Name;
            }
            return result;
        }

        /// <summary>
        /// Loads the collection of event names from file
        /// </summary>
        /// <param name="culture">The culture to load</param>
        /// <returns>The loaded collection of event names</returns>
        private List<EventName> LoadNames(CultureInfo culture)
        {
            var lang = culture.TwoLetterISOLanguageName;

            var supported = new[] { "en", "es", "fr", "de" };
            if (!supported.Contains(lang))
                lang = "en"; // Default to english if not supported

            var filename = this.GetFilePath(lang);

            List<EventName> loadedData = null;
            if (File.Exists(filename))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<EventName>));
                TextReader reader = new StreamReader(filename);
                try
                {
                    object obj = deserializer.Deserialize(reader);
                    loadedData = (List<EventName>)obj;
                }
                finally
                {
                    reader.Close();
                }
            }

            return loadedData;
        }

        /// <summary>
        /// Creates the world events names files
        /// </summary>
        /// <returns></returns>
        private void GenerateFiles()
        {
            // English
            List<EventName> english = new List<EventName>()
            {
                new EventName() { ID = WorldEventID.Megadestroyer, Name = "Megadestroyer" },
                new EventName() { ID = WorldEventID.Tequatl, Name = "Tequatl" },
                new EventName() { ID = WorldEventID.KarkaQueen, Name = "Karka Queen" },
                new EventName() { ID = WorldEventID.EvolvedJungleWurm, Name = "Evolved Jungle Wurm" },
                new EventName() { ID = WorldEventID.Shatterer, Name = "Shatterer" },
                new EventName() { ID = WorldEventID.ClawOfJormag, Name = "Claw of Jormag" },
                new EventName() { ID = WorldEventID.ModniirUlgoth, Name = "Modniir Ulgoth" },
                new EventName() { ID = WorldEventID.InquestGolemMarkII, Name = "Inquest Golem Mark II" },
                new EventName() { ID = WorldEventID.TaidhaCovington, Name = "Taidha Covington" },
                new EventName() { ID = WorldEventID.JungleWurm, Name = "Jungle Wurm" },
                new EventName() { ID = WorldEventID.ShadowBehemoth, Name = "Shadow Behemoth" },
                new EventName() { ID = WorldEventID.FireElemental, Name = "Fire Elemental" },
                new EventName() { ID = WorldEventID.FrozenMaw, Name = "Frozen Maw" }
            };

            // Spanish
            List<EventName> spanish = new List<EventName>()
            {
                new EventName() { ID = WorldEventID.Megadestroyer, Name = "Megadestructor" },
                new EventName() { ID = WorldEventID.Tequatl, Name = "Tequatl" },
                new EventName() { ID = WorldEventID.KarkaQueen, Name = "La Reina Karka" },
                new EventName() { ID = WorldEventID.EvolvedJungleWurm, Name = "Tres Cabezas de la Serpiente" },
                new EventName() { ID = WorldEventID.Shatterer, Name = "El Asolador" },
                new EventName() { ID = WorldEventID.ClawOfJormag, Name = "La Garra de Jormag" },
                new EventName() { ID = WorldEventID.ModniirUlgoth, Name = "Ulgoth el Modniir" },
                new EventName() { ID = WorldEventID.InquestGolemMarkII, Name = "Gólem Serie II de la Inquisa" },
                new EventName() { ID = WorldEventID.TaidhaCovington, Name = "Taidha Covington" },
                new EventName() { ID = WorldEventID.JungleWurm, Name = "Gran Sierpe de la Selva" },
                new EventName() { ID = WorldEventID.ShadowBehemoth, Name = "Behemot de las Sombras" },
                new EventName() { ID = WorldEventID.FireElemental, Name = "Elemental de Fuego" },
                new EventName() { ID = WorldEventID.FrozenMaw, Name = "Jefe Chamán Svanir" }
            };

            // French
            List<EventName> french = new List<EventName>()
            {
                new EventName() { ID = WorldEventID.Megadestroyer, Name = "Mégadestructeur" },
                new EventName() { ID = WorldEventID.Tequatl, Name = "Tequatl" },
                new EventName() { ID = WorldEventID.KarkaQueen, Name = "La Reine Karka" },
                new EventName() { ID = WorldEventID.EvolvedJungleWurm, Name = "Trois têtes Guivre" },
                new EventName() { ID = WorldEventID.Shatterer, Name = "Le Destructeur" },
                new EventName() { ID = WorldEventID.ClawOfJormag, Name = "La Griffe de Jormag" },
                new EventName() { ID = WorldEventID.ModniirUlgoth, Name = "Ulgoth le Modniir" },
                new EventName() { ID = WorldEventID.InquestGolemMarkII, Name = "Golem Marque II de l'Enqueste" },
                new EventName() { ID = WorldEventID.TaidhaCovington, Name = "Taidha Covington" },
                new EventName() { ID = WorldEventID.JungleWurm, Name = "La Grande Guivre de la Jungle" },
                new EventName() { ID = WorldEventID.ShadowBehemoth, Name = "Béhémoth des Ombres" },
                new EventName() { ID = WorldEventID.FireElemental, Name = "L'élémentaire de Feu" },
                new EventName() { ID = WorldEventID.FrozenMaw, Name = "Chef Chamane de Svanir" }
            };

            // German
            List<EventName> german = new List<EventName>()
            {
                new EventName() { ID = WorldEventID.Megadestroyer, Name = "Megazerstörer" },
                new EventName() { ID = WorldEventID.Tequatl, Name = "Tequatl" },
                new EventName() { ID = WorldEventID.KarkaQueen, Name = "Die Karka-Königin" },
                new EventName() { ID = WorldEventID.EvolvedJungleWurm, Name = "Dreiköpfige Wurm" },
                new EventName() { ID = WorldEventID.Shatterer, Name = "Zerschmetterer" },
                new EventName() { ID = WorldEventID.ClawOfJormag, Name = "Die Klaue Jormags" },
                new EventName() { ID = WorldEventID.ModniirUlgoth, Name = "Ulgoth den Modniir" },
                new EventName() { ID = WorldEventID.InquestGolemMarkII, Name = "Inquestur-Golem Typ II" },
                new EventName() { ID = WorldEventID.TaidhaCovington, Name = "Taidha Covington" },
                new EventName() { ID = WorldEventID.JungleWurm, Name = "Großen Dschungelwurm" },
                new EventName() { ID = WorldEventID.ShadowBehemoth, Name = "Schatten-Behemoth" },
                new EventName() { ID = WorldEventID.FireElemental, Name = "Feuerelementar" },
                new EventName() { ID = WorldEventID.FrozenMaw, Name = "Schamanenoberhaupt der Svanir" }
            };

            Serializer.SerializeToXml(english, this.GetFilePath("en"));
            Serializer.SerializeToXml(spanish, this.GetFilePath("es"));
            Serializer.SerializeToXml(french, this.GetFilePath("fr"));
            Serializer.SerializeToXml(german, this.GetFilePath("de"));
        }

        /// <summary>
        /// Retrieves the full path of the stored names file using the given culture
        /// </summary>
        private string GetFilePath(string twoLetterIsoLangId)
        {
            string filename = Paths.LocalizationFolder + "EventNames";

            if (twoLetterIsoLangId != "en")
                filename += "." + twoLetterIsoLangId;

            filename += ".xml";

            return filename;
        }

        /// <summary>
        /// Container class for world event names
        /// </summary>
        public class EventName
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
        }
    }
}
