using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Mumble.Data;

namespace GW2PAO.Mumble.Interfaces
{
    public interface IPlayer
    {
        /// <summary>
        /// Unit vector pointing out of the avatar's eyes in meters. Uses left handed
        /// coordinate system.
        /// </summary>
        Vector AvatarFront { get; }

        /// <summary>
        /// Position of the avatar in meters. Uses left handed coordinate system.
        /// </summary>
        Vector AvatarPosition { get; }

        /// <summary>
        /// Unit vector pointing out of the top of the avatar's head in meters. Uses
        /// left handed coordinate system.
        /// </summary>
        Vector AvatarTop { get; }

        /// <summary>
        /// Returns the current client build number.
        /// </summary>
        uint Build { get; }

        /// <summary>
        /// Unit vector pointing out of the front of the camera in meters. Uses left
        /// handed coordinate system.
        /// </summary>
        Vector CameraFront { get; }

        /// <summary>
        /// Position of the camera in meters. Uses left handed coordinate system.
        /// </summary>
        Vector CameraPosition { get; }

        /// <summary>
        /// Unit vector pointing out of the top of the camera in meters. Uses left handed
        /// coordinate system.
        /// </summary>
        Vector CameraTop { get; }

        /// <summary>
        /// Returns character name of player.
        /// </summary>
        string CharacterName { get; }

        /// <summary>
        /// Link Description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Returns the current map instance number.
        /// </summary>
        uint Instance { get; }

        /// <summary>
        /// Returns true if commander is enabled for player character.
        /// </summary>
        bool IsCommander { get; }

        /// <summary>
        /// Link Name
        /// </summary>
        string LinkName { get; }

        /// <summary>
        /// Returns the Map ID of the current map.
        /// </summary>
        uint MapId { get; }

        /// <summary>
        /// Returns the Map Type of the current map.
        /// </summary>
        uint MapType { get; }

        /// <summary>
        /// Returns profession
        /// </summary>
        Profession Profession { get; }

        /// <summary>
        /// Returns the IPEndPoint of the server.
        /// </summary>
        IPEndPoint ServerAddress { get; }

        /// <summary>
        /// Team Color
        /// </summary>
        TeamColor TeamColor { get; }

        /// <summary>
        /// Tick Value
        /// </summary>
        uint Tick { get; }

        /// <summary>
        /// Link Version
        /// </summary>
        uint Version { get; }

        /// <summary>
        /// Returns the current World/Shard ID, ex. 1006 = Sorrow's Furnace.
        /// </summary>
        uint WorldId { get; }
    }
}
