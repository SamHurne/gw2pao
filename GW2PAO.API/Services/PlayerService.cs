using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GW2NET.MumbleLink;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using NLog;

namespace GW2PAO.API.Services
{
    /// <summary>
    /// Service class for player information.
    /// Makes use of the Gw2 Mumble interface
    /// </summary>
    [Export(typeof(IPlayerService))]
    public class PlayerService : IPlayerService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Name of the current character
        /// </summary>
        public string CharacterName
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data != null)
                        return data.Identity.Name;
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// True if the player is a commander, else false
        /// </summary>
        public bool IsCommander
        {
            get 
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data != null)
                        return data.Identity.Commander;
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// The current MapId of the character
        /// </summary>
        public int MapId
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data != null)
                        return data.Context.MapId;
                    else
                        return 0;
                }
            }
        }

        /// <summary>
        /// Determines if the mumble link is returning a valid map Id
        /// </summary>
        public bool HasValidMapId
        {
            get
            {
                return this.MapId != 0;
            }
        }

        /// <summary>
        /// The current WorldId of the character
        /// </summary>
        public long WorldId
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data != null)
                        return data.Identity.WorldId;
                    else
                        return 0;
                }
            }
        }

        /// <summary>
        /// Determines if the mumble link is returning a valid world Id
        /// </summary>
        public bool HasValidWorldId
        {
            get
            {
                return this.WorldId != 0;
            }
        }

        /// <summary>
        /// Returns the mumble interface tick value
        /// </summary>
        public long Tick
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data != null)
                        return data.UiTick;
                    else
                        return 0;
                }
            }
        }

        /// <summary>
        /// The profession of the Character
        /// </summary>
        public Profession Profession
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data == null)
                        return Profession.Unknown;

                    switch (data.Identity.Profession)
                    {
                        case GW2NET.Common.Profession.Elementalist:
                            return Profession.Elementalist;
                        case GW2NET.Common.Profession.Engineer:
                            return Profession.Engineer;
                        case GW2NET.Common.Profession.Guardian:
                            return Profession.Guardian;
                        case GW2NET.Common.Profession.Mesmer:
                            return Profession.Mesmer;
                        case GW2NET.Common.Profession.Necromancer:
                            return Profession.Necromancer;
                        case GW2NET.Common.Profession.Ranger:
                            return Profession.Ranger;
                        case GW2NET.Common.Profession.Thief:
                            return Profession.Thief;
                        case GW2NET.Common.Profession.Warrior:
                            return Profession.Warrior;
                        default:
                            return Profession.Unknown;
                    }
                }
            }
        }

        /// <summary>
        /// The player's position, in meters
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point PlayerPosition
        { 
            get 
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data == null)
                        return null;

                    return new Point(data.AvatarPosition.X, data.AvatarPosition.Z, data.AvatarPosition.Y);
                }
            } 
        }

        /// <summary>
        /// Unit-vector for the player's direction using a left-handed coordinate system
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point PlayerDirection
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data == null)
                        return null;

                    return new Point(data.AvatarFront.X, data.AvatarFront.Z, data.AvatarFront.Y);
                }
            }
        }

        /// <summary>
        /// Unit-vector for the top of the player using a left-handed coordinate system
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point PlayerTop
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data == null)
                        return null;

                    return new Point(data.AvatarTop.X, data.AvatarTop.Z, data.AvatarTop.Y);
                }
            }
        }

        /// <summary>
        /// The camera's position, in meters
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point CameraPosition
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data == null)
                        return null;

                    return new Point(data.CameraPosition.X, data.CameraPosition.Z, data.CameraPosition.Y);
                }
            }
        }

        /// <summary>
        /// Unit-vector for the camera's direction using a left-handed coordinate system
        /// Note: GW2 returns the Y and Z reversed, so a correction is made here.
        /// </summary>
        public Point CameraDirection
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data == null)
                        return null;

                    return new Point(data.CameraFront.X, data.CameraFront.Z, data.CameraFront.Y);
                }
            }
        }

        /// <summary>
        /// The IP and Port of the address that the player is connected to
        /// </summary>
        public IPEndPoint ServerAddress
        {
            get
            {
                using (var mumbleLink = new MumbleLinkFile())
                {
                    var data = mumbleLink.Read();
                    if (data != null)
                        return data.Context.ServerAddress;
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PlayerService()
        {
            logger.Info("Creating PlayerService");
        }
    }
}
