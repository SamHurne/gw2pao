using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.TS3.Data.Enums;

namespace GW2PAO.TS3.Services.Interfaces
{
    public interface ITeamspeakService
    {
        /// <summary>
        /// The current connected state of the service
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        /// Event raised when connecting to teamspeak fails
        /// </summary>
        event EventHandler ConnectionRefused;

        /// <summary>
        /// Raised when new server information is available (such as when connecting to a new server)
        /// </summary>
        event EventHandler<GW2PAO.TS3.Data.NewServerInfoEventArgs> NewServerInfo;

        /// <summary>
        /// Raised when the TS user changes channel
        /// </summary>
        event EventHandler<GW2PAO.TS3.Data.NewChannelInfoEventArgs> NewChannelInfo;

        /// <summary>
        /// Raised when someone starts or stops talking in TS
        /// </summary>
        event EventHandler<GW2PAO.TS3.Data.TalkStatusEventArgs> TalkStatusChanged;

        /// <summary>
        /// Event raised when a text message is received
        /// </summary>
        event EventHandler<GW2PAO.TS3.Data.TextMessageEventArgs> TextMessageReceived;

        /// <summary>
        /// Raised when someone enters the current channel in TS
        /// </summary>
        event EventHandler<GW2PAO.TS3.Data.ChannelEventArgs> ClientEnteredChannel;

        /// <summary>
        /// Raised when someone leaves the current channel in TS
        /// </summary>
        event EventHandler<GW2PAO.TS3.Data.ChannelEventArgs> ClientExitedChannel;

        /// <summary>
        /// Connects to the Teamspeak Client Query interface
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the Teamspeak Client Query interface
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends a message to the current channel's chat
        /// </summary>
        /// <param name="msg">The message to send</param>
        void SendChannelMessage(string msg);
    }
}
