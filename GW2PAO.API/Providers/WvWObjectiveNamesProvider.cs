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
    public class WvWObjectiveNamesProvider : IStringProvider<int, WvWObjectiveNameEnum>
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
        public string GetString(int id, WvWObjectiveNameEnum selector)
        {
            var result = string.Empty;
            lock (this.objectivesLock)
            {
                var match = this.objectives.FirstOrDefault(obj => obj.ID == id);
                if (match != null)
                {
                    switch (selector)
                    {
                        case WvWObjectiveNameEnum.Full:
                            result = match.Full;
                            break;
                        case WvWObjectiveNameEnum.Short:
                            result = match.Short;
                            break;
                        case WvWObjectiveNameEnum.Cardinal:
                            result = match.Cardinal;
                            break;
                        default:
                            break;
                    }
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
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Keep_Overlook, Cardinal = "N", Full = "Overlook", Short = "Overlook" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Keep_Valley, Cardinal = "SE", Full = "Valley", Short = "Valley" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Keep_Lowlands, Cardinal = "SW", Full = "Lowlands", Short = "Lowlands" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Golanta, Cardinal = "SSW", Full = "Golanta Clearing", Short = "Golanta" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Pangloss, Cardinal = "NNE", Full = "Pangloss Rise", Short = "Pangloss" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Speldan, Cardinal = "NNW", Full = "Speldan Clearcut", Short = "Speldan" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Danelon, Cardinal = "SSE", Full = "Danelon Passage", Short = "Danelon" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Umberglade, Cardinal = "E", Full = "Umberglade Woods", Short = "Umberglade" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Castle_Stonemist, Cardinal = "C", Full = "Stonemist Castle", Short = "Stonemist" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Rogues, Cardinal = "W", Full = "Rogue's Quarry", Short = "Rogue's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Aldons, Cardinal = "W", Full = "Aldon's Ledge", Short = "Aldon's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Wildcreek, Cardinal = "W", Full = "Wildcreek Run", Short = "Wildcreek" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Jerrifers, Cardinal = "W", Full = "Jerrifer's Slough", Short = "Jerrifer's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Klovan, Cardinal = "SW", Full = "Klovan Gully", Short = "Klovan" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Langor, Cardinal = "Sw", Full = "Langor Gulch", Short = "Langor" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Quentin, Cardinal = "SE", Full = "Quentin Lake", Short = "Quentin" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Mendons, Cardinal = "NW", Full = "Mendon's Gap", Short = "Mendon's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Anzalias, Cardinal = "NW", Full = "Anzalias Pass", Short = "Anzalias" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Ogrewatch, Cardinal = "NE", Full = "Ogrewatch Cut", Short = "Ogrewatch" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Veloka, Cardinal = "NW", Full = "Veloka Slope", Short = "Veloka" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Durios, Cardinal = "E", Full = "Durios Gulch", Short = "Durios" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Bravost, Cardinal = "E", Full = "Bravost Escarpment", Short = "Bravost" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Keep_Garrison, Cardinal = "C", Full = "Garrison", Short = "Garrison" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Orchard, Cardinal = "S", Full = "Champion's Demense", Short = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Redbriar, Cardinal = "SW", Full = "Redbriar", Short = "Redbriar" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Greenlake, Cardinal = "SE", Full = "Greenlake", Short = "Greenlake" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Keep_Bay, Cardinal = "W", Full = "Ascension Bay", Short = "Bay" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Dawns, Cardinal = "NE", Full = "Dawn's Eyrie", Short = "Dawn's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Spiritholme, Cardinal = "N", Full = "The Spiritholme", Short = "Spiritholme" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Woodhaven, Cardinal = "NW", Full = "Woodhaven", Short = "Woodhaven" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Keep_Hills, Cardinal = "E", Full = "Askalion Hills", Short = "Hills" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Keep_Hills, Cardinal = "E", Full = "Etheron Hills", Short = "Hills" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Keep_Bay, Cardinal = "W", Full = "Dreaming Bay", Short = "Bay" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Orchard, Cardinal = "S", Full = "Victors's Lodge", Short = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Greenbriar, Cardinal = "SW", Full = "Greenbriar", Short = "Greenbriar" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Bluelake, Cardinal = "SE", Full = "Bluelake", Short = "Bluelake" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Keep_Garrison, Cardinal = "C", Full = "Garrison", Short = "Garrison" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Longview, Cardinal = "NW", Full = "Longview", Short = "Longview" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Godsword, Cardinal = "N", Full = "The Godsword", Short = "Godsword" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Cliffside, Cardinal = "NE", Full = "Cliffside", Short = "Cliffside" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Keep_Hills, Cardinal = "E", Full = "Shadaran Hills", Short = "Hills" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Redlake, Cardinal = "SE", Full = "Redlake", Short = "Redlake" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Orchard, Cardinal = "S", Full = "Hero's Lodge", Short = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Keep_Bay, Cardinal = "W", Full = "Dreadfall Bay", Short = "Bay" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Bluebriar, Cardinal = "SW", Full = "Bluebriar", Short = "Bluebriar" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Keep_Garrison, Cardinal = "C", Full = "Garrison", Short = "Garrison" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Sunnyhill, Cardinal = "NW", Full = "Sunnyhill", Short = "Sunnyhill" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Faithleap, Cardinal = "NW", Full = "Faithleap", Short = "Faithleap" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Bluevale, Cardinal = "SW", Full = "Bluevale Refuge", Short = "Bluevale" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Bluewater, Cardinal = "SE", Full = "Bluewater Lowlands", Short = "Bluewater" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Astralholme, Cardinal = "NE", Full = "Astralholme", Short = "Astralholme" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Arahs, Cardinal = "NW", Full = "Arah's Hope", Short = "Arah's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Greenvale, Cardinal = "SW", Full = "Greenvale Refuge", Short = "Greenvale" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Foghaven, Cardinal = "NE", Full = "Foghaven", Short = "Foghaven" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Redwater, Cardinal = "SE", Full = "Redwater Lowlands", Short = "Redwater" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Titanpaw, Cardinal = "N", Full = "The Titanpaw", Short = "Titanpaw" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Cragtop, Cardinal = "NE", Full = "Cragtop", Short = "Cragtop" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Godslore, Cardinal = "NW", Full = "Godslore", Short = "Godslore" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Redvale, Cardinal = "SW", Full = "Redvale Refuge", Short = "Redvale" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Stargrove, Cardinal = "NE", Full = "Stargrove", Short = "Stargrove" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Greenwater, Cardinal = "SE", Full = "Greenwater Lowlands", Short = "Greenwater" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Temple, Cardinal = "", Full = "Temple of Lost Prayers", Short = "Temple" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Hollow, Cardinal = "", Full = "Battle's Hollow", Short = "Hollow" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Estate, Cardinal = "", Full = "Bauer's Estate", Short = "Estate" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Orchard, Cardinal = "", Full = "Orchard Overlook", Short = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Carvers, Cardinal = "", Full = "Carver's Ascent", Short = "Carver's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Carvers, Cardinal = "", Full = "Carver's Ascent", Short = "Carver's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Orchard, Cardinal = "", Full = "Orchard Overlook", Short = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Estate, Cardinal = "", Full = "Bauer's Estate", Short = "Estate" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Hollow, Cardinal = "", Full = "Battle's Hollow", Short = "Hollow" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Temple, Cardinal = "", Full = "Temple of Lost Prayers", Short = "Temple" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Carvers, Cardinal = "", Full = "Carver's Ascent", Short = "Carver's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Orchard, Cardinal = "", Full = "Orchard Overlook", Short = "Orchard" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Estate, Cardinal = "", Full = "Bauer's Estate", Short = "Estate" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Hollow, Cardinal = "", Full = "Battle's Hollow", Short = "Hollow" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Temple, Cardinal = "", Full = "Temple of Lost Prayers", Short = "Temple" }
            };

            // Spanish TODO
            List<ObjectiveNames> spanish = new List<ObjectiveNames>();

            // French
            List<ObjectiveNames> french = new List<ObjectiveNames>()
            {
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Keep_Overlook, Cardinal = "N", Full = "Belvédère", Short = "Belvédère" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Keep_Valley, Cardinal = "SE", Full = "Vallée", Short = "Vallée" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Keep_Lowlands, Cardinal = "SO", Full = "Basses Terres", Short = "Basses Terres" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Golanta, Cardinal = "SSO", Full = "Clairière de Golanta", Short = "Golanta" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Pangloss, Cardinal = "NNE", Full = "Mine de Pangloss", Short = "Pangloss" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Speldan, Cardinal = "NNO", Full = "Forêt de Speldan", Short = "Speldan" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Danelon, Cardinal = "SSE", Full = "Passage de Danelon", Short = "Danelon" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Umberglade, Cardinal = "E", Full = "Bois d'Ombreclair", Short = "Ombreclair" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Castle_Stonemist, Cardinal = "C", Full = "Chateau Brumepierre", Short = "Brumepierre" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Camp_Rogues, Cardinal = "O", Full = "Carrière du Voleur", Short = "Voleur" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Aldons, Cardinal = "O", Full = "Corniche d'Aldon", Short = "Aldon" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Wildcreek, Cardinal = "O", Full = "Piste du Ruisseau sauvage", Short = "Ruisseau" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Jerrifers, Cardinal = "SO", Full = "Bourbier de Jerrifer", Short = "Jerrifer" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Klovan, Cardinal = "SO", Full = "Ravin de Klovan", Short = "Klovan" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Langor, Cardinal = "SE", Full = "Ravin de Langor", Short = "Langor" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Quentin, Cardinal = "SE", Full = "Lac Quentin", Short = "Quentin" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Mendons, Cardinal = "NW", Full = "Faille de Mendon", Short = "Mendon" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Anzalias, Cardinal = "NW", Full = "Col d'Anzalias", Short = "Anzalias" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Ogrewatch, Cardinal = "NE", Full = "Percée de Gardogre", Short = "Gardogre" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Veloka, Cardinal = "NE", Full = "Flanc de Veloka", Short = "Veloka" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Durios, Cardinal = "E", Full = "Ravin de Durios", Short = "Durios" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.EB_Tower_Bravost, Cardinal = "E", Full = "Falaise de Bravost", Short = "Bravost" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Keep_Garrison, Cardinal = "C", Full = "Garnison", Short = "Garnison" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Orchard, Cardinal = "S", Full = "Fief du Champion", Short = "Verger" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Redbriar, Cardinal = "SO", Full = "Bruyerouge", Short = "Bruyerouge" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Greenlake, Cardinal = "SE", Full = "Lac Vert", Short = "Lac Vert" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Keep_Bay, Cardinal = "O", Full = "Baie de l'Ascension", Short = "Baie" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Dawns, Cardinal = "NE", Full = "Repaire de l'AUbe", Short = "Aube" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Spiritholme, Cardinal = "N", Full = "Le Heaume Spirituel", Short = "Heaume" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Tower_Woodhaven, Cardinal = "NO", Full = "Boisrefuge", Short = "Boisrefuge" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Keep_Hills, Cardinal = "E", Full = "Collines d'Askalion", Short = "Askalion" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Keep_Hills, Cardinal = "E", Full = "Collines d'Etheron", Short = "Etheron" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Keep_Bay, Cardinal = "O", Full = "Baie des Rêves", Short = "Baie" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Orchard, Cardinal = "S", Full = "Pavillon du Vainqueur", Short = "Verger" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Greenbriar, Cardinal = "SO", Full = "Vert-Bruyère", Short = "Vert-Bruyère" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Bluelake, Cardinal = "SE", Full = "Lac Bleu", Short = "Lac Bleu" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Keep_Garrison, Cardinal = "C", Full = "Garnison", Short = "Garnison" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Longview, Cardinal = "NW", Full = "Longuevue", Short = "Longuevue" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Godsword, Cardinal = "N", Full = "Epée Divine", Short = "Epée" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Tower_Cliffside, Cardinal = "NE", Full = "Flanc de Falaise", Short = "Falaise" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Keep_Hills, Cardinal = "E", Full = "Collines Shadaran", Short = "Shadaran" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Redlake, Cardinal = "SE", Full = "Lac Rouge", Short = "Lac Rouge" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Orchard, Cardinal = "S", Full = "Pavillon du Héros", Short = "Verger" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Keep_Bay, Cardinal = "O", Full = "Baie du Déclin Noir", Short = "Baie" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Bluebriar, Cardinal = "SO", Full = "Bruyazur", Short = "Bruyazur" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Keep_Garrison, Cardinal = "C", Full = "Garnison", Short = "Garnison" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Sunnyhill, Cardinal = "NO", Full = "Colline ensoleillée", Short = "Colline" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Faithleap, Cardinal = "NO", Full = "Saut de la Foi", Short = "Saut de la Foi" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Bluevale, Cardinal = "SO", Full = "Refuge de Bleuval", Short = "Bleuval" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Bluewater, Cardinal = "SE", Full = "Basses terres d'Eau-Azur", Short = "Basses terres" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Astralholme, Cardinal = "NE", Full = "Heaume Astral", Short = "Astral" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Arahs, Cardinal = "NO", Full = "Espoir d'Arah", Short = "Arah" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Camp_Greenvale, Cardinal = "SO", Full = "Refuge de Valvert", Short = "Valvert" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Foghaven, Cardinal = "NE", Full = "Havre Gris", Short = "Havre" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Redwater, Cardinal = "SE", Full = "Basses terres de Rubicon", Short = "Basses terres" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Camp_Titanpaw, Cardinal = "N", Full = "Bras du Titan", Short = "Titan" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Tower_Cragtop, Cardinal = "NE", Full = "Sommet de HautCrag", Short = "Hautcrag" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Godslore, Cardinal = "NO", Full = "Savoir Divin", Short = "Divi" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Redvale, Cardinal = "SO", Full = "Refuge de Valrouge", Short = "Valrouge" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Stargrove, Cardinal = "NE", Full = "Bosquet Etoilé", Short = "Bosquet" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Camp_Greenwater, Cardinal = "SE", Full = "Basses terres d'Eau-Verdoyante", Short = "Basses terres" },

                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Temple, Cardinal = "", Full = "Temple des Prières Perdues", Short = "Temple" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Hollow, Cardinal = "", Full = "Vallon de bataille", Short = "Vallon" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Estate, Cardinal = "", Full = "Domaine de Bauer", Short = "Bauer" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Orchard, Cardinal = "", Full = "Belvédère du Berger", Short = "Verger" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.RB_Carvers, Cardinal = "", Full = "Côte du Couteau", Short = "Carver's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Carvers, Cardinal = "", Full = "Côte du Couteau", Short = "Carver's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Orchard, Cardinal = "", Full = "Belvédère du Berger", Short = "Verger" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Estate, Cardinal = "", Full = "Domaine de Bauer", Short = "Bauer" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Hollow, Cardinal = "", Full = "Vallon de bataille", Short = "Vallon" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.BB_Temple, Cardinal = "", Full = "Temple des Prières Perdues", Short = "Temple" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Carvers, Cardinal = "", Full = "Côte du Couteau", Short = "Carver's" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Orchard, Cardinal = "", Full = "Belvédère du Berger", Short = "Verger" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Estate, Cardinal = "", Full = "Domaine de Bauer", Short = "Bauer" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Hollow, Cardinal = "", Full = "Vallon de bataille", Short = "Vallon" },
                new ObjectiveNames() { ID = (int)WvWObjectiveID.GB_Temple, Cardinal = "", Full = "Temple des Prières Perdues", Short = "Temple" }
            };

            // German TODO
            List<ObjectiveNames> german = new List<ObjectiveNames>();

            Serializer.SerializeToXml(english, this.GetFilePath("en"));
            Serializer.SerializeToXml(english, this.GetFilePath("es"));
            Serializer.SerializeToXml(french, this.GetFilePath("fr"));
            Serializer.SerializeToXml(english, this.GetFilePath("de"));
        }

        /// <summary>
        /// Retrieves the full path of the stored names file using the given culture
        /// </summary>
        private string GetFilePath(string twoLetterIsoLangId)
        {
            return string.Format("{0}\\{1}\\{2}", Paths.LocalizationFolder, twoLetterIsoLangId, "WvWObjectiveNames.xml");
        }

        /// <summary>
        /// Container class for objective names
        /// </summary>
        public class ObjectiveNames
        {
            public int ID { get; set; }
            public string Full { get; set; }
            public string Short { get; set; }
            public string Cardinal { get; set; }
        }
    }

    public enum WvWObjectiveNameEnum
    {
        Full,
        Short,
        Cardinal
    }
}
