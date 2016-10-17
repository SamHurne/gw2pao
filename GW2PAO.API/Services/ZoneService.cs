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
        /// Key: Map ID
        /// Value: Map Name
        /// </summary>
        private ConcurrentDictionary<int, MapName> MapNamesCache;

        /// <summary>
        /// Static collection of zone/continent data
        /// Key: Map ID
        /// Value: Continent ID
        /// </summary>
        private ConcurrentDictionary<int, int> MapContinentsCache;

        /// <summary>
        /// General cache of continents
        /// Key: Continent ID
        /// Value: Continent
        /// </summary>
        private ConcurrentDictionary<int, Data.Entities.Continent> ContinentsCache;

        /// <summary>
        /// General cache of continents
        /// Key: Continent & Floor ID
        /// Value: Floor Data
        /// </summary>
        private ConcurrentDictionary<Tuple<int, int>, GW2NET.Maps.Floor> FloorCache;

        private readonly object initLock = new object();

        /// <summary>
        /// Static constructor, initializes the MapNames static property
        /// </summary>
        public ZoneService()
        {
            this.MapNamesCache = new ConcurrentDictionary<int, MapName>();
            this.MapContinentsCache = new ConcurrentDictionary<int, int>();
            this.ContinentsCache = new ConcurrentDictionary<int, Data.Entities.Continent>();
            this.FloorCache = new ConcurrentDictionary<Tuple<int, int>, Floor>();
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

                    if (this.ContinentsCache.IsEmpty)
                    {
                        // Get all continents
                        var continents = GW2.V2.Continents.ForCurrentUICulture().FindAll();
                        foreach (var continent in continents.Values)
                        {
                            Data.Entities.Continent cont = new Data.Entities.Continent(continent.ContinentId);
                            cont.Name = continent.Name;
                            cont.Height = continent.ContinentDimensions.Height;
                            cont.Width = continent.ContinentDimensions.Width;
                            cont.FloorIds = continent.FloorIds;
                            cont.MaxZoom = continent.MaximumZoom;
                            cont.MinZoom = continent.MinimumZoom;
                            this.ContinentsCache.TryAdd(cont.Id, cont);
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
            Data.Entities.Continent result = null;
            if (this.ContinentsCache.ContainsKey(continentId))
            {
                this.ContinentsCache.TryGetValue(continentId, out result);
            }

            return result;
        }

        /// <summary>
        /// Retrieves the continent information for the given map ID
        /// </summary>
        /// <param name="mapId">The ID of a zone</param>
        /// <returns>The continent data</returns>
        public Data.Entities.Continent GetContinentByMap(int mapId)
        {
            Data.Entities.Continent result = null;

            if (this.MapContinentsCache.ContainsKey(mapId))
            {
                int continentId;
                this.MapContinentsCache.TryGetValue(mapId, out continentId);
                this.ContinentsCache.TryGetValue(continentId, out result);
            }
            
            if (result == null)
            {
                // If we didn't get the continent from our cache of Map ID -> Continent ID,
                // request the map info and add it our cache
                try
                {
                    var map = GW2.V2.Maps.ForCurrentUICulture().Find(mapId);
                    if (map != null)
                    {
                        this.MapContinentsCache.TryAdd(mapId, map.ContinentId);
                        this.ContinentsCache.TryGetValue(map.ContinentId, out result);
                    }
                }
                catch (Exception ex)
                {
                    // Don't crash if something goes wrong, but log the error
                    logger.Error(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves continent information for all continents
        /// </summary>
        /// <returns>a collection of continents</returns>
        public IEnumerable<Data.Entities.Continent> GetContinents()
        {
            return this.ContinentsCache.Values;
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
        /// Retrieves map information using the provided continent coordinates,
        /// or null if the coordinates do not fall into any current zone
        /// </summary>
        /// <param name="continentId">ID of the continent that the coordinates are for</param>
        /// <param name="continentCoordinates">Continent coordinates to use</param>
        /// <returns>The map data, or null if not found</returns>
        public Data.Entities.Map GetMap(int continentId, Point continentCoordinates)
        {
            var continent = this.GetContinent(continentId);
            var floorService = GW2.V1.Floors.ForCurrentUICulture(continentId);

            var floor = this.GetFloor(continentId, continent.FloorIds.First());
            if (floor != null && floor.Regions != null)
            {
                foreach (var region in floor.Regions.Values)
                {
                    foreach (var map in region.Maps.Values)
                    {
                        if (continentCoordinates.X >= map.ContinentRectangle.X
                            && continentCoordinates.X <= (map.ContinentRectangle.X + map.ContinentRectangle.Width)
                            && continentCoordinates.Y >= map.ContinentRectangle.Y
                            && continentCoordinates.Y <= (map.ContinentRectangle.Y + map.ContinentRectangle.Height))
                        {
                            Data.Entities.Map mapData = new Data.Entities.Map(map.MapId);

                            mapData.MaxLevel = map.MaximumLevel;
                            mapData.MinLevel = map.MinimumLevel;
                            mapData.DefaultFloor = map.DefaultFloor;

                            mapData.ContinentId = continent.Id;
                            mapData.RegionId = region.RegionId;
                            mapData.RegionName = region.Name;

                            mapData.MapRectangle.X = map.MapRectangle.X;
                            mapData.MapRectangle.Y = map.MapRectangle.Y;
                            mapData.MapRectangle.Height = map.MapRectangle.Height;
                            mapData.MapRectangle.Width = map.MapRectangle.Width;

                            mapData.ContinentRectangle.X = map.ContinentRectangle.X;
                            mapData.ContinentRectangle.Y = map.ContinentRectangle.Y;
                            mapData.ContinentRectangle.Height = map.ContinentRectangle.Height;
                            mapData.ContinentRectangle.Width = map.ContinentRectangle.Width;

                            // Done - return the data
                            return mapData;
                        }
                        
                    }
                }
            }

            // Not found
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
                var continents = this.GetContinents();

                // Get the current map info
                var map = GW2.V2.Maps.ForCurrentUICulture().Find(mapId);
                if (map != null)
                {
                    // Retrieve details of items on every floor of the map
                    foreach (var floorId in map.Floors)
                    {
                        var floor = this.GetFloor(map.ContinentId, floorId);
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
                var continent = this.GetContinent(continentId);

                Parallel.ForEach(continent.FloorIds, floorId =>
                {
                    var floor = this.GetFloor(continentId, floorId);
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

                    logger.Debug("{0}-{1} done", continentId, floorId);
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
            if (mapId <= 0)
                return "Unknown";

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

        /// <summary>
        /// Retrieves the floor data for the given floor ID, using the internal cache when possible
        /// </summary>
        /// <param name="continentId">ID of the continent the floor is for</param>
        /// <param name="floorId">ID of the floor to retrieve</param>
        /// <returns>The floor data</returns>
        private GW2NET.Maps.Floor GetFloor(int continentId, int floorId)
        {
            GW2NET.Maps.Floor result = null;

            var key = new Tuple<int, int>(continentId, floorId);

            if (this.FloorCache.ContainsKey(key))
            {
                this.FloorCache.TryGetValue(key, out result);
            }
            else
            {
                var floorService = GW2.V1.Floors.ForCurrentUICulture(continentId);
                var floor = floorService.Find(floorId);
                if (floor != null)
                {
                    this.FloorCache.TryAdd(key, floor);
                    result = floor;
                }
            }

            return result;
        }
    }
}
