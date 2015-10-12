using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2NET;
using GW2NET.Common;
using GW2NET.Maps;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using NLog;

namespace GW2PAO.API.Services
{
    /// <summary>
    /// Service class used to retrieve information such as points of interest, hearts, waypoints, etc
    /// </summary>
    [Export(typeof(IZoneService))]
    public class ZoneService : IZoneService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Static collection of zone/map names
        /// </summary>
        private ConcurrentDictionary<int, MapName> MapNamesCache;

        private readonly object initLock = new object();

        /// <summary>
        /// Static constructor, initializes the MapNames static property
        /// </summary>
        public ZoneService()
        {
            this.MapNamesCache = new ConcurrentDictionary<int, MapName>();
        }

        /// <summary>
        /// Initializes the zone service
        /// </summary>
        public void Initialize()
        {
            if (Monitor.TryEnter(this.initLock))
            {
                try
                {
                    if (this.MapNamesCache.IsEmpty)
                    {
                        foreach (var mapName in GW2.V1.MapNames.ForCurrentUICulture().FindAll())
                        {
                            this.MapNamesCache.TryAdd(mapName.Key, mapName.Value);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(this.initLock);
                }
            }
        }

        /// <summary>
        /// Retrieves the continent information for the given continent ID
        /// </summary>
        /// <param name="mapId">The ID of a continent</param>
        /// <returns>The continent data</returns>
        public Data.Entities.Continent GetContinent(int continentId)
        {
            try
            {
                // Get all continents
                var continent = GW2.V2.Continents.ForCurrentUICulture().Find(continentId);
                if (continent != null)
                {
                    Data.Entities.Continent cont = new Data.Entities.Continent(continentId);
                    cont.Name = continent.Name;
                    cont.Height = continent.ContinentDimensions.Height;
                    cont.Width = continent.ContinentDimensions.Width;
                    cont.FloorIds = continent.FloorIds;
                    cont.MaxZoom = continent.MaximumZoom;
                    cont.MinZoom = continent.MinimumZoom;

                    return cont;
                }  
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong, but log the error
                logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Retrieves the continent information for the given map ID
        /// </summary>
        /// <param name="mapId">The ID of a zone</param>
        /// <returns>The continent data</returns>
        public Data.Entities.Continent GetContinentByMap(int mapId)
        {
            try
            {
                // Get all continents
                var continents = GW2.V2.Continents.ForCurrentUICulture().FindAll();

                // Get the map info
                var map = GW2.V2.Maps.ForCurrentUICulture().Find(mapId);
                if (map != null)
                {
                    // Find the map's continent
                    var continent = continents[map.ContinentId];

                    if (continent != null)
                    {
                        Data.Entities.Continent cont = new Data.Entities.Continent(map.ContinentId);
                        cont.Name = continent.Name;
                        cont.Height = continent.ContinentDimensions.Height;
                        cont.Width = continent.ContinentDimensions.Width;
                        cont.FloorIds = continent.FloorIds;
                        cont.MaxZoom = continent.MaximumZoom;
                        cont.MinZoom = continent.MinimumZoom;

                        return cont;
                    }
                }
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong, but log the error
                logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Retrieves continent information for all continents
        /// </summary>
        /// <returns>a collection of continents</returns>
        public IEnumerable<Data.Entities.Continent> GetContinents()
        {
            List<Data.Entities.Continent> continents = new List<Data.Entities.Continent>();

            try
            {
                var data = GW2.V2.Continents.ForCurrentUICulture().FindAll();
                foreach (var continent in data.Values)
                {
                    Data.Entities.Continent cont = new Data.Entities.Continent(continent.ContinentId);
                    cont.Name = continent.Name;
                    cont.Height = continent.ContinentDimensions.Height;
                    cont.Width = continent.ContinentDimensions.Width;
                    cont.FloorIds = continent.FloorIds;
                    cont.MaxZoom = continent.MaximumZoom;
                    cont.MinZoom = continent.MinimumZoom;

                    continents.Add(cont);
                }
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong, but log the error
                logger.Error(ex);
            }

            return continents;
        }

        /// <summary>
        /// Retrieves the map information for the given map ID
        /// </summary>
        /// <param name="mapId">The ID of a zone</param>
        /// <returns>The map data</returns>
        public Data.Entities.Map GetMap(int mapId)
        {
            try
            {
                // Get the current map info
                var map = GW2.V2.Maps.ForCurrentUICulture().Find(mapId);
                if (map != null)
                {
                    Data.Entities.Map m = new Data.Entities.Map(mapId);

                    m.MaxLevel = map.MaximumLevel;
                    m.MinLevel = map.MinimumLevel;
                    m.DefaultFloor = map.DefaultFloor;

                    m.ContinentId = map.ContinentId;
                    m.RegionId = map.RegionId;
                    m.RegionName = map.RegionName;

                    m.MapRectangle.X = map.MapRectangle.X;
                    m.MapRectangle.Y = map.MapRectangle.Y;
                    m.MapRectangle.Height = map.MapRectangle.Height;
                    m.MapRectangle.Width = map.MapRectangle.Width;

                    m.ContinentRectangle.X = map.ContinentRectangle.X;
                    m.ContinentRectangle.Y = map.ContinentRectangle.Y;
                    m.ContinentRectangle.Height = map.ContinentRectangle.Height;
                    m.ContinentRectangle.Width = map.ContinentRectangle.Width;

                    return m;
                }
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong, but log the error
                logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Retrieves a collection of ZoneItems located in the zone with the given mapID
        /// </summary>
        /// <param name="mapId">The mapID of the zone to retrieve zone items for</param>
        /// <returns>a collection of ZoneItems located in the zone with the given mapID</returns>
        public IEnumerable<ZoneItem> GetZoneItems(int mapId)
        {
            List<ZoneItem> zoneItems = new List<ZoneItem>();
            try
            {
                // Get the continents (used later in determining the location of items)
                var continents = GW2.V2.Continents.ForCurrentUICulture().FindAll();

                // Get the current map info
                var map = GW2.V2.Maps.ForCurrentUICulture().Find(mapId);
                if (map != null)
                {
                    // Find the map's continent
                    var continent = continents[map.ContinentId];
                    map.Continent = continent;

                    var floorService = GW2.V1.Floors.ForCurrentUICulture(map.ContinentId);

                    // Retrieve details of items on every floor of the map
                    foreach (var floorId in map.Floors)
                    {
                        var floor = floorService.Find(floorId);
                        if (floor != null && floor.Regions != null)
                        {
                            // Find the region that this map is located in
                            var region = floor.Regions.Values.FirstOrDefault(r => r.Maps.ContainsKey(mapId));
                            if (region != null)
                            {
                                if (region.Maps.ContainsKey(mapId))
                                {
                                    var regionMap = region.Maps[mapId];
                                    var continentRectangle = new Rectangle()
                                    {
                                        X = regionMap.ContinentRectangle.X,
                                        Y = regionMap.ContinentRectangle.Y,
                                        Height = regionMap.ContinentRectangle.Height,
                                        Width = regionMap.ContinentRectangle.Width
                                    };
                                    var mapRectangle = new Rectangle()
                                    {
                                        X = regionMap.MapRectangle.X,
                                        Y = regionMap.MapRectangle.Y,
                                        Height = regionMap.MapRectangle.Height,
                                        Width = regionMap.MapRectangle.Width
                                    };

                                    // Points of Interest
                                    foreach (var item in regionMap.PointsOfInterest)
                                    {
                                        // If we haven't already added the item, get it's info and add it
                                        if (!zoneItems.Any(zi => zi.ID == item.PointOfInterestId))
                                        {
                                            // Determine the location
                                            var location = MapsHelper.ConvertToMapPos(
                                                continentRectangle,
                                                mapRectangle,
                                                new Point(item.Coordinates.X, item.Coordinates.Y));

                                            ZoneItem zoneItem = new ZoneItem();
                                            zoneItem.ID = item.PointOfInterestId;
                                            zoneItem.Name = item.Name;
                                            zoneItem.ContinentLocation = new Point(item.Coordinates.X, item.Coordinates.Y);
                                            zoneItem.Location = new Point(location.X, location.Y);
                                            zoneItem.MapId = mapId;
                                            zoneItem.MapName = map.MapName;
                                            var mapChatLink = item.GetMapChatLink();
                                            if (mapChatLink != null)
                                                zoneItem.ChatCode = mapChatLink.ToString();

                                            // Translate the item's type
                                            if (item is GW2NET.Maps.Vista)
                                                zoneItem.Type = ZoneItemType.Vista;
                                            else if (item is GW2NET.Maps.Waypoint)
                                                zoneItem.Type = ZoneItemType.Waypoint;
                                            else if (item is GW2NET.Maps.Dungeon)
                                                zoneItem.Type = ZoneItemType.Dungeon;
                                            else
                                                zoneItem.Type = ZoneItemType.PointOfInterest;

                                            zoneItems.Add(zoneItem);
                                        }
                                    }

                                    // Iterate over every Task in the map (Tasks are the same as HeartQuests)
                                    foreach (var task in regionMap.Tasks)
                                    {
                                        // If we haven't already added the item, get it's info and add it
                                        if (!zoneItems.Any(zi => zi.ID == task.TaskId))
                                        {
                                            // Determine the location
                                            var location = MapsHelper.ConvertToMapPos(
                                                continentRectangle,
                                                mapRectangle, 
                                                new Point(task.Coordinates.X, task.Coordinates.Y));

                                            ZoneItem zoneItem = new ZoneItem();
                                            zoneItem.ID = task.TaskId;
                                            zoneItem.Name = task.Objective;
                                            zoneItem.Level = task.Level;
                                            zoneItem.ContinentLocation = new Point(task.Coordinates.X, task.Coordinates.Y);
                                            zoneItem.Location = new Point(location.X, location.Y);
                                            zoneItem.MapId = mapId;
                                            zoneItem.MapName = map.MapName;
                                            zoneItem.Type = ZoneItemType.HeartQuest;

                                            zoneItems.Add(zoneItem);
                                        }
                                    }

                                    // Iterate over every skill challenge in the map
                                    foreach (var skillChallenge in regionMap.SkillChallenges)
                                    {
                                        // Determine the location, this serves an internally-used ID for skill challenges
                                        var location = MapsHelper.ConvertToMapPos(
                                                continentRectangle,
                                                mapRectangle, 
                                                new Point(skillChallenge.Coordinates.X, skillChallenge.Coordinates.Y));

                                        // Use a custom-generated ID
                                        int id = (int)(mapId + location.X + location.Y);

                                        // If we havn't already added the item, get it's info and add it
                                        if (!zoneItems.Any(zi => zi.ID == id))
                                        {
                                            ZoneItem zoneItem = new ZoneItem();
                                            zoneItem.ID = id;
                                            zoneItem.ContinentLocation = new Point(skillChallenge.Coordinates.X, skillChallenge.Coordinates.Y);
                                            zoneItem.Location = new Point(location.X, location.Y);
                                            zoneItem.MapId = mapId;
                                            zoneItem.MapName = map.MapName;
                                            zoneItem.Type = Data.Enums.ZoneItemType.HeroPoint;

                                            zoneItems.Add(zoneItem);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong, but log the error
                logger.Error(ex);
            }
            return zoneItems;
        }

        /// <summary>
        /// Retrieves a collection of zone items located in the given continent
        /// </summary>
        /// <param name="continentId">ID of the continent</param>
        /// <returns></returns>
        public IEnumerable<ZoneItem> GetZoneItemsByContinent(int continentId)
        {
            ConcurrentDictionary<int, ZoneItem> pointsOfInterest = new ConcurrentDictionary<int, ZoneItem>();
            ConcurrentDictionary<int, ZoneItem> tasks = new ConcurrentDictionary<int, ZoneItem>();
            ConcurrentDictionary<int, ZoneItem> heroPoints = new ConcurrentDictionary<int, ZoneItem>();
            try
            {
                // Get the continents (used later in determining the location of items)
                var continent = GW2.V2.Continents.ForCurrentUICulture().Find(continentId);

                Parallel.ForEach(continent.FloorIds, floorId =>
                {
                    var floorService = GW2.V1.Floors.ForCurrentUICulture(continentId);

                    var floor = floorService.Find(floorId);
                    if (floor != null && floor.Regions != null)
                    {
                        foreach (var region in floor.Regions)
                        {
                            foreach (var subRegion in region.Value.Maps)
                            {
                                var continentRectangle = new Rectangle()
                                {
                                    X = subRegion.Value.ContinentRectangle.X,
                                    Y = subRegion.Value.ContinentRectangle.Y,
                                    Height = subRegion.Value.ContinentRectangle.Height,
                                    Width = subRegion.Value.ContinentRectangle.Width
                                };
                                var mapRectangle = new Rectangle()
                                {
                                    X = subRegion.Value.MapRectangle.X,
                                    Y = subRegion.Value.MapRectangle.Y,
                                    Height = subRegion.Value.MapRectangle.Height,
                                    Width = subRegion.Value.MapRectangle.Width
                                };

                                // Points of Interest
                                foreach (var item in subRegion.Value.PointsOfInterest)
                                {
                                    // If we haven't already added the item, get it's info and add it
                                    if (!pointsOfInterest.ContainsKey(item.PointOfInterestId))
                                    {
                                        // Determine the location
                                        var location = MapsHelper.ConvertToMapPos(
                                            continentRectangle,
                                            mapRectangle,
                                            new Point(item.Coordinates.X, item.Coordinates.Y));

                                        ZoneItem zoneItem = new ZoneItem();
                                        zoneItem.ID = item.PointOfInterestId;
                                        zoneItem.Name = item.Name;
                                        zoneItem.ContinentLocation = new Point(item.Coordinates.X, item.Coordinates.Y);
                                        zoneItem.Location = new Point(location.X, location.Y);
                                        zoneItem.MapId = subRegion.Value.MapId;
                                        zoneItem.MapName = this.MapNamesCache[subRegion.Value.MapId].Name;
                                        var mapChatLink = item.GetMapChatLink();
                                        if (mapChatLink != null)
                                            zoneItem.ChatCode = mapChatLink.ToString();

                                        // Translate the item's type
                                        if (item is GW2NET.Maps.Vista)
                                            zoneItem.Type = ZoneItemType.Vista;
                                        else if (item is GW2NET.Maps.Waypoint)
                                            zoneItem.Type = ZoneItemType.Waypoint;
                                        else if (item is GW2NET.Maps.Dungeon)
                                            zoneItem.Type = ZoneItemType.Dungeon;
                                        else
                                            zoneItem.Type = ZoneItemType.PointOfInterest;

                                        if (!pointsOfInterest.TryAdd(zoneItem.ID, zoneItem))
                                        {
                                            logger.Warn("Failed to add {0} to PointsOfInterest collection", zoneItem);
                                        }
                                    }
                                }

                                // Iterate over every Task in the map (Tasks are the same as HeartQuests)
                                foreach (var task in subRegion.Value.Tasks)
                                {
                                    // If we haven't already added the item, get it's info and add it
                                    if (!tasks.ContainsKey(task.TaskId))
                                    {
                                        // Determine the location
                                        var location = MapsHelper.ConvertToMapPos(
                                            continentRectangle,
                                            mapRectangle, 
                                            new Point(task.Coordinates.X, task.Coordinates.Y));

                                        ZoneItem zoneItem = new ZoneItem();
                                        zoneItem.ID = task.TaskId;
                                        zoneItem.Name = task.Objective;
                                        zoneItem.Level = task.Level;
                                        zoneItem.ContinentLocation = new Point(task.Coordinates.X, task.Coordinates.Y);
                                        zoneItem.Location = new Point(location.X, location.Y);
                                        zoneItem.MapId = subRegion.Value.MapId;
                                        zoneItem.MapName = this.MapNamesCache[subRegion.Value.MapId].Name;
                                        zoneItem.Type = ZoneItemType.HeartQuest;

                                        if (!tasks.TryAdd(zoneItem.ID, zoneItem))
                                        {
                                            logger.Warn("Failed to add {0} to Tasks collection", zoneItem);
                                        }
                                    }
                                }

                                // Iterate over every skill challenge in the map
                                foreach (var skillChallenge in subRegion.Value.SkillChallenges)
                                {
                                    // Determine the location, this serves an internally-used ID for skill challenges
                                    var location = MapsHelper.ConvertToMapPos(
                                            continentRectangle,
                                            mapRectangle, 
                                            new Point(skillChallenge.Coordinates.X, skillChallenge.Coordinates.Y));

                                    // Use a custom-generated ID
                                    int id = (int)(subRegion.Value.MapId + location.X + location.Y);

                                    // If we havn't already added the item, get it's info and add it
                                    if (!heroPoints.ContainsKey(id))
                                    {
                                        ZoneItem zoneItem = new ZoneItem();
                                        zoneItem.ID = id;
                                        zoneItem.ContinentLocation = new Point(skillChallenge.Coordinates.X, skillChallenge.Coordinates.Y);
                                        zoneItem.Location = new Point(location.X, location.Y);
                                        zoneItem.MapId = subRegion.Value.MapId;
                                        zoneItem.MapName = this.MapNamesCache[subRegion.Value.MapId].Name;
                                        zoneItem.Type = Data.Enums.ZoneItemType.HeroPoint;

                                        if (!heroPoints.TryAdd(zoneItem.ID, zoneItem))
                                        {
                                            logger.Warn("Failed to add {0} to HeroPoints collection", zoneItem);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    logger.Debug("{0} done", floor.FloorId);
                });
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong, but log the error
                logger.Error(ex);
            }
            return pointsOfInterest.Values.Concat(tasks.Values).Concat(heroPoints.Values);
        }

        /// <summary>
        /// Retrieves the name of the zone using the given mapID
        /// </summary>
        /// <param name="mapId">The mapID of the zone to retrieve the name for</param>
        /// <returns>the name of the zone</returns>
        public string GetZoneName(int mapId)
        {
            try
            {
                if (MapNamesCache.ContainsKey(mapId))
                {
                    return MapNamesCache[mapId].Name;
                }
                else
                {
                    var map = GW2.V2.Maps.ForCurrentUICulture().Find(mapId);
                    if (map != null)
                        return map.MapName;
                    else
                        return "Unknown";
                }
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong, but log the error
                logger.Error(ex);
                return "Unknown";
            }
        }
    }
}
