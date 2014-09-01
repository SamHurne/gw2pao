using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.TS3.Data.Enums;
using TS3QueryLib.Core;
using TS3QueryLib.Core.Client;
using TS3QueryLib.Core.Client.Entities;
using TS3QueryLib.Core.Client.Notification.EventArgs;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using System.Net.Sockets;
using TS3QueryLib.Core.Communication;
using GW2PAO.TS3.Services.Interfaces;
using GW2PAO.TS3.Constants;
using System.Timers;
using GW2PAO.TS3.Data;

namespace GW2PAO.TS3.Services
{
    public class TeamspeakService : ITeamspeakService
    {
        public AsyncTcpDispatcher QueryDispatcher { get; private set; }
        public QueryRunner QueryRunner { get; private set; }

        /// <summary>
        /// The current connected state of the service
        /// </summary>
        public ConnectionState ConnectionState { get; private set; }

        /// <summary>
        /// The current user's client ID
        /// </summary>
        private uint currentClientID;

        /// <summary>
        /// The current channel ID
        /// </summary>
        private uint currentChannelID;

        /// <summary>
        /// Collection of client nicknames
        /// </summary>
        private ConcurrentDictionary<uint, Client> clients = new ConcurrentDictionary<uint, Client>();

        /// <summary>
        /// Timer used for polling the server, keeping the connection open
        /// </summary>
        private Timer pollTimer;

        /// <summary>
        /// Locking object for the poll timer
        /// </summary>
        private readonly object pollLock = new object();

        /// <summary>
        /// Event raised when connecting to teamspeak fails
        /// </summary>
        public event EventHandler ConnectionRefused;

        /// <summary>
        /// Raised when someone starts or stops talking in TS
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.TalkStatusEventArgs> TalkStatusChanged;

        /// <summary>
        /// Event raised when a text message is received
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.TextMessageEventArgs> TextMessageReceived;

        /// <summary>
        /// Raised when someone enters the current channel in TS
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.ChannelEventArgs> ClientEnteredChannel;

        /// <summary>
        /// Raised when someone leaves the current channel in TS
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.ChannelEventArgs> ClientExitedChannel;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TeamspeakService()
        {
            this.pollTimer = new Timer(5000);
            this.pollTimer.AutoReset = true;
            this.pollTimer.Elapsed += (o, e) => this.Poll();
        }

        /// <summary>
        /// Connects to the Teamspeak Client Query interface
        /// </summary>
        public void Connect()
        {
            // do not connect when already connected or during connection establishing
            if (this.QueryRunner != null)
                return;

            this.ConnectionState = ConnectionState.Connecting;

            this.QueryDispatcher = new AsyncTcpDispatcher("localhost", 25639);
            this.QueryDispatcher.BanDetected += QueryDispatcher_BanDetected;
            this.QueryDispatcher.ReadyForSendingCommands += QueryDispatcher_ReadyForSendingCommands;
            this.QueryDispatcher.ServerClosedConnection += QueryDispatcher_ServerClosedConnection;
            this.QueryDispatcher.SocketError += QueryDispatcher_SocketError;
            this.QueryDispatcher.NotificationReceived += QueryDispatcher_NotificationReceived;

            this.QueryDispatcher.Connect();
        }

        /// <summary>
        /// Disconnects from the Teamspeak Client Query interface
        /// </summary>
        public void Disconnect()
        {
            lock (this.pollLock)
            {
                // Stop the poll timer
                this.pollTimer.Stop();
                this.clients.Clear();

                // QueryRunner disposes the Dispatcher too
                if (this.QueryRunner != null)
                    this.QueryRunner.Dispose();

                this.QueryDispatcher = null;
                this.QueryRunner = null;
                this.ConnectionState = ConnectionState.Disconnected;
            }
        }

        /// <summary>
        /// Sends a message to the current channel's chat
        /// </summary>
        /// <param name="msg">The message to send</param>
        public void SendChannelMessage(string msg)
        {
            if (this.QueryRunner != null)
            {
                var command = new Command("sendtextmessage targetmode=2 msg=" + this.EncodeString(msg));
                this.QueryRunner.SendCommand(command);
            }
        }

        /// <summary>
        /// Handler for the BanDetected event
        /// </summary>
        private void QueryDispatcher_BanDetected(object sender, EventArgs<SimpleResponse> e)
        {
            System.Diagnostics.Debug.WriteLine("BanDetected");
            // Force disconnect
            this.Disconnect();
        }

        /// <summary>
        /// Handler for the ReadyForSendingCommands event
        /// </summary>
        private void QueryDispatcher_ReadyForSendingCommands(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ReadyForSendingCommands");
            this.ConnectionState = ConnectionState.Connected;
            // you can only run commands on the queryrunner when this event has been raised first!
            this.QueryRunner = new QueryRunner(QueryDispatcher);
            this.QueryRunner.Notifications.TalkStatusChanged += Notifications_TalkStatusChanged;

            // Determine who we are
            var whoami = this.QueryRunner.SendWhoAmI();
            this.currentClientID = whoami.ClientId;
            this.currentChannelID = whoami.ChannelId;
            System.Diagnostics.Debug.WriteLine("Current Client ID: " + currentClientID);
            System.Diagnostics.Debug.WriteLine("Current Channel ID: " + currentChannelID);

            // Send a request for the full list of clients
            string result = this.QueryRunner.SendCommand(new Command("clientlist"));
            this.AddClients(result);

            // Start handling notifications
            this.QueryRunner.RegisterForNotifications(ClientNotifyRegisterEvent.Any);

            // Start a timer to send a message every so often, keeping the connection open
            this.pollTimer.Start();
        }

        /// <summary>
        /// Handler for the ServerClosedConnection event
        /// </summary>
        private void QueryDispatcher_ServerClosedConnection(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ServerClosedConnection");

            // Reconnect
            this.Disconnect();
            this.Connect();
        }

        /// <summary>
        /// Handler for the SocketError event
        /// </summary>
        private void QueryDispatcher_SocketError(object sender, SocketErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SocketError: " + e.SocketError);

            // Do not handle connection lost errors because they are already handled by QueryDispatcher_ServerClosedConnection
            if (e.SocketError == SocketError.ConnectionReset)
            {
                return;
            }
            else if (e.SocketError == SocketError.AccessDenied
                    || e.SocketError == SocketError.AddressAlreadyInUse
                    || e.SocketError == SocketError.ConnectionRefused)
            {
                this.RaiseConnectionRefused();
            }

            // Force disconnect
            this.Disconnect();
        }

        /// <summary>
        /// Handler for the NotificationReceived event
        /// </summary>
        private void QueryDispatcher_NotificationReceived(object sender, EventArgs<string> e)
        {
            System.Diagnostics.Debug.WriteLine("Notification: " + e.Value.Trim());

            if (e.Value.StartsWith(Notifications.TextMessage))
            {
                // Recieved a chat message
                this.HandleTextMessage(e.Value);
            }
            else if (e.Value.StartsWith(Notifications.ConnectStatusChange))
            {
                // Connection status just changed, if it's "connected", we need to rebuild our client list, so clear it out
                var connectStatusProperties = e.Value.Split(' ');
                var status = connectStatusProperties.First(id => id.StartsWith("status")).Substring("status".Length + 1);
                if (status == "connected")
                {
                    // Send a stop for everyone, to make sure clients don't get stuck as "talking"
                    foreach (var client in this.clients.Values)
                        this.RaiseTalkStatusChanged(new Data.TalkStatusEventArgs(client.ID, client.Name, TalkStatus.TalkStopped, false));
                    this.clients.Clear();

                    // Also figure out our new client and channel
                    var whoami = this.QueryRunner.SendWhoAmI();
                    this.currentClientID = whoami.ClientId;
                    this.currentChannelID = whoami.ChannelId;
                    System.Diagnostics.Debug.WriteLine("New Client ID: " + currentClientID);
                    System.Diagnostics.Debug.WriteLine("New Channel ID: " + currentChannelID);
                }
            }
            else if (e.Value.StartsWith(Notifications.ClientMoved))
            {
                // Client moved channel
                if (this.currentClientID == this.ParseClientID(e.Value))
                {
                    // The current user moved channel, so update our current channel
                    uint channelId = this.ParseChannelID(e.Value);
                    this.currentChannelID = channelId;
                    System.Diagnostics.Debug.WriteLine("New Channel ID: " + currentChannelID);
                }
                else
                {
                    // Someone else moved - raise the client entered/exited based on what channel they moved to
                    uint clientId = this.ParseClientID(e.Value);
                    uint newChannelId = this.ParseChannelID(e.Value);
                    if (this.clients[clientId].ChannelID != this.currentChannelID && newChannelId == this.currentChannelID)
                    {
                        // Someone joined the channel
                        this.RaiseClientEnteredChannel(new Data.ChannelEventArgs(clientId, this.clients[clientId].Name));
                    }
                    else if (this.clients[clientId].ChannelID == this.currentChannelID && newChannelId != this.currentChannelID)
                    {
                        // Someone left the channel
                        this.RaiseClientExitedChannel(new Data.ChannelEventArgs(clientId, this.clients[clientId].Name));
                    }
                    this.clients[clientId].ChannelID = newChannelId;
                }
            }
            else if (e.Value.StartsWith(Notifications.ClientEnterView))
            {
                // Someone joined the server
                this.AddClients(e.Value);

                // If they joined the current channel, raise the client entered channel event
                uint channelId = this.ParseChannelID(e.Value);
                if (channelId == this.currentChannelID)
                {
                    // Someone joined the channel
                    uint clientId = this.ParseClientID(e.Value);
                    this.RaiseClientEnteredChannel(new Data.ChannelEventArgs(clientId, this.clients[clientId].Name));
                }
            }
            else if (e.Value.StartsWith(Notifications.ClientLeftView))
            {
                // Someone left the server
                var clientId = this.ParseClientID(e.Value);
                Client client;
                if (this.clients.TryRemove(clientId, out client))
                {
                    if (client.ChannelID == this.currentChannelID)
                    {
                        // They were in our channel, so raise the client left channel event
                        this.RaiseClientExitedChannel(new Data.ChannelEventArgs(clientId, client.Name));
                    }
                }
            }
            else if (e.Value.StartsWith(Notifications.ClientUpdated))
            {
                // Someone changed their nickname
                if (e.Value.Contains(Properties.ClientNickname))
                {
                    uint clientId = this.ParseClientID(e.Value);
                    if (this.clients.ContainsKey(clientId))
                    {
                        string clientNickname = this.ParseNickname(e.Value);
                        this.clients[clientId].Name = clientNickname;
                    }
                }
            }
        }

        /// <summary>
        /// Handler for the TalkStatusChanged event
        /// </summary>
        private void Notifications_TalkStatusChanged(object sender, TS3QueryLib.Core.Client.Notification.EventArgs.TalkStatusEventArgs e)
        {
            string name = "Unknown";

            if (this.clients.ContainsKey(e.ClientId))
                name = this.clients[e.ClientId].Name;

            System.Diagnostics.Debug.WriteLine("TalkStatusChanged: " + name + " " + e.TalkStatus);

            TalkStatus internalStatus = TalkStatus.Unknown;
            switch (e.TalkStatus)
            {
                case TS3QueryLib.Core.Client.Notification.Enums.TalkStatus.TalkStarted:
                    internalStatus = TalkStatus.TalkStarted;
                    break;
                case TS3QueryLib.Core.Client.Notification.Enums.TalkStatus.TalkFinished:
                    internalStatus = TalkStatus.TalkStopped;
                    break;
                default:
                    break;
            }

            this.RaiseTalkStatusChanged(new Data.TalkStatusEventArgs(e.ClientId, name, internalStatus, e.IsWisper));
        }

        /// <summary>
        /// Parses and handles a "clientlist" response and updates the dictionary of client ID/nicknames
        /// </summary>
        /// <param name="clientList">The client list response</param>
        private void AddClients(string clientList)
        {
            var clientStrings = clientList.Split('|');
            foreach (var clientString in clientStrings)
            {
                if (clientString.Contains(Properties.ClientID) && clientString.Contains(Properties.ClientNickname))
                {
                    uint clientId = this.ParseClientID(clientString);

                    string clientNickname = this.ParseNickname(clientString);

                    uint channelId = 0;
                    if (clientString.Contains(Properties.ChannelID) || clientString.Contains(Properties.TargetChannelID))
                        channelId = this.ParseChannelID(clientString);

                    var client = new Client(clientId, clientNickname, channelId);
                    this.clients.AddOrUpdate(clientId, client, (key, oldValue) => client);
                }
            }
        }

        /// <summary>
        /// Parses and handles a text message notification string
        /// </summary>
        /// <param name="notificationString">The text message notification string to parse</param>
        private void HandleTextMessage(string notificationString)
        {
            var notificationProperties = notificationString.Split(' ');
            uint clientId = uint.Parse(notificationProperties.First(id => id.StartsWith(Properties.InvokerID)).Substring(Properties.InvokerID.Length + 1));
            string clientNickname = notificationProperties.First(id => id.StartsWith(Properties.InvokerName)).Substring(Properties.InvokerName.Length + 1);
            string message = notificationProperties.First(id => id.StartsWith(Properties.Message)).Substring(Properties.Message.Length + 1);

            // Raise the text message received event
            this.RaiseTextMessageReceived(new Data.TextMessageEventArgs(clientId, clientNickname, this.ParseString(message)));
        }

        /// <summary>
        /// Raises the TalkStatusChanged event
        /// </summary>
        private void RaiseConnectionRefused()
        {
            if (this.ConnectionRefused != null)
                this.ConnectionRefused(this, new EventArgs());
        }

        /// <summary>
        /// Raises the TalkStatusChanged event
        /// </summary>
        private void RaiseTalkStatusChanged(GW2PAO.TS3.Data.TalkStatusEventArgs args)
        {
            if (this.TalkStatusChanged != null)
                this.TalkStatusChanged(this, args);
        }

        /// <summary>
        /// Raises the TextMessageReceived event
        /// </summary>
        private void RaiseTextMessageReceived(GW2PAO.TS3.Data.TextMessageEventArgs args)
        {
            if (this.TextMessageReceived != null)
                this.TextMessageReceived(this, args);
        }

        /// <summary>
        /// Raises the ClientEnteredChannel event
        /// </summary>
        private void RaiseClientEnteredChannel(GW2PAO.TS3.Data.ChannelEventArgs args)
        {
            if (this.ClientEnteredChannel != null)
                this.ClientEnteredChannel(this, args);
        }

        /// <summary>
        /// Raises the ClientExitedChannel event
        /// </summary>
        private void RaiseClientExitedChannel(GW2PAO.TS3.Data.ChannelEventArgs args)
        {
            if (this.ClientExitedChannel != null)
                this.ClientExitedChannel(this, args);
        }

        /// <summary>
        /// Method used to poll the server, keeping the connection open
        /// </summary>
        private void Poll()
        {
            lock (this.pollLock)
            {
                if (this.QueryRunner != null && !this.QueryRunner.IsDisposed)
                {
                    var whoami = this.QueryRunner.SendWhoAmI();
                    this.currentClientID = whoami.ClientId;
                    this.currentChannelID = whoami.ChannelId;
                }
            }
        }

        /// <summary>
        /// Parses out the value of clid from an input string
        /// </summary>
        /// <param name="input">The input string to parse</param>
        /// <returns>The parsed client id</returns>
        private uint ParseClientID(string input)
        {
            var clientProperties = input.Split(' ', '\n', '\r');
            return uint.Parse(clientProperties.First(id => id.StartsWith(Properties.ClientID)).Substring(Properties.ClientID.Length + 1));
        }

        /// <summary>
        /// Parses out the value of client nickname from an input string
        /// </summary>
        /// <param name="input">The input string to parse</param>
        /// <returns>the parsed nickname</returns>
        private string ParseNickname(string input)
        {
            var clientProperties = input.Split(' ', '\n', '\r');
            string clientNickname = clientProperties.First(id => id.StartsWith(Properties.ClientNickname)).Substring(Properties.ClientNickname.Length + 1);
            return this.ParseString(clientNickname);
        }

        /// <summary>
        /// Parses out the value of channel ID from an input string
        /// </summary>
        /// <param name="input">The input string to parse</param>
        /// <returns>the parsed channel ID</returns>
        private uint ParseChannelID(string input)
        {
            var clientProperties = input.Split(' ', '\n', '\r');

            // There are two possible properties that contain the channel ID
            string clientNickname = clientProperties.FirstOrDefault(id => id.StartsWith(Properties.ChannelID));
            if (clientNickname != null)
            {
                clientNickname = clientNickname.Substring(Properties.ChannelID.Length + 1);
            }
            else
            {
                clientNickname = clientProperties.First(id => id.StartsWith(Properties.TargetChannelID)).Substring(Properties.TargetChannelID.Length + 1);
            }

            return uint.Parse(clientNickname);
        }

        /// <summary>
        /// Parses out all special characters (such as /s) from the input string
        /// </summary>
        /// <param name="input">String to clean up</param>
        /// <returns>The normal string representation of the input</returns>
        private string ParseString(string input)
        {
            string output = input.Replace(@"\s", " "); // spaces come through as \s
            output = output.Replace(@"\", string.Empty); // additional '\' characters come through as well
            return output;
        }

        /// <summary>
        /// Encodes special characters (such as ' ') using the input string
        /// </summary>
        /// <param name="input">String to encode</param>
        /// <returns>The encoded string representation of the input</returns>
        private string EncodeString(string input)
        {
            string output = input.Replace(@"\", "\\");
            output = output.Replace(" ", @"\s");
            return output;
        }
    }
}
