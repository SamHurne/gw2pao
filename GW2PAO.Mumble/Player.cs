using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Mumble.Data;
using GW2PAO.Mumble.Interfaces;

namespace GW2PAO.Mumble
{
    /// <summary>
    /// For now, this is just making use of the GwApiNET mumble interface implementation (it's pretty good and already makes use of caching)
    /// If, in the future, the mumble interface is expanded, this can be re-written to make use of the new interface without much impact on the
    /// rest of the application
    /// </summary>
    public class Player : IPlayer, IDisposable
    {
        /// <summary>
        /// The GwApiNET Player object
        /// </summary>
        private GwApiNET.Gw2PositionReader.Player player;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Player()
        {
            this.player = new GwApiNET.Gw2PositionReader.Player();
        }

        /// <summary>
        /// Unit vector pointing out of the avatar's eyes in meters. Uses left handed
        /// coordinate system.
        /// </summary>
        public Vector AvatarFront
        {
            get
            {
                return new Vector(this.player.AvatarFront.X, this.player.AvatarFront.Y, this.player.AvatarFront.Z);
            }
        }

        /// <summary>
        /// Position of the avatar in meters. Uses left handed coordinate system.
        /// </summary>
        public Vector AvatarPosition
        {
            get
            {
                return new Vector(this.player.AvatarPosition.X, this.player.AvatarPosition.Y, this.player.AvatarPosition.Z);
            }
        }

        /// <summary>
        /// Unit vector pointing out of the top of the avatar's head in meters. Uses
        /// left handed coordinate system.
        /// </summary>
        public Vector AvatarTop
        {
            get
            {
                return new Vector(this.player.AvatarTop.X, this.player.AvatarTop.Y, this.player.AvatarTop.Z);
            }
        }

        /// <summary>
        /// Returns the current client build number.
        /// </summary>
        public uint Build
        {
            get
            {
                return this.player.Build;
            }
        }

        /// <summary>
        /// Unit vector pointing out of the front of the camera in meters. Uses left
        /// handed coordinate system.
        /// </summary>
        public Vector CameraFront
        {
            get
            {
                return new Vector(this.player.CameraFront.X, this.player.CameraFront.Y, this.player.CameraFront.Z);
            }
        }

        /// <summary>
        /// Position of the camera in meters. Uses left handed coordinate system.
        /// </summary>
        public Vector CameraPosition
        {
            get
            {
                return new Vector(this.player.CameraPosition.X, this.player.CameraPosition.Y, this.player.CameraPosition.Z);
            }
        }

        /// <summary>
        /// Unit vector pointing out of the top of the camera in meters. Uses left handed
        /// coordinate system.
        /// </summary>
        public Vector CameraTop
        {
            get
            {
                return new Vector(this.player.CameraTop.X, this.player.CameraTop.Y, this.player.CameraTop.Z);
            }
        }

        /// <summary>
        /// Returns character name of player.
        /// </summary>
        public string CharacterName
        {
            get
            {
                return this.player.CharacterName;
            }
        }

        /// <summary>
        /// Link Description
        /// </summary>
        public string Description
        {
            get
            {
                return this.player.Description;
            }
        }

        /// <summary>
        /// Returns the current map instance number.
        /// </summary>
        public uint Instance
        {
            get
            {
                return this.player.Instance;
            }
        }

        /// <summary>
        /// Returns true if commander is enabled for player character.
        /// </summary>
        public bool IsCommander
        {
            get
            {
                return this.player.IsCommander;
            }
        }

        /// <summary>
        /// Link Name
        /// </summary>
        public string LinkName
        {
            get
            {
                return this.player.LinkName;
            }
        }

        /// <summary>
        /// Returns the Map ID of the current map.
        /// </summary>
        public uint MapId
        {
            get
            {
                return this.player.MapId;
            }
        }

        /// <summary>
        /// Returns the Map Type of the current map.
        /// </summary>
        public uint MapType
        {
            get
            {
                return this.player.MapType;
            }
        }

        /// <summary>
        /// Returns profession
        /// </summary>
        public Profession Profession
        {
            get
            {
                switch (this.player.Profession)
                {
                    case GwApiNET.Profession.Guardian:
                        return Profession.Guardian;
                    case GwApiNET.Profession.Warrior:
                        return Profession.Warrior;
                    case GwApiNET.Profession.Engineer:
                        return Profession.Engineer;
                    case GwApiNET.Profession.Ranger:
                        return Profession.Ranger;
                    case GwApiNET.Profession.Thief:
                        return Profession.Thief;
                    case GwApiNET.Profession.Elementalist:
                        return Profession.Elementalist;
                    case GwApiNET.Profession.Mesmer:
                        return Profession.Mesmer;
                    case GwApiNET.Profession.Necromancer:
                        return Profession.Necromancer;
                    default:
                        return Profession.Unknown;
                }
            }
        }

        /// <summary>
        /// Returns the IPEndPoint of the server.
        /// </summary>
        public IPEndPoint ServerAddress
        {
            get
            {
                return this.player.ServerAddress;
            }
        }

        /// <summary>
        /// Team Color
        /// </summary>
        public TeamColor TeamColor
        {
            get
            {
                switch (this.player.TeamColor)
                {
                    case GwApiNET.TeamColor.None:
                        return TeamColor.None;
                    case GwApiNET.TeamColor.Red:
                        return TeamColor.Red;
                    case GwApiNET.TeamColor.Blue:
                        return TeamColor.Blue;
                    case GwApiNET.TeamColor.Green:
                        return TeamColor.Green;
                    default:
                        return TeamColor.None;
                }
            }
        }

        /// <summary>
        /// Tick Value
        /// </summary>
        public uint Tick
        {
            get
            {
                return this.player.Tick;
            }
        }

        /// <summary>
        /// Link Version
        /// </summary>
        public uint Version
        {
            get
            {
                return this.player.Version;
            }
        }

        /// <summary>
        /// Returns the current World/Shard ID, ex. 1006 = Sorrow's Furnace.
        /// </summary>
        public uint WorldId
        {
            get
            {
                return this.player.WorldId;
            }
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            this.player.Dispose();
        }

        #endregion
    }
}
