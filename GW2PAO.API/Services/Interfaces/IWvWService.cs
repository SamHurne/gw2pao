using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GwApiNET;
using NLog;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IWvWService
    {
        /// <summary>
        /// The Worlds table
        /// </summary>
        WorldsTable Worlds { get; }

        /// <summary>
        /// The WvW objectives table
        /// </summary>
        WvWObjectivesTable ObjectivesTable { get; }

        /// <summary>
        /// Loads the worlds and objectives tables and initializes any/all cached information
        /// </summary>
        void LoadTable();

        /// <summary>
        /// Find and returns the Match ID for the given world
        /// </summary>
        /// <param name="worldId">The ID for the world</param>
        /// <returns>The match ID for the given world ID</returns>
        string GetMatchId(int worldId);

        /// <summary>
        /// Retrieves the TeamColor of the given World
        /// </summary>
        /// <param name="worldId">The ID for the world</param>
        /// <returns>The team color for the given world</returns>
        WorldColor GetTeamColor(int worldId);

        /// <summary>
        /// Retrieves the current WvW score for the given world
        /// </summary>
        /// <param name="worldId">The ID for the world</param>
        /// <returns>The current WvW for the given world</returns>
        int GetWorldScore(int worldId);

        /// <summary>
        /// Retrieves the current WvW score for the given world
        /// </summary>
        /// <param name="matchId">The match ID for the world</param>
        /// <param name="worldId">The ID for the world</param>
        /// <returns>The current WvW for the given world</returns>
        int GetWorldScore(string matchId, int worldId);

        /// <summary>
        /// Retrieves a list of the objectives in the given match and map
        /// </summary>
        /// <param name="matchId">The match's ID</param>
        /// <param name="mapId">The map's ID</param>
        IEnumerable<WvWObjective> GetMapObjectives(string matchId, WvWMap map);

        /// <summary>
        /// Retrieves a list of the objectives in the given match for all maps
        /// </summary>
        /// <param name="matchId">The match's ID</param>
        IEnumerable<WvWObjective> GetAllObjectives(string matchId);
    }
}
