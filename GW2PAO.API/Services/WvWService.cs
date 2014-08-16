using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using GwApiNET;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Services.Interfaces;

namespace GW2PAO.API.Services
{
    public class WvWService : IWvWService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Worlds table
        /// </summary>
        public WorldsTable Worlds { get; private set; }

        /// <summary>
        /// The WvW objectives table
        /// </summary>
        public WvWObjectivesTable ObjectivesTable { get; private set; }

        /// <summary>
        /// Loads the dungeons table and initializes all cached information
        /// </summary>
        public void LoadTable()
        {
            logger.Info("Loading Worlds Table");
            try
            {
                this.Worlds = WorldsTable.LoadTable();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Info("Error loading Worlds Table, re-creating table");
                WorldsTable.CreateTable();
                this.Worlds = WorldsTable.LoadTable();
            }

            logger.Info("Loading Objectives Table");
            try
            {
                this.ObjectivesTable = WvWObjectivesTable.LoadTable();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Info("Error loading Objectives Table, re-creating table");
                WvWObjectivesTable.CreateTable();
                this.ObjectivesTable = WvWObjectivesTable.LoadTable();
            }
        }

        /// <summary>
        /// Find and returns the Match ID for the given world
        /// </summary>
        /// <param name="worldId">The ID for the world</param>
        /// <returns>The match ID for the given world ID</returns>
        public string GetMatchId(int worldId)
        {
            try
            {
                var matches = GwApi.GetMatches();
                var wvwMatch = matches.Values.FirstOrDefault(match => match.BlueWorldId == worldId
                                                                   || match.RedWorldId == worldId
                                                                   || match.GreenWorldId == worldId);
                if (wvwMatch == null)
                    return null;
                else
                    return wvwMatch.Id;
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong (like if WvW is resetting)
                logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves the TeamColor of the given World
        /// </summary>
        /// <param name="worldId">The ID for the world</param>
        /// <returns>The team color for the given world</returns>
        public WorldColor GetTeamColor(int worldId)
        {
            try
            {
                var matches = GwApi.GetMatches();
                var wvwMatch = matches.Values.FirstOrDefault(match => match.BlueWorldId == worldId
                                                                   || match.RedWorldId == worldId
                                                                   || match.GreenWorldId == worldId);
                if (wvwMatch != null)
                {
                    if (wvwMatch.BlueWorldId == worldId)
                        return WorldColor.Blue;
                    else if (wvwMatch.RedWorldId == worldId)
                        return WorldColor.Red;
                    else if (wvwMatch.GreenWorldId == worldId)
                        return WorldColor.Green;
                    else
                        return WorldColor.None;
                }
                else
                {
                    return WorldColor.None;
                }
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong (like if WvW is resetting)
                logger.Error(ex);
                return WorldColor.None;
            }
        }

        /// <summary>
        /// Retrieves the current WvW score for the given world
        /// </summary>
        /// <param name="worldId">The ID for the world</param>
        /// <returns>The current WvW for the given world</returns>
        public int GetWorldScore(int worldId)
        {
            return this.GetWorldScore(this.GetMatchId(worldId), worldId);
        }

        /// <summary>
        /// Retrieves the current WvW score for the given world
        /// </summary>
        /// <param name="matchId">The match ID for the world</param>
        /// <param name="worldId">The ID for the world</param>
        /// <returns>The current WvW for the given world</returns>
        public int GetWorldScore(string matchId, int worldId)
        {
            try
            {
                var details = GwApiNET.GwApi.GetMatchDetails(matchId, true);
                if (details != null)
                {
                    switch (this.GetTeamColor(worldId))
                    {
                        case WorldColor.Red:
                            return details.RedScore;
                        case WorldColor.Blue:
                            return details.BlueScore;
                        case WorldColor.Green:
                            return details.GreenScore;
                        default:
                            return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong (like if WvW is resetting)
                logger.Error(ex);
                return -1;
            }
        }

        /// <summary>
        /// Retrieves a list of the objectives in the given match and map
        /// </summary>
        /// <param name="matchId">The match's ID</param>
        /// <param name="mapId">The map's ID</param>
        public IEnumerable<WvWObjective> GetMapObjectives(string matchId, WvWMap map)
        {
            List<WvWObjective> objectives = new List<WvWObjective>();
            try
            {
                var details = GwApi.GetMatchDetails(matchId, true);
                if (details != null)
                {
                    MatchMapType mapType;
                    switch (map)
                    {
                        case WvWMap.BlueBorderlands:
                            mapType = MatchMapType.BlueHome;
                            break;
                        case WvWMap.GreenBorderlands:
                            mapType = MatchMapType.GreenHome;
                            break;
                        case WvWMap.RedBorderlands:
                            mapType = MatchMapType.RedHome;
                            break;
                        case WvWMap.EternalBattlegrounds:
                            mapType = MatchMapType.Center;
                            break;
                        default:
                            mapType = MatchMapType.Center;
                            break;
                    }

                    var mapDetails = details.Maps.FirstOrDefault(m => m.Type == mapType);
                    if (mapDetails != null)
                    {
                        foreach (var objective in mapDetails.Objectives)
                        {
                            var objData = new WvWObjective();

                            objData.ID = objective.Id;
                            objData.MatchId = matchId;
                            objData.Map = map;
                            objData.GuildOwner = objective.OwnerGuildId;

                            var objDetails = this.ObjectivesTable.Objectives.FirstOrDefault(obj => obj.ID == objData.ID);
                            if (objDetails != null)
                            {
                                objData.Type = objDetails.Type;
                                objData.Name = objDetails.Name;
                                objData.FullName = objDetails.FullName;
                                objData.Location = objDetails.Location;
                                objData.MapLocation = objDetails.MapLocation;
                                objData.Points = objDetails.Points;
                            }

                            switch (objective.Owner)
                            {
                                case OwnerColor.Blue:
                                    objData.WorldOwner = WorldColor.Blue;
                                    break;
                                case OwnerColor.Green:
                                    objData.WorldOwner = WorldColor.Green;
                                    break;
                                case OwnerColor.Red:
                                    objData.WorldOwner = WorldColor.Red;
                                    break;
                                default:
                                    objData.WorldOwner = WorldColor.None;
                                    break;
                            }

                            objectives.Add(objData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong (like if WvW is resetting)
                logger.Error(ex);
            }

            return objectives;
        }

        /// <summary>
        /// Retrieves a list of the objectives in the given match for all maps
        /// </summary>
        /// <param name="matchId">The match's ID</param>
        public IEnumerable<WvWObjective> GetAllObjectives(string matchId)
        {
            List<WvWObjective> objectives = new List<WvWObjective>();
            try
            {
                var details = GwApi.GetMatchDetails(matchId, true);
                if (details != null)
                {
                    foreach (var mapDetails in details.Maps)
                    {
                        foreach (var objective in mapDetails.Objectives)
                        {
                            var objData = new WvWObjective();

                            objData.ID = objective.Id;
                            objData.MatchId = matchId;
                            objData.GuildOwner = objective.OwnerGuildId;

                            var objDetails = this.ObjectivesTable.Objectives.FirstOrDefault(obj => obj.ID == objData.ID);
                            if (objDetails != null)
                            {
                                objData.Type = objDetails.Type;
                                objData.Name = objDetails.Name;
                                objData.FullName = objDetails.FullName;
                                objData.Location = objDetails.Location;
                                objData.MapLocation = objDetails.MapLocation;
                                objData.Points = objDetails.Points;
                            }

                            switch (mapDetails.Type)
                            {
                                case MatchMapType.BlueHome:
                                    objData.Map = WvWMap.BlueBorderlands;
                                    break;
                                case MatchMapType.GreenHome:
                                    objData.Map = WvWMap.GreenBorderlands;
                                    break;
                                case MatchMapType.RedHome:
                                    objData.Map = WvWMap.RedBorderlands;
                                    break;
                                case MatchMapType.Center:
                                    objData.Map = WvWMap.EternalBattlegrounds;
                                    break;
                                default:
                                    objData.Map = WvWMap.Unknown;
                                    break;
                            }

                            switch (objective.Owner)
                            {
                                case OwnerColor.Blue:
                                    objData.WorldOwner = WorldColor.Blue;
                                    break;
                                case OwnerColor.Green:
                                    objData.WorldOwner = WorldColor.Green;
                                    break;
                                case OwnerColor.Red:
                                    objData.WorldOwner = WorldColor.Red;
                                    break;
                                default:
                                    objData.WorldOwner = WorldColor.None;
                                    break;
                            }

                            objectives.Add(objData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Don't crash if something goes wrong (like if WvW is resetting)
                logger.Error(ex);
            }

            return objectives;
        }
    }
}
