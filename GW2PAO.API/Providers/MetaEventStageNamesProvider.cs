using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Constants;
using GW2PAO.API.Util;
using NLog;

namespace GW2PAO.API.Providers
{
    public class MetaEventStageNamesProvider : IStringProvider<Guid>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loaded world event names
        /// </summary>
        private List<MetaEventStageName> metaEventStageNames;

        /// <summary>
        /// Locking object for accessing the loadedNames list
        /// </summary>
        private readonly object worldEventsLock = new object();

        /// <summary>
        /// Default constructor
        /// </summary>
        public MetaEventStageNamesProvider()
        {
            // By default, load the CurrentUICulture table of event names
            lock (this.worldEventsLock)
            {
                try
                {
                    this.metaEventStageNames = this.LoadNames(CultureInfo.CurrentUICulture);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex);
                }

                if (this.metaEventStageNames == null)
                {
                    this.GenerateFiles();
                    this.metaEventStageNames = this.LoadNames(CultureInfo.CurrentUICulture);
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
                    this.metaEventStageNames = loadedNames;
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
                var match = this.metaEventStageNames.FirstOrDefault(evt => evt.ID == id);
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
        private List<MetaEventStageName> LoadNames(CultureInfo culture)
        {
            var lang = culture.TwoLetterISOLanguageName;

            var supported = new[] { "en", "es", "fr", "de" };
            if (!supported.Contains(lang))
                lang = "en"; // Default to english if not supported

            var filename = this.GetFilePath(lang);
            return Serialization.DeserializeFromXml<List<MetaEventStageName>>(filename);
        }

        /// <summary>
        /// Creates the world events names files
        /// </summary>
        /// <returns></returns>
        private void GenerateFiles()
        {
            // English
            List<MetaEventStageName> english = new List<MetaEventStageName>()
            {
                new MetaEventStageName() { ID = MetaEventStageID.DryTop_CrashSite,           Name = "Crash Site" },
                new MetaEventStageName() { ID = MetaEventStageID.DryTop_Sandstorm,           Name = "Sandstorm" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_NightBosses,   Name = "Night Bosses" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_Daytime,       Name = "Daytime" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_Night,         Name = "Night" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Challenges,      Name = "Challenges" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Octovine,        Name = "Octovine" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Reset,           Name = "Reset" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Pylons,          Name = "Pylons" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_Preparation,  Name = "Preparation" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_ChakGerent,   Name = "Chak Gerent" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_HelpOutposts, Name = "Help Outposts" },
                new MetaEventStageName() { ID = MetaEventStageID.DragonsStand_MapActive,     Name = "Map Active" }
            };

            // Spanish
            List<MetaEventStageName> spanish = new List<MetaEventStageName>()
            {
                new MetaEventStageName() { ID = MetaEventStageID.DryTop_CrashSite,           Name = "Lugar del accidente" },
                new MetaEventStageName() { ID = MetaEventStageID.DryTop_Sandstorm,           Name = "Tormenta de arena" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_NightBosses,   Name = "Los jefes noche" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_Daytime,       Name = "Tiempo de día" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_Night,         Name = "Noche" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Challenges,      Name = "desafíos" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Octovine,        Name = "Octovine" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Reset,           Name = "Reiniciar" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Pylons,          Name = "pilones" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_Preparation,  Name = "Preparación" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_ChakGerent,   Name = "Chak Gerent" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_HelpOutposts, Name = "Ayuda Outposts" },
                new MetaEventStageName() { ID = MetaEventStageID.DragonsStand_MapActive,     Name = "mapa activo" }
            };

            // French
            List<MetaEventStageName> french = new List<MetaEventStageName>()
            {
                new MetaEventStageName() { ID = MetaEventStageID.DryTop_CrashSite,           Name = "Site du crash" },
                new MetaEventStageName() { ID = MetaEventStageID.DryTop_Sandstorm,           Name = "Tempête de Sable" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_NightBosses,   Name = "Boss de nuit" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_Daytime,       Name = "Jour" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_Night,         Name = "Nuit" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Challenges,      Name = "Défis" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Octovine,        Name = "Octoliane" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Reset,           Name = "Reset" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Pylons,          Name = "Pylones" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_Preparation,  Name = "Préparation" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_ChakGerent,   Name = "Régent Chak" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_HelpOutposts, Name = "Aider les campements" },
                new MetaEventStageName() { ID = MetaEventStageID.DragonsStand_MapActive,     Name = "Carte active" }
            };

            // German
            List<MetaEventStageName> german = new List<MetaEventStageName>()
            {
                new MetaEventStageName() { ID = MetaEventStageID.DryTop_CrashSite,           Name = "Crash Site" },
                new MetaEventStageName() { ID = MetaEventStageID.DryTop_Sandstorm,           Name = "Sandsturm" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_NightBosses,   Name = "Nacht Bosses" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_Daytime,       Name = "Tageszeit" },
                new MetaEventStageName() { ID = MetaEventStageID.VerdantBrink_Night,         Name = "Nacht" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Challenges,      Name = "Herausforderungen" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Octovine,        Name = "Octovine" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Reset,           Name = "zurückstellen" },
                new MetaEventStageName() { ID = MetaEventStageID.AuricBasin_Pylons,          Name = "pylons" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_Preparation,  Name = "Vorbereitung" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_ChakGerent,   Name = "Chak Gerent" },
                new MetaEventStageName() { ID = MetaEventStageID.TangledDepths_HelpOutposts, Name = "Hilfe Outposts" },
                new MetaEventStageName() { ID = MetaEventStageID.DragonsStand_MapActive,     Name = "Karte Aktiv" }
            };

            Serialization.SerializeToXml(english, this.GetFilePath("en"));
            Serialization.SerializeToXml(spanish, this.GetFilePath("es"));
            Serialization.SerializeToXml(french, this.GetFilePath("fr"));
            Serialization.SerializeToXml(german, this.GetFilePath("de"));
        }

        /// <summary>
        /// Retrieves the full path of the stored names file using the given culture
        /// </summary>
        private string GetFilePath(string twoLetterIsoLangId)
        {
            return string.Format("{0}\\{1}\\{2}", Paths.LocalizationFolder, twoLetterIsoLangId, "EventNames.xml");
        }

        /// <summary>
        /// Container class for meta event stage names
        /// </summary>
        public class MetaEventStageName
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
        }
    }
}
