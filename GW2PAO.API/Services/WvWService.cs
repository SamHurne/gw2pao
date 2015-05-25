using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Services.Interfaces;
using GW2NET;
using GW2NET.WorldVersusWorld;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Providers;
using System.ComponentModel.Composition;

namespace GW2PAO.API.Services
{
    [Export(typeof(IWvWService))]
    public class WvWService : IWvWService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The GW2.NET API service objective
        /// </summary>
        private IMatchRepository matchService = GW2.V1.WorldVersusWorld.Matches;

        /// <summary>
        /// String provider that provides objective names
        /// </summary>
        private IStringProvider<int, WvWObjectiveNameEnum> objectiveNamesProvider;

        /// <summary>
        /// Internal cache of the current WvW matchup
        /// </summary>
        private Matchup currentMatchup;

        /// <summary>
        /// Internally cached match ID. If this changes, then the GetMatches() request is sent. See GetCurrentMatchup().
        /// </summary>
        private int cachedWorldID;

        /// <summary>
        /// The collection of worlds
        /// </summary>
        public List<World> Worlds
        {
            get;
            private set;
        }

        /// <summary>
        /// The WvW objectives table
        /// </summary>
        public WvWObjectivesTable ObjectivesTable { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="objectiveNamesProvider">The StringProvider that supplies localized objective names. If null, a default implementation is used</param>
        public WvWService()
        {
            this.objectiveNamesProvider = new WvWObjectiveNamesProvider();
        }

        /// <summary>
        /// Alternate constructor
        /// </summary>
        /// <param name="objectiveNamesProvider">The StringProvider that supplies localized objective names. If null, a default implementation is used</param>
        public WvWService(IStringProvider<int, WvWObjectiveNameEnum> objectiveNamesProvider)
        {
            this.objectiveNamesProvider = objectiveNamesProvider;
        }

        /// <summary>
        /// Loads the WvW objects  and initializes all cached information
        /// </summary>
        public void LoadData()
        {
            try
            {
                logger.Info("Loading worlds via API");
                this.Worlds = new List<World>();
                var worldRepository = GW2.V2.Worlds.ForCurrentUICulture();
                var worlds = worldRepository.FindAll();
                foreach (var world in worlds.Values)
                {
                    this.Worlds.Add(new World()
                        {
                            ID = world.WorldId,
                            Name = world.Name
                        });
                }
            }
            catch (GW2NET.Common.ServiceException ex)
            {
                logger.Error("Failed to load worlds data: ");
                logger.Error(ex);
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
        /// Retrieves a full list of all match IDs for all servers
        /// </summary>
        /// <returns>a full list of all match IDs for all servers</returns>
        public Dictionary<int, string> GetMatchIDs()
        {
            Dictionary<int, string> worldMatchIDs = new Dictionary<int, string>();

            var matches = this.matchService.Discover();
            foreach (var match in matches)
            {
                worldMatchIDs.Add(match.BlueWorldId, match.MatchId);
                worldMatchIDs.Add(match.GreenWorldId, match.MatchId);
                worldMatchIDs.Add(match.RedWorldId, match.MatchId);
            }

            return worldMatchIDs;
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
        /// Retrieves a full list of all team colors for all servers
        /// </summary>
        /// <returns>a full list of all team colors for all servers</returns>
        public Dictionary<int, WorldColor> GetTeamColors()
        {
            Dictionary<int, WorldColor> teamColors = new Dictionary<int, WorldColor>();

            var matches = this.matchService.Discover();
            foreach (var match in matches)
            {
                teamColors.Add(match.BlueWorldId, WorldColor.Blue);
                teamColors.Add(match.GreenWorldId, WorldColor.Green);
                teamColors.Add(match.RedWorldId, WorldColor.Red);
            }

            return teamColors;
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
                var details = this.matchService.Find(new Matchup { MatchId = matchId });
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
                var matchDetails = this.matchService.Find(new Matchup { MatchId = matchId });
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
                                objData.Name = this.objectiveNamesProvider.GetString(objData.ID, WvWObjectiveNameEnum.Short);
                                objData.FullName = this.objectiveNamesProvider.GetString(objData.ID, WvWObjectiveNameEnum.Full);
                                objData.Location = this.objectiveNamesProvider.GetString(objData.ID, WvWObjectiveNameEnum.Cardinal);
                                objData.MapLocation = objDetails.MapLocation;
                            }

                            switch (objData.Type)
                            {
                                case ObjectiveType.Castle:
                                    objData.Points = 35;
                                    break;
                                case ObjectiveType.Keep:
                                    objData.Points = 25;
                                    break;
                                case ObjectiveType.Tower:
                                    objData.Points = 10;
                                    break;
                                case ObjectiveType.Camp:
                                    objData.Points = 5;
                                    break;
                                default:
                                    break;
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
                var matchDetails = this.matchService.Find(new Matchup { MatchId = matchId });
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
                                objData.Name = this.objectiveNamesProvider.GetString(objData.ID, WvWObjectiveNameEnum.Short);
                                objData.FullName = this.objectiveNamesProvider.GetString(objData.ID, WvWObjectiveNameEnum.Full);
                                objData.Location = this.objectiveNamesProvider.GetString(objData.ID, WvWObjectiveNameEnum.Cardinal);
                                objData.MapLocation = objDetails.MapLocation;
                                objData.ChatCode = objDetails.ChatCode;
                            }

                            switch (objData.Type)
                            {
                                case ObjectiveType.Castle:
                                    objData.Points = 35;
                                    break;
                                case ObjectiveType.Keep:
                                    objData.Points = 25;
                                    break;
                                case ObjectiveType.Tower:
                                    objData.Points = 10;
                                    break;
                                case ObjectiveType.Camp:
                                    objData.Points = 5;
                                    break;
                                default:
                                    break;
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
                || (this.cachedWorldID != worldId)
                || (this.currentMatchup.EndTime.CompareTo(DateTimeOffset.UtcNow) < 0))
            {
                // We've never requested the current matchup, or we've passed the end time for the current matchup
                var matches = this.matchService.Discover();
                this.currentMatchup = matches.FirstOrDefault(match => match.BlueWorldId == worldId
                                                                        || match.GreenWorldId == worldId
                                                                        || match.RedWorldId == worldId);
                this.cachedWorldID = worldId;
            }

            return this.currentMatchup;
        }
    }
}
