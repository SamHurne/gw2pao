using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Services.Interfaces;
using GwApiNET.Gw2PositionReader;
using NLog;

namespace GW2PAO.API.Services
{
    /// <summary>
    /// Service class for player information.
    /// Makes use of the Gw2 Mumble interface
    /// </summary>
    public class PlayerService : IPlayerService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The GwApiNET Player object
        /// </summary>
        private Player player;

        /// <summary>
        /// Name of the current character
        /// </summary>
        public string CharacterName { get { return this.player.CharacterName; } }

        /// <summary>
        /// True if the player is a commander, else false
        /// </summary>
        public bool IsCommander { get { return this.player.IsCommander; } }

        /// <summary>
        /// The current MapId of the character
        /// </summary>
        public int MapId { get { return (int)this.player.MapId; } }

        /// <summary>
        /// Determines if the mumble link is returning a valid map Id
        /// </summary>
        public bool HasValidMapId { get { return this.MapId != 0; } }

        /// <summary>
        /// The current WorldId of the character
        /// </summary>
        public int WorldId { get { return (int)this.player.WorldId; } }

        /// <summary>
        /// Determines if the mumble link is returning a valid world Id
        /// </summary>
        public bool HasValidWorldId { get { return this.WorldId != 0; } }

        /// <summary>
        /// The profession of the Character
        /// </summary>
        public Profession Profession
        {
            get
            {
                switch (this.player.Profession)
                {
                    case GwApiNET.Profession.Elementalist:
                        return Profession.Elementalist;
                    case GwApiNET.Profession.Engineer:
                        return Profession.Engineer;
                    case GwApiNET.Profession.Guardian:
                        return Profession.Guardian;
                    case GwApiNET.Profession.Mesmer:
                        return Profession.Mesmer;
                    case GwApiNET.Profession.Necromancer:
                        return Profession.Necromancer;
                    case GwApiNET.Profession.Ranger:
                        return Profession.Ranger;
                    case GwApiNET.Profession.Thief:
                        return Profession.Thief;
                    case GwApiNET.Profession.Warrior:
                        return Profession.Warrior;
                    default:
                        return Profession.Unknown;
                }
            }
        }

        /// <summary>
        /// The player's position, in meters
        /// Note: The GwApiNET interface returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point PlayerPosition { get { return new Point(this.player.AvatarPosition.X, this.player.AvatarPosition.Z, this.player.AvatarPosition.Y); } }

        /// <summary>
        /// Unit-vector for the player's direction using a left-handed coordinate system
        /// Note: The GwApiNET interface returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point PlayerDirection { get { return new Point(this.player.AvatarFront.X, this.player.AvatarFront.Z, this.player.AvatarFront.Y); } }

        /// <summary>
        /// Unit-vector for the top of the player using a left-handed coordinate system
        /// Note: The GwApiNET interface returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point PlayerTop { get { return new Point(this.player.AvatarTop.X, this.player.AvatarTop.Z, this.player.AvatarTop.Y); } }

        /// <summary>
        /// The camera's position, in meters
        /// Note: The GwApiNET interface returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point CameraPosition { get { return new Point(this.player.CameraPosition.X, this.player.CameraPosition.Z, this.player.CameraPosition.Y); } }

        /// <summary>
        /// Unit-vector for the camera's direction using a left-handed coordinate system
        /// Note: The GwApiNET interface returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point CameraDirection { get { return new Point(this.player.CameraFront.X, this.player.CameraFront.Z, this.player.CameraFront.Y); } }

        /// <summary>
        /// Unit-vector for the top of the camera using a left-handed coordinate system
        /// Note: The GwApiNET interface returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point CameraTop { get { return new Point(this.player.CameraTop.X, this.player.CameraTop.Z, this.player.CameraTop.Y); } }

        /// <summary>
        /// The IP and Port of the address that the player is connected to
        /// </summary>
        public IPEndPoint ServerAddress { get { return this.player.ServerAddress; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PlayerService()
        {
            logger.Info("Creating PlayerService");
            this.player = new Player();
        }
    }
}
