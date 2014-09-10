using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Services.Interfaces;
using GW2DotNET;
using GW2DotNET.V1.WorldVersusWorld;
using GW2DotNET.Entities.WorldVersusWorld;

namespace GW2PAO.API.Services
{
    public class WvWService : IWvWService
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
        /// Internal cache of the current WvW matchup
        /// </summary>
        private Matchup currentMatchup;

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
                var wvwMatch = this.GetCurrentMatchup(worldId);
                if (wvwMatch == null)
                    return null;
                else
                    return wvwMatch.MatchId;
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
                var wvwMatch = this.GetCurrentMatchup(worldId);
                if (wvwMatch != null)
                {
                    if (wvwMatch.BlueWorldId == worldId)
                        return WorldColor.Blue;
                    else if (wvwMatch.GreenWorldId == worldId)
                        return WorldColor.Red;
                    else if (wvwMatch.RedWorldId == worldId)
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
                var details = this.service.GetMatchDetails(matchId);
                if (details != null)
                {
                    switch (this.GetTeamColor(worldId))
                    {
                        case WorldColor.Red:
                            return details.Scores.Red;
                        case WorldColor.Blue:
                            return details.Scores.Blue;
                        case WorldColor.Green:
                            return details.Scores.Green;
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
                var matchDetails = this.service.GetMatchDetails(matchId);
                if (matchDetails != null)
                {
                    CompetitiveMap mapDetails = null;
                    switch (map)
                    {
                        case WvWMap.BlueBorderlands:
                            mapDetails = matchDetails.Maps.FirstOrDefault(m => m is BlueBorderlands);
                            break;
                        case WvWMap.GreenBorderlands:
                            mapDetails = matchDetails.Maps.FirstOrDefault(m => m is GreenBorderlands);
                            break;
                        case WvWMap.RedBorderlands:
                            mapDetails = matchDetails.Maps.FirstOrDefault(m => m is RedBorderlands);
                            break;
                        case WvWMap.EternalBattlegrounds:
                            mapDetails = matchDetails.Maps.FirstOrDefault(m => m is EternalBattlegrounds);
                            break;
                        default:
                            break;
                    }

                    if (mapDetails != null)
                    {
                        foreach (var objective in mapDetails.Objectives)
                        {
                            var objData = new WvWObjective();

                            objData.ID = objective.ObjectiveId;
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
                                case TeamColor.Blue:
                                    objData.WorldOwner = WorldColor.Blue;
                                    break;
                                case TeamColor.Green:
                                    objData.WorldOwner = WorldColor.Green;
                                    break;
                                case TeamColor.Red:
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
                var matchDetails = this.service.GetMatchDetails(matchId);
                if (matchDetails != null)
                {
                    foreach (var mapDetails in matchDetails.Maps)
                    {
                        foreach (var objective in mapDetails.Objectives)
                        {
                            var objData = new WvWObjective();

                            objData.ID = objective.ObjectiveId;
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
                                objData.ChatCode = objDetails.ChatCode;
                                objData.Points = objDetails.Points;
                            }

                            if (mapDetails is BlueBorderlands)
                                objData.Map = WvWMap.BlueBorderlands;
                            else if (mapDetails is GreenBorderlands)
                                objData.Map = WvWMap.GreenBorderlands;
                            else if (mapDetails is RedBorderlands)
                                objData.Map = WvWMap.RedBorderlands;
                            else if (mapDetails is EternalBattlegrounds)
                                objData.Map = WvWMap.EternalBattlegrounds;
                            else
                                objData.Map = WvWMap.Unknown;

                            switch (objective.Owner)
                            {
                                case TeamColor.Blue:
                                    objData.WorldOwner = WorldColor.Blue;
                                    break;
                                case TeamColor.Green:
                                    objData.WorldOwner = WorldColor.Green;
                                    break;
                                case TeamColor.Red:
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
        /// Retrieves either the cached matchup details, or requests new matchup details
        /// </summary>
        /// <returns>The current WvW matchup</returns>
        private Matchup GetCurrentMatchup(int worldId)
        {
            if (this.currentMatchup == null
                || (this.currentMatchup.EndTime.CompareTo(DateTimeOffset.UtcNow) < 0))
            {
                // We've never requested the current matchup, or we've passed the end time for the current matchup
                var matches = this.service.GetMatches();
                this.currentMatchup = matches.Values.FirstOrDefault(match => match.BlueWorldId == worldId
                                                                            || match.GreenWorldId == worldId
                                                                            || match.RedWorldId == worldId);
            }

            return this.currentMatchup;
        }
    }
}
