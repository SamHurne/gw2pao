using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Services.Interfaces;
using GwApiNET;
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
                var continents = GwApi.GetContinents();

                // Get the current map info
                var map = GwApi.GetMap(mapId).Values.FirstOrDefault();
                if (map != null)
                {
                    // Find the map's continent
                    var continent = continents[map.ContinentId];

                    // Retrieve details of items on every floor of the map
                    foreach (var floorId in map.Floors)
                    {
                        var floor = GwApi.GetMapFloor(map.ContinentId, floorId);
                        if (floor != null && floor.Regions != null)
                        {
                            // Find the region that this map is located in
                            var region = floor.Regions.Values.FirstOrDefault(r => r.Maps.ContainsKey(mapId));
                            if (region != null)
                            {
                                if (region.Maps.ContainsKey(mapId))
                                {
                                    var regionMap = region.Maps[mapId];

                                    // Iterate over every PointsOfInterest in the map (note: PointsOfInterest includes POIs, Vistas, and Waypoints)
                                    foreach (var item in regionMap.PointsOfInterest)
                                    {
                                        if (item.Type != PointOfInterestType.Unlock)
                                        {
                                            // If we havn't already added the item, get it's info and add it
                                            if (!zoneItems.Any(zi => zi.ID == item.Id))
                                            {
                                                // Determine the location
                                                var location = GwMapsHelper.PixelToWorldPos(map, new Gw2Point(item.Coordinates[0], item.Coordinates[1]), continent.MaxZoom);

                                                ZoneItem zoneItem = new ZoneItem();
                                                zoneItem.ID = item.Id;
                                                zoneItem.Name = item.Name;
                                                zoneItem.Location = new Point(location.X, location.Y);
                                                zoneItem.MapId = mapId;
                                                zoneItem.MapName = map.MapName;

                                                // Translate the item's type
                                                switch (item.Type)
                                                {
                                                    case PointOfInterestType.Landmark:
                                                        zoneItem.Type = Data.Enums.ZoneItemType.PointOfInterest;
                                                        break;
                                                    case PointOfInterestType.Vista:
                                                        zoneItem.Type = Data.Enums.ZoneItemType.Vista;
                                                        break;
                                                    case PointOfInterestType.Waypoint:
                                                        zoneItem.Type = Data.Enums.ZoneItemType.Waypoint;
                                                        break;
                                                }

                                                zoneItems.Add(zoneItem);
                                            }
                                        }
                                    }

                                    // Iterate over every Task in the map (Tasks are the same as HeartQuests
                                    foreach (var item in regionMap.Tasks)
                                    {
                                        // If we havn't already added the item, get it's info and add it
                                        if (!zoneItems.Any(zi => zi.ID == item.Id))
                                        {
                                            // Determine the location
                                            var location = GwMapsHelper.PixelToWorldPos(map, new Gw2Point(item.Coordinates[0], item.Coordinates[1]), continent.MaxZoom);

                                            ZoneItem zoneItem = new ZoneItem();
                                            zoneItem.ID = item.Id;
                                            zoneItem.Name = item.Objective;
                                            zoneItem.Level = item.Level;
                                            zoneItem.Location = new Point(location.X, location.Y);
                                            zoneItem.MapId = mapId;
                                            zoneItem.MapName = map.MapName;
                                            zoneItem.Type = Data.Enums.ZoneItemType.HeartQuest;

                                            zoneItems.Add(zoneItem);
                                        }
                                    }

                                    // Iterate over every skill challenge in the map
                                    foreach (var item in regionMap.SkillChallenges)
                                    {
                                        // Determine the location, this serves an internally-used ID for skill challenges
                                        var location = GwMapsHelper.PixelToWorldPos(map, new Gw2Point(item.Coordinents[0], item.Coordinents[1]), continent.MaxZoom);
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
                var map = GwApi.GetMap(mapId).Values.FirstOrDefault();
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
