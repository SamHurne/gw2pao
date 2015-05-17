using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GW2PAO.API.Constants;
using GW2PAO.API.Util;
using NLog;

namespace GW2PAO.API.Providers
{
    public class DungeonNamesProvider : IStringProvider<Guid>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loaded dungeon names
        /// </summary>
        private List<DungeonName> dungeons;

        /// <summary>
        /// Loaded path names
        /// </summary>
        private List<PathName> paths = new List<PathName>();

        /// <summary>
        /// Locking object for accessing the dungeons list
        /// </summary>
        private readonly object dungeonsLock = new object();

        /// <summary>
        /// Default constructor
        /// </summary>
        public DungeonNamesProvider()
        {
            // By default, load the CurrentUICulture table of event names
            lock (this.dungeonsLock)
            {
                try
                {
                    this.dungeons = this.LoadNames(CultureInfo.CurrentUICulture);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex);
                }

                if (this.dungeons == null)
                {
                    this.GenerateFiles();
                    this.dungeons = this.LoadNames(CultureInfo.CurrentUICulture);
                }

                // Pull out/expand the paths for quick path access
                foreach (var dungeon in this.dungeons)
                {
                    foreach (var path in dungeon.PathNames)
                        this.paths.Add(path);
                }
            }
        }

        /// <summary>
        /// Changes the culture used for localization of strings
        /// </summary>
        /// <param name="culture">The culture to use for localization</param>
        public void SetCulture(CultureInfo culture)
        {
            var loadedDungeons = this.LoadNames(culture);
            if (loadedDungeons != null)
            {
                lock (this.dungeonsLock)
                {
                    this.dungeons = loadedDungeons;

                    // Pull out/expand the paths for quick path access
                    this.paths.Clear();
                    foreach (var dungeon in this.dungeons)
                    {
                        foreach (var path in dungeon.PathNames)
                            this.paths.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a string using the given identifier
        /// </summary>
        /// <param name="id">The ID of the dungeon or path</param>
        /// <returns>The localized name of the dungeon or path</returns>
        public string GetString(Guid id)
        {
            var result = string.Empty;
            lock (this.dungeonsLock)
            {
                var dungeonMatch = this.dungeons.FirstOrDefault(dun => dun.ID == id);

                if (dungeonMatch == null)
                {
                    var pathMatch = this.paths.FirstOrDefault(path => path.ID == id);
                    if (pathMatch != null)
                        result = pathMatch.Name;
                }
                else
                {
                    result = dungeonMatch.Name;
                }
            }
            return result;
        }

        /// <summary>
        /// Loads the collection of dungeon/path names from file
        /// </summary>
        /// <param name="culture">The culture to load</param>
        /// <returns>The loaded collection of dungeon/path names</returns>
        private List<DungeonName> LoadNames(CultureInfo culture)
        {
            var lang = culture.TwoLetterISOLanguageName;

            var supported = new[] { "en", "es", "fr", "de" };
            if (!supported.Contains(lang))
                lang = "en"; // Default to english if not supported

            var filename = this.GetFilePath(lang);
            return Serialization.DeserializeFromXml<List<DungeonName>>(filename);
        }

        /// <summary>
        /// Creates the world events names files
        /// </summary>
        /// <returns></returns>
        private void GenerateFiles()
        {
            List<DungeonName> english = new List<DungeonName>()
            {
                new DungeonName { ID = DungeonID.AscalonianCatacombs, Name = "Ascalonian Catacombs", PathNames = new List<PathName>()
                {
                    new PathName() { ID = AscalonianCatacombsPathID.Story, Name = "Story" },
                    new PathName() { ID = AscalonianCatacombsPathID.P1, Name = "Hodgins" },
                    new PathName() { ID = AscalonianCatacombsPathID.P2, Name = "Detha" },
                    new PathName() { ID = AscalonianCatacombsPathID.P3, Name = "Tzark" }
                }},
                new DungeonName { ID = DungeonID.CaudecusManor, Name = "Caudecus's Manor", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CaudecusManorPathID.Story, Name = "Story" },
                    new PathName() { ID = CaudecusManorPathID.P1, Name = "Asura" },
                    new PathName() { ID = CaudecusManorPathID.P2, Name = "Seraph" },
                    new PathName() { ID = CaudecusManorPathID.P3, Name = "Butler" }
                }},
                new DungeonName { ID = DungeonID.TwilightArbor, Name = "Twilight Arbor", PathNames = new List<PathName>()
                {
                    new PathName() { ID = TwilightArborPathID.Story, Name = "Story" },
                    new PathName() { ID = TwilightArborPathID.P1, Name = "Vevina (Forward)" },
                    new PathName() { ID = TwilightArborPathID.P2, Name = "Leurent (Up)" },
                    new PathName() { ID = TwilightArborPathID.P3, Name = "Aetherpath" }
                }},
                new DungeonName { ID = DungeonID.SorrowsEmbrace, Name = "Sorrow's Embrace", PathNames = new List<PathName>()
                {
                    new PathName() { ID = SorrowsEmbracePathID.Story, Name = "Story" },
                    new PathName() { ID = SorrowsEmbracePathID.P1, Name = "Fergg" },
                    new PathName() { ID = SorrowsEmbracePathID.P2, Name = "Rasolov" },
                    new PathName() { ID = SorrowsEmbracePathID.P3, Name = "Koptev" }
                }},
                new DungeonName { ID = DungeonID.CitadelOfFlame, Name = "Citadel of Flame", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CitadelOfFlamePathID.Story, Name = "Story" },
                    new PathName() { ID = CitadelOfFlamePathID.P1, Name = "Ferrah" },
                    new PathName() { ID = CitadelOfFlamePathID.P2, Name = "Magg" },
                    new PathName() { ID = CitadelOfFlamePathID.P3, Name = "Rhiannon" }
                }},
                new DungeonName { ID = DungeonID.HonorOfTheWaves, Name = "Honor of the Waves", PathNames = new List<PathName>()
                {
                    new PathName() { ID = HonorOfTheWavesPathID.Story, Name = "Story" },
                    new PathName() { ID = HonorOfTheWavesPathID.P1, Name = "Butcher" },
                    new PathName() { ID = HonorOfTheWavesPathID.P2, Name = "Plunderer" },
                    new PathName() { ID = HonorOfTheWavesPathID.P3, Name = "Zealot" }
                }},
                new DungeonName { ID = DungeonID.CrucibleOfEternity, Name = "Crucible of Eternity", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CrucibleOfEternityPathID.Story, Name = "Story" },
                    new PathName() { ID = CrucibleOfEternityPathID.P1, Name = "Submarine" },
                    new PathName() { ID = CrucibleOfEternityPathID.P2, Name = "Teleporter" },
                    new PathName() { ID = CrucibleOfEternityPathID.P3, Name = "Front Door" }
                }},
                new DungeonName { ID = DungeonID.RuinedCityOfArah, Name = "Ruined City of Arah", PathNames = new List<PathName>()
                {
                    new PathName() { ID = RuinedCityOfArahPathID.Story, Name = "Story" },
                    new PathName() { ID = RuinedCityOfArahPathID.P1, Name = "Jotun" },
                    new PathName() { ID = RuinedCityOfArahPathID.P2, Name = "Mursaat" },
                    new PathName() { ID = RuinedCityOfArahPathID.P3, Name = "Forgotten" },
                    new PathName() { ID = RuinedCityOfArahPathID.P4, Name = "Seer" }
                }},
                new DungeonName { ID = DungeonID.FractalsOfTheMists, Name = "Fractals of the Mists", PathNames = new List<PathName>()
                {
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier0, Name = "Tier 0 (1-10)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier1, Name = "Tier 1 (11-20)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier2, Name = "Tier 2 (21-30)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier3, Name = "Tier 3 (31-40)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier4, Name = "Tier 4 (41-50)" }
                }},
            };

            List<DungeonName> spanish = new List<DungeonName>()
            {
                new DungeonName { ID = DungeonID.AscalonianCatacombs, Name = "Catacumbas Ascalonianas", PathNames = new List<PathName>()
                {
                    new PathName() { ID = AscalonianCatacombsPathID.Story, Name = "Relato" },
                    new PathName() { ID = AscalonianCatacombsPathID.P1, Name = "Hodgins" },
                    new PathName() { ID = AscalonianCatacombsPathID.P2, Name = "Detha" },
                    new PathName() { ID = AscalonianCatacombsPathID.P3, Name = "Tzark" }
                }},
                new DungeonName { ID = DungeonID.CaudecusManor, Name = "Mansión de Caudecus", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CaudecusManorPathID.Story, Name = "Relato" },
                    new PathName() { ID = CaudecusManorPathID.P1, Name = "Asura" },
                    new PathName() { ID = CaudecusManorPathID.P2, Name = "Seraph" },
                    new PathName() { ID = CaudecusManorPathID.P3, Name = "Butler" }
                }},
                new DungeonName { ID = DungeonID.TwilightArbor, Name = "La Pérgola del Crepúsculo", PathNames = new List<PathName>()
                {
                    new PathName() { ID = TwilightArborPathID.Story, Name = "Relato" },
                    new PathName() { ID = TwilightArborPathID.P1, Name = "Vevina (Forward)" },
                    new PathName() { ID = TwilightArborPathID.P2, Name = "Leurent (Up)" },
                    new PathName() { ID = TwilightArborPathID.P3, Name = "Aetherpath" }
                }},
                new DungeonName { ID = DungeonID.SorrowsEmbrace, Name = "Abrazo del Pesar", PathNames = new List<PathName>()
                {
                    new PathName() { ID = SorrowsEmbracePathID.Story, Name = "Relato" },
                    new PathName() { ID = SorrowsEmbracePathID.P1, Name = "Fergg" },
                    new PathName() { ID = SorrowsEmbracePathID.P2, Name = "Rasolov" },
                    new PathName() { ID = SorrowsEmbracePathID.P3, Name = "Koptev" }
                }},
                new DungeonName { ID = DungeonID.CitadelOfFlame, Name = "Ciudadela de la Llama", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CitadelOfFlamePathID.Story, Name = "Relato" },
                    new PathName() { ID = CitadelOfFlamePathID.P1, Name = "Ferrah" },
                    new PathName() { ID = CitadelOfFlamePathID.P2, Name = "Magg" },
                    new PathName() { ID = CitadelOfFlamePathID.P3, Name = "Rhiannon" }
                }},
                new DungeonName { ID = DungeonID.HonorOfTheWaves, Name = "El Honor de las Olas", PathNames = new List<PathName>()
                {
                    new PathName() { ID = HonorOfTheWavesPathID.Story, Name = "Relato" },
                    new PathName() { ID = HonorOfTheWavesPathID.P1, Name = "Butcher" },
                    new PathName() { ID = HonorOfTheWavesPathID.P2, Name = "Plunderer" },
                    new PathName() { ID = HonorOfTheWavesPathID.P3, Name = "Zealot" }
                }},
                new DungeonName { ID = DungeonID.CrucibleOfEternity, Name = "Crisol de la Eternidad", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CrucibleOfEternityPathID.Story, Name = "Relato" },
                    new PathName() { ID = CrucibleOfEternityPathID.P1, Name = "Submarine" },
                    new PathName() { ID = CrucibleOfEternityPathID.P2, Name = "Teleporter" },
                    new PathName() { ID = CrucibleOfEternityPathID.P3, Name = "Front Door" }
                }},
                new DungeonName { ID = DungeonID.RuinedCityOfArah, Name = "La ciudad en ruinas de Arah", PathNames = new List<PathName>()
                {
                    new PathName() { ID = RuinedCityOfArahPathID.Story, Name = "Relato" },
                    new PathName() { ID = RuinedCityOfArahPathID.P1, Name = "Jotun" },
                    new PathName() { ID = RuinedCityOfArahPathID.P2, Name = "Mursaat" },
                    new PathName() { ID = RuinedCityOfArahPathID.P3, Name = "Forgotten" },
                    new PathName() { ID = RuinedCityOfArahPathID.P4, Name = "Seer" }
                }},
                new DungeonName { ID = DungeonID.FractalsOfTheMists, Name = "Fractales de la Niebla", PathNames = new List<PathName>()
                {
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier0, Name = "Tier 0 (1-10)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier1, Name = "Tier 1 (11-20)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier2, Name = "Tier 2 (21-30)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier3, Name = "Tier 3 (31-40)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier4, Name = "Tier 4 (41-50)" }
                }},
            };

            List<DungeonName> french = new List<DungeonName>()
            {
                new DungeonName { ID = DungeonID.AscalonianCatacombs, Name = "Catacombes d'Ascalon", PathNames = new List<PathName>()
                {
                    new PathName() { ID = AscalonianCatacombsPathID.Story, Name = "Histoire" },
                    new PathName() { ID = AscalonianCatacombsPathID.P1, Name = "Hodgins" },
                    new PathName() { ID = AscalonianCatacombsPathID.P2, Name = "Detha" },
                    new PathName() { ID = AscalonianCatacombsPathID.P3, Name = "Tzark" }
                }},
                new DungeonName { ID = DungeonID.CaudecusManor, Name = "Manoir de Caudecus", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CaudecusManorPathID.Story, Name = "Histoire" },
                    new PathName() { ID = CaudecusManorPathID.P1, Name = "Asura" },
                    new PathName() { ID = CaudecusManorPathID.P2, Name = "Seraph" },
                    new PathName() { ID = CaudecusManorPathID.P3, Name = "Butler" }
                }},
                new DungeonName { ID = DungeonID.TwilightArbor, Name = "Tonnelle du crépuscule", PathNames = new List<PathName>()
                {
                    new PathName() { ID = TwilightArborPathID.Story, Name = "Histoire" },
                    new PathName() { ID = TwilightArborPathID.P1, Name = "Vevina (Forward)" },
                    new PathName() { ID = TwilightArborPathID.P2, Name = "Leurent (Up)" },
                    new PathName() { ID = TwilightArborPathID.P3, Name = "Aetherpath" }
                }},
                new DungeonName { ID = DungeonID.SorrowsEmbrace, Name = "Etreinte des Lamentations", PathNames = new List<PathName>()
                {
                    new PathName() { ID = SorrowsEmbracePathID.Story, Name = "Histoire" },
                    new PathName() { ID = SorrowsEmbracePathID.P1, Name = "Fergg" },
                    new PathName() { ID = SorrowsEmbracePathID.P2, Name = "Rasolov" },
                    new PathName() { ID = SorrowsEmbracePathID.P3, Name = "Koptev" }
                }},
                new DungeonName { ID = DungeonID.CitadelOfFlame, Name = "Citadelle de la Flamme", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CitadelOfFlamePathID.Story, Name = "Histoire" },
                    new PathName() { ID = CitadelOfFlamePathID.P1, Name = "Ferrah" },
                    new PathName() { ID = CitadelOfFlamePathID.P2, Name = "Magg" },
                    new PathName() { ID = CitadelOfFlamePathID.P3, Name = "Rhiannon" }
                }},
                new DungeonName { ID = DungeonID.HonorOfTheWaves, Name = "Honneur des vagues", PathNames = new List<PathName>()
                {
                    new PathName() { ID = HonorOfTheWavesPathID.Story, Name = "Histoire" },
                    new PathName() { ID = HonorOfTheWavesPathID.P1, Name = "Butcher" },
                    new PathName() { ID = HonorOfTheWavesPathID.P2, Name = "Plunderer" },
                    new PathName() { ID = HonorOfTheWavesPathID.P3, Name = "Zealot" }
                }},
                new DungeonName { ID = DungeonID.CrucibleOfEternity, Name = "Creuset de l'éternité", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CrucibleOfEternityPathID.Story, Name = "Histoire" },
                    new PathName() { ID = CrucibleOfEternityPathID.P1, Name = "Submarine" },
                    new PathName() { ID = CrucibleOfEternityPathID.P2, Name = "Teleporter" },
                    new PathName() { ID = CrucibleOfEternityPathID.P3, Name = "Front Door" }
                }},
                new DungeonName { ID = DungeonID.RuinedCityOfArah, Name = "La cité en ruine d'Arah", PathNames = new List<PathName>()
                {
                    new PathName() { ID = RuinedCityOfArahPathID.Story, Name = "Histoire" },
                    new PathName() { ID = RuinedCityOfArahPathID.P1, Name = "Jotun" },
                    new PathName() { ID = RuinedCityOfArahPathID.P2, Name = "Mursaat" },
                    new PathName() { ID = RuinedCityOfArahPathID.P3, Name = "Forgotten" },
                    new PathName() { ID = RuinedCityOfArahPathID.P4, Name = "Seer" }
                }},
                new DungeonName { ID = DungeonID.FractalsOfTheMists, Name = "Fractales des Brumes", PathNames = new List<PathName>()
                {
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier0, Name = "Tier 0 (1-10)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier1, Name = "Tier 1 (11-20)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier2, Name = "Tier 2 (21-30)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier3, Name = "Tier 3 (31-40)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier4, Name = "Tier 4 (41-50)" }
                }},
            };

            List<DungeonName> german = new List<DungeonName>()
            {
                new DungeonName { ID = DungeonID.AscalonianCatacombs, Name = "Katakomben von Ascalon", PathNames = new List<PathName>()
                {
                    new PathName() { ID = AscalonianCatacombsPathID.Story, Name = "Geschichte" },
                    new PathName() { ID = AscalonianCatacombsPathID.P1, Name = "Hodgins" },
                    new PathName() { ID = AscalonianCatacombsPathID.P2, Name = "Detha" },
                    new PathName() { ID = AscalonianCatacombsPathID.P3, Name = "Tzark" }
                }},
                new DungeonName { ID = DungeonID.CaudecusManor, Name = "Caudecus' Anwesen", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CaudecusManorPathID.Story, Name = "Geschichte" },
                    new PathName() { ID = CaudecusManorPathID.P1, Name = "Asura" },
                    new PathName() { ID = CaudecusManorPathID.P2, Name = "Seraph" },
                    new PathName() { ID = CaudecusManorPathID.P3, Name = "Butler" }
                }},
                new DungeonName { ID = DungeonID.TwilightArbor, Name = "Zwielichtgarten", PathNames = new List<PathName>()
                {
                    new PathName() { ID = TwilightArborPathID.Story, Name = "Geschichte" },
                    new PathName() { ID = TwilightArborPathID.P1, Name = "Vevina (Forward)" },
                    new PathName() { ID = TwilightArborPathID.P2, Name = "Leurent (Up)" },
                    new PathName() { ID = TwilightArborPathID.P3, Name = "Aetherpath" }
                }},
                new DungeonName { ID = DungeonID.SorrowsEmbrace, Name = "Umarmung der Betrübnis", PathNames = new List<PathName>()
                {
                    new PathName() { ID = SorrowsEmbracePathID.Story, Name = "Geschichte" },
                    new PathName() { ID = SorrowsEmbracePathID.P1, Name = "Fergg" },
                    new PathName() { ID = SorrowsEmbracePathID.P2, Name = "Rasolov" },
                    new PathName() { ID = SorrowsEmbracePathID.P3, Name = "Koptev" }
                }},
                new DungeonName { ID = DungeonID.CitadelOfFlame, Name = "Flammenzitadelle", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CitadelOfFlamePathID.Story, Name = "Geschichte" },
                    new PathName() { ID = CitadelOfFlamePathID.P1, Name = "Ferrah" },
                    new PathName() { ID = CitadelOfFlamePathID.P2, Name = "Magg" },
                    new PathName() { ID = CitadelOfFlamePathID.P3, Name = "Rhiannon" }
                }},
                new DungeonName { ID = DungeonID.HonorOfTheWaves, Name = "Zierde der Wogen", PathNames = new List<PathName>()
                {
                    new PathName() { ID = HonorOfTheWavesPathID.Story, Name = "Geschichte" },
                    new PathName() { ID = HonorOfTheWavesPathID.P1, Name = "Butcher" },
                    new PathName() { ID = HonorOfTheWavesPathID.P2, Name = "Plunderer" },
                    new PathName() { ID = HonorOfTheWavesPathID.P3, Name = "Zealot" }
                }},
                new DungeonName { ID = DungeonID.CrucibleOfEternity, Name = "Schmelztiegel der Ewigkeit", PathNames = new List<PathName>()
                {
                    new PathName() { ID = CrucibleOfEternityPathID.Story, Name = "Geschichte" },
                    new PathName() { ID = CrucibleOfEternityPathID.P1, Name = "Submarine" },
                    new PathName() { ID = CrucibleOfEternityPathID.P2, Name = "Teleporter" },
                    new PathName() { ID = CrucibleOfEternityPathID.P3, Name = "Front Door" }
                }},
                new DungeonName { ID = DungeonID.RuinedCityOfArah, Name = "Die Ruinenstadt Arah", PathNames = new List<PathName>()
                {
                    new PathName() { ID = RuinedCityOfArahPathID.Story, Name = "Geschichte" },
                    new PathName() { ID = RuinedCityOfArahPathID.P1, Name = "Jotun" },
                    new PathName() { ID = RuinedCityOfArahPathID.P2, Name = "Mursaat" },
                    new PathName() { ID = RuinedCityOfArahPathID.P3, Name = "Forgotten" },
                    new PathName() { ID = RuinedCityOfArahPathID.P4, Name = "Seer" }
                }},
                new DungeonName { ID = DungeonID.FractalsOfTheMists, Name = "Fraktale der Nebel", PathNames = new List<PathName>()
                {
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier0, Name = "Tier 0 (1-10)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier1, Name = "Tier 1 (11-20)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier2, Name = "Tier 2 (21-30)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier3, Name = "Tier 3 (31-40)" },
                    new PathName() { ID = FractalsOfTheMistsPathID.Tier4, Name = "Tier 4 (41-50)" }
                }},
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
            return string.Format("{0}\\{1}\\{2}", Paths.LocalizationFolder, twoLetterIsoLangId, "DungeonNames.xml");
        }

        /// <summary>
        /// Container class for dungeon names
        /// </summary>
        public class DungeonName
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public List<PathName> PathNames { get; set; }

            public DungeonName()
            {
                this.PathNames = new List<PathName>();
            }
        }

        /// <summary>
        /// Container class for path names
        /// </summary>
        public class PathName
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
        }
    }
}
