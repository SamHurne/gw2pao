using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IPlayerService
    {
        string CharacterName { get; }
        bool IsCommander { get; }
        int MapId { get; }
        bool HasValidMapId { get; }
        int WorldId { get; }
        bool HasValidWorldId { get; }
        Profession Profession { get; }
        Point PlayerPosition { get; }
        Point PlayerDirection { get; }
        Point PlayerTop { get; }
        Point CameraPosition { get; }
        Point CameraDirection { get; }
        Point CameraTop { get; }
        IPEndPoint ServerAddress { get; }
    }
}
