using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IPlayerService
    {
        /// <summary>
        /// Name of the current character
        /// </summary>
        string CharacterName { get; }

        /// <summary>
        /// True if the player is a commander, else false
        /// </summary>
        bool IsCommander { get; }

        /// <summary>
        /// The current MapId of the character
        /// </summary>
        int MapId { get; }

        /// <summary>
        /// Determines if the mumble link is returning a valid map Id
        /// </summary>
        bool HasValidMapId { get; }

        /// <summary>
        /// The current WorldId of the character
        /// </summary>
        long WorldId { get; }

        /// <summary>
        /// Determines if the mumble link is returning a valid world Id
        /// </summary>
        bool HasValidWorldId { get; }

        /// <summary>
        /// Returns the mumble interface tick value
        /// </summary>
        long Tick { get; }

        /// <summary>
        /// The profession of the Character
        /// </summary>
        Profession Profession { get; }

        /// <summary>
        /// The player's position, in meters
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        Point PlayerPosition { get; }

        /// <summary>
        /// Unit-vector for the player's direction using a left-handed coordinate system
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        Point PlayerDirection { get; }

        /// <summary>
        /// Unit-vector for the top of the player using a left-handed coordinate system
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        Point PlayerTop { get; }

        /// <summary>
        /// The camera's position, in meters
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        Point CameraPosition { get; }

        /// <summary>
        /// Unit-vector for the camera's direction using a left-handed coordinate system
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        Point CameraDirection { get; }

        /// <summary>
        /// The IP and Port of the address that the player is connected to
        /// </summary>
        IPEndPoint ServerAddress { get; }
    }
}
