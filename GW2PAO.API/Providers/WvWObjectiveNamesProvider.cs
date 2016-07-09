using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GW2PAO.API.Constants;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Util;
using NLog;

namespace GW2PAO.API.Providers
{
    public class WvWObjectiveNamesProvider : IStringProvider<WvWObjectiveId, WvWObjectiveNameEnum>
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
        public string GetString(WvWObjectiveId id, WvWObjectiveNameEnum selector)
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
            return Serialization.DeserializeFromXml<List<ObjectiveNames>>(filename);
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
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Overlook, Cardinal = "N", Full = "Overlook", Short = "Overlook" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Valley, Cardinal = "SE", Full = "Valley", Short = "Valley" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Lowlands, Cardinal = "SW", Full = "Lowlands", Short = "Lowlands" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Golanta, Cardinal = "SSW", Full = "Golanta Clearing", Short = "Golanta" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Pangloss, Cardinal = "NNE", Full = "Pangloss Rise", Short = "Pangloss" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Speldan, Cardinal = "NNW", Full = "Speldan Clearcut", Short = "Speldan" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Danelon, Cardinal = "SSE", Full = "Danelon Passage", Short = "Danelon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Umberglade, Cardinal = "E", Full = "Umberglade Woods", Short = "Umberglade" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Castle_Stonemist, Cardinal = "C", Full = "Stonemist Castle", Short = "Stonemist" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Rogues, Cardinal = "W", Full = "Rogue's Quarry", Short = "Rogue" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Aldons, Cardinal = "W", Full = "Aldon's Ledge", Short = "Aldon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Wildcreek, Cardinal = "W", Full = "Wildcreek Run", Short = "Wildcreek" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Jerrifers, Cardinal = "SW", Full = "Jerrifer's Slough", Short = "Jerrifer" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Klovan, Cardinal = "SW", Full = "Klovan Gully", Short = "Klovan" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Langor, Cardinal = "SE", Full = "Langor Gulch", Short = "Langor" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Quentin, Cardinal = "SE", Full = "Quentin Lake", Short = "Quentin" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Mendons, Cardinal = "NW", Full = "Mendon's Gap", Short = "Mendon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Anzalias, Cardinal = "NW", Full = "Anzalias Pass", Short = "Anzalias" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Ogrewatch, Cardinal = "NE", Full = "Ogrewatch Cut", Short = "Ogrewatch" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Veloka, Cardinal = "NE", Full = "Veloka Slope", Short = "Veloka" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Durios, Cardinal = "E", Full = "Durios Gulch", Short = "Durios" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Bravost, Cardinal = "E", Full = "Bravost Escarpment", Short = "Bravost" },

                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Garrison, Cardinal = "C", Full = "Garrison", Short = "Garrison" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Orchard, Cardinal = "S", Full = "Champion's Demense", Short = "Orchard" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Redbriar, Cardinal = "SW", Full = "Redbriar", Short = "Redbriar" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Greenlake, Cardinal = "SE", Full = "Greenlake", Short = "Greenlake" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Bay, Cardinal = "W", Full = "Ascension Bay", Short = "Bay" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Dawns, Cardinal = "NE", Full = "Dawn's Eyrie", Short = "Dawn's" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Spiritholme, Cardinal = "N", Full = "The Spiritholme", Short = "Spiritholme" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Woodhaven, Cardinal = "NW", Full = "Woodhaven", Short = "Woodhaven" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Hills, Cardinal = "E", Full = "Askalion Hills", Short = "Hills" },

                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Hills, Cardinal = "E", Full = "Etheron Hills", Short = "Hills" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Bay, Cardinal = "W", Full = "Dreaming Bay", Short = "Bay" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Orchard, Cardinal = "S", Full = "Victors's Lodge", Short = "Orchard" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Greenbriar, Cardinal = "SW", Full = "Greenbriar", Short = "Greenbriar" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Bluelake, Cardinal = "SE", Full = "Bluelake", Short = "Bluelake" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Garrison, Cardinal = "C", Full = "Garrison", Short = "Garrison" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Longview, Cardinal = "NW", Full = "Longview", Short = "Longview" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Godsword, Cardinal = "N", Full = "The Godsword", Short = "Godsword" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Cliffside, Cardinal = "NE", Full = "Cliffside", Short = "Cliffside" },

                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Hills, Cardinal = "E", Full = "Shadaran Hills", Short = "Hills" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Redlake, Cardinal = "SE", Full = "Redlake", Short = "Redlake" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Orchard, Cardinal = "S", Full = "Hero's Lodge", Short = "Orchard" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Bay, Cardinal = "W", Full = "Dreadfall Bay", Short = "Bay" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Bluebriar, Cardinal = "SW", Full = "Bluebriar", Short = "Bluebriar" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Garrison, Cardinal = "C", Full = "Garrison", Short = "Garrison" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Sunnyhill, Cardinal = "NW", Full = "Sunnyhill", Short = "Sunnyhill" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Faithleap, Cardinal = "NW", Full = "Faithleap", Short = "Faithleap" },

                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Bluevale, Cardinal = "SW", Full = "Bluevale Refuge", Short = "Bluevale" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Bluewater, Cardinal = "SE", Full = "Bluewater Lowlands", Short = "Bluewater" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Astralholme, Cardinal = "NE", Full = "Astralholme", Short = "Astralholme" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Arahs, Cardinal = "NW", Full = "Arah's Hope", Short = "Arah's" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Greenvale, Cardinal = "SW", Full = "Greenvale Refuge", Short = "Greenvale" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Foghaven, Cardinal = "NE", Full = "Foghaven", Short = "Foghaven" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Redwater, Cardinal = "SE", Full = "Redwater Lowlands", Short = "Redwater" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Titanpaw, Cardinal = "N", Full = "The Titanpaw", Short = "Titanpaw" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Cragtop, Cardinal = "NE", Full = "Cragtop", Short = "Cragtop" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Godslore, Cardinal = "NW", Full = "Godslore", Short = "Godslore" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Redvale, Cardinal = "SW", Full = "Redvale Refuge", Short = "Redvale" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Stargrove, Cardinal = "NE", Full = "Stargrove", Short = "Stargrove" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Greenwater, Cardinal = "SE", Full = "Greenwater Lowlands", Short = "Greenwater" },

                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Temple, Cardinal = "", Full = "Temple of Lost Prayers", Short = "Temple" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Hollow, Cardinal = "", Full = "Battle's Hollow", Short = "Hollow" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Estate, Cardinal = "", Full = "Bauer's Estate", Short = "Estate" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Orchard, Cardinal = "", Full = "Orchard Overlook", Short = "Orchard" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Carvers, Cardinal = "", Full = "Carver's Ascent", Short = "Carver's" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Carvers, Cardinal = "", Full = "Carver's Ascent", Short = "Carver's" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Orchard, Cardinal = "", Full = "Orchard Overlook", Short = "Orchard" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Estate, Cardinal = "", Full = "Bauer's Estate", Short = "Estate" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Hollow, Cardinal = "", Full = "Battle's Hollow", Short = "Hollow" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Temple, Cardinal = "", Full = "Temple of Lost Prayers", Short = "Temple" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Carvers, Cardinal = "", Full = "Carver's Ascent", Short = "Carver's" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Orchard, Cardinal = "", Full = "Orchard Overlook", Short = "Orchard" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Estate, Cardinal = "", Full = "Bauer's Estate", Short = "Estate" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Hollow, Cardinal = "", Full = "Battle's Hollow", Short = "Hollow" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Temple, Cardinal = "", Full = "Temple of Lost Prayers", Short = "Temple" }
            };

            // Spanish
            List<ObjectiveNames> spanish = new List<ObjectiveNames>()
            {
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Overlook, Cardinal = "N", Full = "Mirador", Short = "Mirador" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Valley, Cardinal = "SE", Full = "Valle", Short = "Valle" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Lowlands, Cardinal = "SO", Full = "Tierras Bajas", Short = "Tierras Bajas" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Golanta, Cardinal = "SSO", Full = "Claro Golanta", Short = "Golanta" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Pangloss, Cardinal = "NNE", Full = "Colina Pangloss", Short = "Pangloss" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Speldan, Cardinal = "NNO", Full = "Claro Espeldia", Short = "Espeldia" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Danelon, Cardinal = "SSE", Full = "Pasaje Danelon", Short = "Danelon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Umberglade, Cardinal = "E", Full = "Bosques Clarosombra", Short = "Clarosombra" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Castle_Stonemist, Cardinal = "C", Full = "Castillo Piedraniebla", Short = "Piedraniebla" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Rogues, Cardinal = "O", Full = "Cantera del Pícaro", Short = "Pícaro" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Aldons, Cardinal = "O", Full = "Pista Arroyosalvaje", Short = "Arroyosalvaje" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Wildcreek, Cardinal = "O", Full = "Pista Arroyosalvaje", Short = "Arroyosalvaje" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Jerrifers, Cardinal = "SO", Full = "Cenagal de Jerrifer", Short = "Jerrifer" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Klovan, Cardinal = "SO", Full = "Barranco Klovan", Short = "Klovan" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Langor, Cardinal = "SE", Full = "Barranco Langor", Short = "Langor" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Quentin, Cardinal = "SE", Full = "Lago Quentin", Short = "Quentin" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Mendons, Cardinal = "NO", Full = "Zanja de Mendon", Short = "Mendon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Anzalias, Cardinal = "NO", Full = "Paso Anzalias", Short = "Anzalias" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Ogrewatch, Cardinal = "NE", Full = "Tajo de la Guardia del Ogro", Short = "Ogro" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Veloka, Cardinal = "NE", Full = "Pendiente Veloka", Short = "Veloka" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Durios, Cardinal = "E", Full = "Barranco Durios", Short = "Durios" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Bravost, Cardinal = "E", Full = "Escarpadura Bravost", Short = "Bravost" },

                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Garrison, Cardinal = "C", Full = "Fuerte", Short = "Fuerte" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Orchard, Cardinal = "S", Full = "Dominio del Campeón", Short = "Huerto" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Redbriar, Cardinal = "SO", Full = "Zarzarroja", Short = "Zarzarroja" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Greenlake, Cardinal = "SE", Full = "Lagoverde", Short = "Lagoverde" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Bay, Cardinal = "O", Full = "Bahía de la Ascensión", Short = "Bahía" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Dawns, Cardinal = "NE", Full = "Aguilera del Alba", Short = "Alba" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Spiritholme, Cardinal = "N", Full = "La Isleta Espiritual", Short = "Espiritual" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Woodhaven, Cardinal = "NO", Full = "Refugio Forestal", Short = "Refugio" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Hills, Cardinal = "E", Full = "Colinas Askalion", Short = "Colinas" },

                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Hills, Cardinal = "E", Full = "Colinas Etheron", Short = "Colinas" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Bay, Cardinal = "O", Full = "Bahía Onírica", Short = "Bahía" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Orchard, Cardinal = "S", Full = "Albergue del Vencedor", Short = "Huerto" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Greenbriar, Cardinal = "SO", Full = "Zarzaverde", Short = "Zarzaverde" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Bluelake, Cardinal = "SE", Full = "Lagoazul", Short = "Lagoazul" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Garrison, Cardinal = "C", Full = "Fuerte", Short = "Fuerte" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Longview, Cardinal = "NO", Full = "Vistaluenga", Short = "Vistaluenga" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Godsword, Cardinal = "N", Full = "La Hoja Divina", Short = "Hoja Divina" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Cliffside, Cardinal = "NE", Full = "Despeñadero", Short = "Despeñadero" },

                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Hills, Cardinal = "E", Full = "Colinas Shadaran", Short = "Colinas" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Redlake, Cardinal = "SE", Full = "Lagorrojo", Short = "Lagorrojo" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Orchard, Cardinal = "S", Full = "Albergue del Héro", Short = "Huerto" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Bay, Cardinal = "O", Full = "Bahía Salto Aciago", Short = "Bahía" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Bluebriar, Cardinal = "SO", Full = "Zarzazul", Short = "Zarzazul" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Garrison, Cardinal = "C", Full = "Fuerte", Short = "Fuerte" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Sunnyhill, Cardinal = "NO", Full = "Colina Soleada", Short = "Soleada" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Faithleap, Cardinal = "NO", Full = "Salto de Fe", Short = "Salto de Fe" },

                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Bluevale, Cardinal = "SO", Full = "Refugio Valleazule", Short = "Valleazule" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Bluewater, Cardinal = "SE", Full = "Tierras Bajas de Aguazul", Short = "Aguazul" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Astralholme, Cardinal = "NE", Full = "Isleta Astral", Short = "Isleta Astral" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Arahs, Cardinal = "NO", Full = "Esperanza de Arah", Short = "Arah" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Greenvale, Cardinal = "SO", Full = "Refugio Valleverde", Short = "Valleverde" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Foghaven, Cardinal = "NE", Full = "Refugio Neblinoso", Short = "Neblinoso" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Redwater, Cardinal = "SE", Full = "Tierras Bajas de Aguarrojo", Short = "Aguarrojo" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Titanpaw, Cardinal = "N", Full = "La Garra del Titán", Short = "Titán" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Cragtop, Cardinal = "NE", Full = "Cumbrepeñasco", Short = "Cumbrepeñasco" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Godslore, Cardinal = "NO", Full = "Sabiduría de los Dioses", Short = "Dioses" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Redvale, Cardinal = "SO", Full = "Refugio Vallerrojo", Short = "Vallerrojo" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Stargrove, Cardinal = "NE", Full = "Arboleda de las Estrellas", Short = "Estrellas" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Greenwater, Cardinal = "SE", Full = "Tierras Bajas de Aguaverde", Short = "Aguaverde" },

                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Temple, Cardinal = "", Full = "Templo de las Plegarias Perdidas", Short = "Templo" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Hollow, Cardinal = "", Full = "Hondonada de la batalla", Short = "Hondonada" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Estate, Cardinal = "", Full = "Hacienda de Bauer", Short = "Hacienda" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Orchard, Cardinal = "", Full = "Mirador del Huerto", Short = "Huerto" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Carvers, Cardinal = "", Full = "Ascenso del Trinchador", Short = "Trinchador" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Carvers, Cardinal = "", Full = "Ascenso del Trinchador", Short = "Trinchador" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Orchard, Cardinal = "", Full = "Mirador del Huerto", Short = "Huerto" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Estate, Cardinal = "", Full = "Hacienda de Bauer", Short = "Hacienda" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Hollow, Cardinal = "", Full = "Hondonada de la batalla", Short = "Hondonada" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Temple, Cardinal = "", Full = "Templo de las Plegarias Perdidas", Short = "Templo" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Carvers, Cardinal = "", Full = "Ascenso del Trinchador", Short = "Trinchador" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Orchard, Cardinal = "", Full = "Mirador del Huerto", Short = "Huerto" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Estate, Cardinal = "", Full = "Hacienda de Bauer", Short = "Hacienda" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Hollow, Cardinal = "", Full = "Hondonada de la batalla", Short = "Hondonada" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Temple, Cardinal = "", Full = "Templo de las Plegarias Perdidas", Short = "Templo" }
            };

            // French
            List<ObjectiveNames> french = new List<ObjectiveNames>()
            {
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Overlook, Cardinal = "N", Full = "Belvédère", Short = "Belvédère" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Valley, Cardinal = "SE", Full = "Vallée", Short = "Vallée" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Lowlands, Cardinal = "SO", Full = "Basses Terres", Short = "Basses Terres" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Golanta, Cardinal = "SSO", Full = "Clairière de Golanta", Short = "Golanta" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Pangloss, Cardinal = "NNE", Full = "Mine de Pangloss", Short = "Pangloss" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Speldan, Cardinal = "NNO", Full = "Forêt de Speldan", Short = "Speldan" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Danelon, Cardinal = "SSE", Full = "Passage de Danelon", Short = "Danelon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Umberglade, Cardinal = "E", Full = "Bois d'Ombreclair", Short = "Ombreclair" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Castle_Stonemist, Cardinal = "C", Full = "Chateau Brumepierre", Short = "Brumepierre" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Rogues, Cardinal = "O", Full = "Carrière du Voleur", Short = "Voleur" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Aldons, Cardinal = "O", Full = "Corniche d'Aldon", Short = "Aldon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Wildcreek, Cardinal = "O", Full = "Piste du Ruisseau sauvage", Short = "Ruisseau" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Jerrifers, Cardinal = "SO", Full = "Bourbier de Jerrifer", Short = "Jerrifer" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Klovan, Cardinal = "SO", Full = "Ravin de Klovan", Short = "Klovan" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Langor, Cardinal = "SE", Full = "Ravin de Langor", Short = "Langor" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Quentin, Cardinal = "SE", Full = "Lac Quentin", Short = "Quentin" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Mendons, Cardinal = "NO", Full = "Faille de Mendon", Short = "Mendon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Anzalias, Cardinal = "NO", Full = "Col d'Anzalias", Short = "Anzalias" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Ogrewatch, Cardinal = "NE", Full = "Percée de Gardogre", Short = "Gardogre" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Veloka, Cardinal = "NE", Full = "Flanc de Veloka", Short = "Veloka" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Durios, Cardinal = "E", Full = "Ravin de Durios", Short = "Durios" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Bravost, Cardinal = "E", Full = "Falaise de Bravost", Short = "Bravost" },

                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Garrison, Cardinal = "C", Full = "Garnison", Short = "Garnison" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Orchard, Cardinal = "S", Full = "Fief du Champion", Short = "Verger" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Redbriar, Cardinal = "SO", Full = "Bruyerouge", Short = "Bruyerouge" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Greenlake, Cardinal = "SE", Full = "Lac Vert", Short = "Lac Vert" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Bay, Cardinal = "O", Full = "Baie de l'Ascension", Short = "Baie" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Dawns, Cardinal = "NE", Full = "Repaire de l'AUbe", Short = "Aube" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Spiritholme, Cardinal = "N", Full = "Le Heaume Spirituel", Short = "Heaume" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Woodhaven, Cardinal = "NO", Full = "Boisrefuge", Short = "Boisrefuge" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Hills, Cardinal = "E", Full = "Collines d'Askalion", Short = "Askalion" },

                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Hills, Cardinal = "E", Full = "Collines d'Etheron", Short = "Etheron" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Bay, Cardinal = "O", Full = "Baie des Rêves", Short = "Baie" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Orchard, Cardinal = "S", Full = "Pavillon du Vainqueur", Short = "Verger" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Greenbriar, Cardinal = "SO", Full = "Vert-Bruyère", Short = "Vert-Bruyère" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Bluelake, Cardinal = "SE", Full = "Lac Bleu", Short = "Lac Bleu" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Garrison, Cardinal = "C", Full = "Garnison", Short = "Garnison" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Longview, Cardinal = "NO", Full = "Longuevue", Short = "Longuevue" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Godsword, Cardinal = "N", Full = "Epée Divine", Short = "Epée" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Cliffside, Cardinal = "NE", Full = "Flanc de Falaise", Short = "Falaise" },

                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Hills, Cardinal = "E", Full = "Collines Shadaran", Short = "Shadaran" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Redlake, Cardinal = "SE", Full = "Lac Rouge", Short = "Lac Rouge" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Orchard, Cardinal = "S", Full = "Pavillon du Héros", Short = "Verger" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Bay, Cardinal = "O", Full = "Baie du Déclin Noir", Short = "Baie" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Bluebriar, Cardinal = "SO", Full = "Bruyazur", Short = "Bruyazur" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Garrison, Cardinal = "C", Full = "Garnison", Short = "Garnison" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Sunnyhill, Cardinal = "NO", Full = "Colline ensoleillée", Short = "Colline" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Faithleap, Cardinal = "NO", Full = "Saut de la Foi", Short = "Saut de la Foi" },

                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Bluevale, Cardinal = "SO", Full = "Refuge de Bleuval", Short = "Bleuval" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Bluewater, Cardinal = "SE", Full = "Basses terres d'Eau-Azur", Short = "Basses terres" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Astralholme, Cardinal = "NE", Full = "Heaume Astral", Short = "Astral" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Arahs, Cardinal = "NO", Full = "Espoir d'Arah", Short = "Arah" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Greenvale, Cardinal = "SO", Full = "Refuge de Valvert", Short = "Valvert" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Foghaven, Cardinal = "NE", Full = "Havre Gris", Short = "Havre" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Redwater, Cardinal = "SE", Full = "Basses terres de Rubicon", Short = "Basses terres" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Titanpaw, Cardinal = "N", Full = "Bras du Titan", Short = "Titan" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Cragtop, Cardinal = "NE", Full = "Sommet de HautCrag", Short = "Hautcrag" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Godslore, Cardinal = "NO", Full = "Savoir Divin", Short = "Divi" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Redvale, Cardinal = "SO", Full = "Refuge de Valrouge", Short = "Valrouge" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Stargrove, Cardinal = "NE", Full = "Bosquet Etoilé", Short = "Bosquet" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Greenwater, Cardinal = "SE", Full = "Basses terres d'Eau-Verdoyante", Short = "Basses terres" },

                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Temple, Cardinal = "", Full = "Temple des Prières Perdues", Short = "Temple" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Hollow, Cardinal = "", Full = "Vallon de bataille", Short = "Vallon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Estate, Cardinal = "", Full = "Domaine de Bauer", Short = "Bauer" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Orchard, Cardinal = "", Full = "Belvédère du Berger", Short = "Verger" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Carvers, Cardinal = "", Full = "Côte du Couteau", Short = "Carver's" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Carvers, Cardinal = "", Full = "Côte du Couteau", Short = "Carver's" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Orchard, Cardinal = "", Full = "Belvédère du Berger", Short = "Verger" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Estate, Cardinal = "", Full = "Domaine de Bauer", Short = "Bauer" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Hollow, Cardinal = "", Full = "Vallon de bataille", Short = "Vallon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Temple, Cardinal = "", Full = "Temple des Prières Perdues", Short = "Temple" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Carvers, Cardinal = "", Full = "Côte du Couteau", Short = "Carver's" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Orchard, Cardinal = "", Full = "Belvédère du Berger", Short = "Verger" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Estate, Cardinal = "", Full = "Domaine de Bauer", Short = "Bauer" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Hollow, Cardinal = "", Full = "Vallon de bataille", Short = "Vallon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Temple, Cardinal = "", Full = "Temple des Prières Perdues", Short = "Temple" }
            };

            // German
            List<ObjectiveNames> german = new List<ObjectiveNames>()
            {
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Overlook, Cardinal = "N", Full = "Aussichtspunkt", Short = "Aussichtspunkt" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Valley, Cardinal = "SO", Full = "Tal", Short = "Tal" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Keep_Lowlands, Cardinal = "SW", Full = "Tiefland", Short = "Tiefland" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Golanta, Cardinal = "SSW", Full = "Golanta-Lichtung", Short = "Golanta" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Pangloss, Cardinal = "NNO", Full = "Pangloss-Anhöhe", Short = "Pangloss" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Speldan, Cardinal = "NNW", Full = "Speldan-Kalschlag", Short = "Speldan" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Danelon, Cardinal = "SSO", Full = "Danelon-Passage", Short = "Danelon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Umberglade, Cardinal = "O", Full = "Umberlichtung-Forst", Short = "Umber" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Castle_Stonemist, Cardinal = "Z", Full = "Schloss Steinnebel", Short = "Steinnebel" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Camp_Rogues, Cardinal = "W", Full = "Schurkenbruch", Short = "Schurken" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Aldons, Cardinal = "W", Full = "Aldons Vorsprung", Short = "Aldon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Wildcreek, Cardinal = "W", Full = "Wildbach-Strecke", Short = "Wildbach" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Jerrifers, Cardinal = "SW", Full = "Jerrifers Sumpfloch", Short = "Jerrifer" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Klovan, Cardinal = "SW", Full = "Klovan-Senke", Short = "Klovan" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Langor, Cardinal = "SO", Full = "Langor-Schlucht", Short = "Langor" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Quentin, Cardinal = "SO", Full = "Quentin-See", Short = "Quentin" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Mendons, Cardinal = "NW", Full = "Mendon Spalt", Short = "Mendon" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Anzalias, Cardinal = "NW", Full = "Anzalias-Pass", Short = "Anzalias" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Ogrewatch, Cardinal = "NO", Full = "Ogrewatch-Kanal", Short = "Ogrewatch" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Veloka, Cardinal = "NO", Full = "Veloka-Hang", Short = "Veloka" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Durios, Cardinal = "O", Full = "Durios-Schlucht", Short = "Durios" },
                new ObjectiveNames() { ID = WvWObjectiveIds.EB_Tower_Bravost, Cardinal = "O", Full = "Bravost-Abhang", Short = "Bravost" },

                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Garrison, Cardinal = "Z", Full = "Festung", Short = "Festung" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Orchard, Cardinal = "S", Full = "Domäne des Champions", Short = "Obstgarten" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Redbriar, Cardinal = "SW", Full = "Rotstrauch", Short = "Rotstrauch" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Greenlake, Cardinal = "SO", Full = "Grünsee", Short = "Grünsee" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Bay, Cardinal = "W", Full = "Aufstiegsbucht", Short = "Bucht" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Dawns, Cardinal = "NO", Full = "Horst der Morgendammerung", Short = "Morgen" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Spiritholme, Cardinal = "N", Full = "Der Geistholm", Short = "Geistholm" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Tower_Woodhaven, Cardinal = "NW", Full = "Wald-Freistatt", Short = "Wald-Freistatt" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Keep_Hills, Cardinal = "O", Full = "Askalion-Hügel", Short = "Hügel" },

                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Hills, Cardinal = "O", Full = "Etheron-Hügel", Short = "Hügel" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Bay, Cardinal = "W", Full = "Traumbucht", Short = "Bucht" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Orchard, Cardinal = "S", Full = "Sieger-Halle", Short = "Obstgarten" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Greenbriar, Cardinal = "SW", Full = "Grünstrauch", Short = "Grünstrauch" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Bluelake, Cardinal = "SO", Full = "Blausee", Short = "Blausee" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Keep_Garrison, Cardinal = "Z", Full = "Festung", Short = "Festung" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Longview, Cardinal = "NW", Full = "Weitsicht", Short = "Weitsicht" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Godsword, Cardinal = "N", Full = "Das Gottesschwert", Short = "Gottesschwert" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Tower_Cliffside, Cardinal = "NO", Full = "Felswand", Short = "Felswand" },

                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Hills, Cardinal = "O", Full = "Shadaran-Hügel", Short = "Hügel" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Redlake, Cardinal = "SO", Full = "Rotsee", Short = "Rotsee" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Orchard, Cardinal = "S", Full = "Heldenhalle", Short = "Obstgarten" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Bay, Cardinal = "W", Full = "Schreckensfall-Bucht", Short = "Bucht" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Bluebriar, Cardinal = "SW", Full = "Blaustrauch", Short = "Blaustrauch" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Keep_Garrison, Cardinal = "Z", Full = "Festung", Short = "Festung" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Sunnyhill, Cardinal = "NW", Full = "Sonnenhügel", Short = "Sonnenhügel" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Faithleap, Cardinal = "NW", Full = "Glaubenssprung", Short = "Glaubenssprung" },

                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Bluevale, Cardinal = "SW", Full = "Blautal-Zuflucht", Short = "Blautal" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Bluewater, Cardinal = "SO", Full = "Blauwasser-Tiefland", Short = "Blauwasser" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Astralholme, Cardinal = "NO", Full = "Astralholm", Short = "Astralholm" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Arahs, Cardinal = "NW", Full = "Arahs Hoffnung", Short = "Arahs" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Camp_Greenvale, Cardinal = "SW", Full = "Grüntal-Zuflucht", Short = "Grüntal" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Foghaven, Cardinal = "NO", Full = "Nebel-Freistatt", Short = "Nebel-Freistatt" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Redwater, Cardinal = "SO", Full = "Rotwasser-Tiefland", Short = "Rotwasser" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Camp_Titanpaw, Cardinal = "N", Full = "Die Titanenpranke", Short = "Titanenpranke" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Tower_Cragtop, Cardinal = "NO", Full = "Schroffgipfel", Short = "Schroffgipfel" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Godslore, Cardinal = "NW", Full = "Gottessage", Short = "Gottessage" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Redvale, Cardinal = "SW", Full = "Rottal-Zuflucht", Short = "Rottal" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Stargrove, Cardinal = "NO", Full = "Sternhain", Short = "Sternhain" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Camp_Greenwater, Cardinal = "SO", Full = "Grünwasser-Tiefland", Short = "Grünwasser" },

                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Temple, Cardinal = "", Full = "Tempel der Verlorenen Gebete", Short = "Tempel" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Hollow, Cardinal = "", Full = "Schlachten-Senke", Short = "Senke" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Estate, Cardinal = "", Full = "Bauers Anwesen", Short = "Anwesen" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Orchard, Cardinal = "", Full = "Obstgarten-Aussichtspunkt", Short = "Obstgarten" },
                new ObjectiveNames() { ID = WvWObjectiveIds.RB_Carvers, Cardinal = "", Full = "Aufstieg des Schnitzers", Short = "Schnitzers" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Carvers, Cardinal = "", Full = "Aufstieg des Schnitzers", Short = "Schnitzers" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Orchard, Cardinal = "", Full = "Obstgarten-Aussichtspunkt", Short = "Obstgarten" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Estate, Cardinal = "", Full = "Bauers Anwesen", Short = "Anwesen" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Hollow, Cardinal = "", Full = "Schlachten-Senke", Short = "Senke" },
                new ObjectiveNames() { ID = WvWObjectiveIds.BB_Temple, Cardinal = "", Full = "Tempel der Verlorenen Gebete", Short = "Tempel" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Carvers, Cardinal = "", Full = "Aufstieg des Schnitzers", Short = "Schnitzers" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Orchard, Cardinal = "", Full = "Obstgarten-Aussichtspunkt", Short = "Obstgarten" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Estate, Cardinal = "", Full = "Bauers Anwesen", Short = "Anwesen" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Hollow, Cardinal = "", Full = "Schlachten-Senke", Short = "Senke" },
                new ObjectiveNames() { ID = WvWObjectiveIds.GB_Temple, Cardinal = "", Full = "Tempel der Verlorenen Gebete", Short = "Tempel" }
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
            return string.Format("{0}\\{1}\\{2}", Paths.LocalizationFolder, twoLetterIsoLangId, "WvWObjectiveNames.xml");
        }

        /// <summary>
        /// Container class for objective names
        /// </summary>
        public class ObjectiveNames
        {
            public WvWObjectiveId ID { get; set; }
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
