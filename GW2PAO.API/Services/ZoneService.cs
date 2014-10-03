using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2DotNET;
using GW2DotNET.Entities.Maps;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using NLog;

namespace GW2PAO.API.Services
{
    /// <summary>
    /// Service class used to retrieve information such as points of interest, hearts, waypoints, etc
    /// </summary>
    public class ZoneService : IZoneService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The GW2.NET API service objective
        /// </summary>
        private ServiceManager service = new ServiceManager();

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
                var continents = this.service.GetContinents();

                // Get the current map info
                var map = this.service.GetMap(mapId);
                if (map != null)
                {
                    // Find the map's continent
                    var continent = continents[map.ContinentId];
                    map.Continent = continent;

                    // Retrieve details of items on every floor of the map
                    foreach (var floorId in map.Floors)
                    {
                        var floor = this.service.GetMapFloor(map.ContinentId, floorId);
                        if (floor != null && floor.Regions != null)
                        {
                            // Find the region that this map is located in
                            var region = floor.Regions.Values.FirstOrDefault(r => r.Maps.ContainsKey(mapId));
                            if (region != null)
                            {
                                if (region.Maps.ContainsKey(mapId))
                                {
                                    var regionMap = region.Maps[mapId];

                                    // Points of Interest
                                    foreach (var item in regionMap.PointsOfInterest)
                                    {
                                        // If we haven't already added the item, get it's info and add it
                                        if (!zoneItems.Any(zi => zi.ID == item.PointOfInterestId))
                                        {
                                            // Determine the location
                                            var location = MapsHelper.ConvertToMapPos(map, new Point(item.Coordinates.X, item.Coordinates.Y));

                                            ZoneItem zoneItem = new ZoneItem();
                                            zoneItem.ID = item.PointOfInterestId;
                                            zoneItem.Name = item.Name;
                                            zoneItem.Location = new Point(location.X, location.Y);
                                            zoneItem.MapId = mapId;
                                            zoneItem.MapName = map.MapName;
                                            var mapChatLink = item.GetMapChatLink();
                                            if (mapChatLink != null)
                                                zoneItem.ChatCode = mapChatLink.ToString();

                                            // Translate the item's type
                                            if (item is GW2DotNET.Entities.Maps.Vista)
                                                zoneItem.Type = ZoneItemType.Vista;
                                            else if (item is GW2DotNET.Entities.Maps.Waypoint)
                                                zoneItem.Type = ZoneItemType.Waypoint;
                                            else if (item is GW2DotNET.Entities.Maps.Dungeon)
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
                                            var location = MapsHelper.ConvertToMapPos(map, new Point(task.Coordinates.X, task.Coordinates.Y));

                                            ZoneItem zoneItem = new ZoneItem();
                                            zoneItem.ID = task.TaskId;
                                            zoneItem.Name = task.Objective;
                                            zoneItem.Level = task.Level;
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
                                        var location = MapsHelper.ConvertToMapPos(map, new Point(skillChallenge.Coordinates.X, skillChallenge.Coordinates.Y));

                                        // Use a custom-generated ID
                                        int id = (int)(mapId + location.X + location.Y);

                                        // If we havn't already added the item, get it's info and add it
                                        if (!zoneItems.Any(zi => zi.ID == id))
                                        {
                                            ZoneItem zoneItem = new ZoneItem();
                                            zoneItem.ID = id;
                                            zoneItem.Location = new Point(location.X, location.Y);
                                            zoneItem.MapId = mapId;
                                            zoneItem.MapName = map.MapName;
                                            zoneItem.Type = Data.Enums.ZoneItemType.SkillChallenge;

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
        /// Retrieves the name of the zone using the given mapID
        /// </summary>
        /// <param name="mapId">The mapID of the zone to retrieve the name for</param>
        /// <returns>the name of the zone</returns>
        public string GetZoneName(int mapId)
        {
            try
            {
                var map = this.service.GetMap(mapId);
                if (map != null)
                    return map.MapName;
                else
                    return "Unknown";
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
