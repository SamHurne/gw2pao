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
using NLog;
using System.ComponentModel.Composition;
using TS3QueryLib.Core.Common.Entities;
using System.Threading;
using GW2PAO.TS3.Util;

namespace GW2PAO.TS3.Services
{
    [Export(typeof(ITeamspeakService))]
    public class TeamspeakService : ITeamspeakService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const string Host = "localhost";
        private const int Port = 25639;

        public AsyncTcpDispatcher CommandQueryDispatcher { get; private set; }
        public QueryRunner CommandQueryRunner { get; private set; }

        public AsyncTcpDispatcher EventQueryDispatcher { get; private set; }
        public QueryRunner EventQueryRunner { get; private set; }

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
        /// Collection of clients on the current server
        /// </summary>
        private ConcurrentDictionary<uint, Client> clients = new ConcurrentDictionary<uint, Client>();

        /// <summary>
        /// Collection of clients in the current channel
        /// </summary>
        private ConcurrentDictionary<uint, Client> localClients = new ConcurrentDictionary<uint, Client>();

        /// <summary>
        /// Collection of channels on the current server
        /// </summary>
        private ConcurrentDictionary<uint, Channel> channels = new ConcurrentDictionary<uint, Channel>();

        /// <summary>
        /// Timer used for polling the server, keeping the connection open
        /// </summary>
        private System.Timers.Timer pollTimer;

        /// <summary>
        /// Locking object for the poll timer
        /// </summary>
        private readonly object pollLock = new object();

        /// <summary>
        /// Event raised when connecting to teamspeak fails
        /// </summary>
        public event EventHandler ConnectionRefused;

        /// <summary>
        /// Raised when connected to a server in TS
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.NewServerInfoEventArgs> NewServerInfo;

        /// <summary>
        /// Raised when the TS user changes channel
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.ChannelEventArgs> ClientChannelChanged;

        /// <summary>
        /// Raised when a channel is added to the TS channel list
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.ChannelEventArgs> ChannelAdded;

        /// <summary>
        /// Raised when a channel is removed from the TS channel list
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.ChannelEventArgs> ChannelRemoved;

        /// <summary>
        /// Raised when a channel in the TS channel list is updated
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.ChannelEventArgs> ChannelUpdated;

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
        public event EventHandler<GW2PAO.TS3.Data.ClientEventArgs> ClientEnteredChannel;

        /// <summary>
        /// Raised when someone leaves the current channel in TS
        /// </summary>
        public event EventHandler<GW2PAO.TS3.Data.ClientEventArgs> ClientExitedChannel;

        /// <summary>
        /// The channel ID that the user is current in
        /// </summary>
        public uint CurrentChannelID
        {
            get { return this.currentChannelID; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TeamspeakService()
        {
            this.pollTimer = new System.Timers.Timer(5000);
            this.pollTimer.AutoReset = true;
            this.pollTimer.Elapsed += (o, e) => this.Poll();
            logger.Trace("New Teamspeak Service constructed");
        }

        /// <summary>
        /// Connects to the Teamspeak Client Query interface
        /// </summary>
        public void Connect()
        {
            // do not connect when already connected or during connection establishing
            if (this.CommandQueryRunner != null || this.EventQueryRunner != null)
                return;

            this.ConnectionState = ConnectionState.Connecting;

            this.CommandQueryDispatcher = new AsyncTcpDispatcher(TeamspeakService.Host, TeamspeakService.Port);
            this.CommandQueryDispatcher.ReadyForSendingCommands += CommandQueryDispatcher_ReadyForSendingCommands;
            this.CommandQueryDispatcher.SocketError += QueryDispatcher_SocketError;

            this.EventQueryDispatcher = new AsyncTcpDispatcher(TeamspeakService.Host, TeamspeakService.Port);
            this.EventQueryDispatcher.BanDetected += QueryDispatcher_BanDetected;
            this.EventQueryDispatcher.ReadyForSendingCommands += EventQueryDispatcher_ReadyForSendingCommands;
            this.EventQueryDispatcher.ServerClosedConnection += EventQueryDispatcher_ServerClosedConnection;
            this.EventQueryDispatcher.SocketError += QueryDispatcher_SocketError;
            this.EventQueryDispatcher.NotificationReceived += EventQueryDispatcher_NotificationReceived;

            logger.Info("Connecting");
            this.CommandQueryDispatcher.Connect();
            this.EventQueryDispatcher.Connect();
        }

        /// <summary>
        /// Disconnects from the Teamspeak Client Query interface
        /// </summary>
        public void Disconnect()
        {
            lock (this.pollLock)
            {
                logger.Debug("Disconnecting and cleaning up");

                // Stop the poll timer
                this.pollTimer.Stop();
                this.clients.Clear();

                // QueryRunner disposes the Dispatcher too
                if (this.CommandQueryRunner != null)
                    this.CommandQueryRunner.Dispose();
                if (this.EventQueryRunner != null)
                    this.EventQueryRunner.Dispose();

                this.CommandQueryDispatcher = null;
                this.CommandQueryRunner = null;
                this.EventQueryDispatcher = null;
                this.EventQueryRunner = null;
                this.ConnectionState = ConnectionState.Disconnected;
            }
        }

        /// <summary>
        /// Sends a message to the current channel's chat
        /// </summary>
        /// <param name="msg">The message to send</param>
        public void SendChannelMessage(string msg)
        {
            if (this.CommandQueryRunner != null)
            {
                logger.Info("Sending text message: {0}", msg);
                var command = new Command("sendtextmessage targetmode=2");
                command.AddParameter("msg", DecodeUtility.EncodeString(msg));
                logger.Info("Command: {0}", command.ToString());
                this.CommandQueryRunner.SendCommand(command);
            }
        }

        /// <summary>
        /// Sends a command to change the current channel
        /// </summary>
        /// <param name="channelID"></param>
        public void ChangeChannel(uint channelID)
        {
            if (this.CommandQueryRunner != null)
            {
                logger.Info("Moving client to channel {0}", channelID);
                var command = new Command(string.Format("clientmove cid={0} clid={1}", channelID, this.currentClientID));
                string restul = this.CommandQueryRunner.SendCommand(command);
            }
        }

        /// <summary>
        /// Handler for the BanDetected event
        /// </summary>
        private void QueryDispatcher_BanDetected(object sender, EventArgs<SimpleResponse> e)
        {
            logger.Warn("Ban detected!");
            // Force disconnect
            this.Disconnect();
        }

        /// <summary>
        /// Handler for the ReadyForSendingCommands event
        /// </summary>
        private void CommandQueryDispatcher_ReadyForSendingCommands(object sender, EventArgs e)
        {
            logger.Info("Command - Ready For Sending Commands");
            this.ConnectionState = ConnectionState.Connected;

            // you can only run commands on the queryrunner when this event has been raised first!
            this.CommandQueryRunner = new QueryRunner(this.CommandQueryDispatcher);

            // Determine who we are
            var whoami = this.CommandQueryRunner.SendWhoAmI();
            this.currentClientID = whoami.ClientId;
            this.currentChannelID = whoami.ChannelId;
            logger.Info("Current Client ID: {0}", this.currentClientID);
            logger.Info("Current Channel ID: {0}", this.currentChannelID);

            if (this.currentClientID != 0 || this.currentChannelID != 0)
            {
                // Determine the current server and channel information
                this.RequestCurrentServerInfo();
                this.RequestCurrentChannelInfo();
            }

            // Do all of this on a seperate thread so we don't hold up the UI
            Task.Factory.StartNew(() =>
            {
                // Send a request for the full list of clients
                logger.Info("Sending request for client list");
                string result = this.CommandQueryRunner.SendCommand(new Command("clientlist"));
                this.AddClients(result);

                // Request the client and channel lists last - these can take a little while...
                // TODO: This is kind of horrible, look into a better way to make this work better
                if (this.currentClientID != 0 || this.currentChannelID != 0)
                {
                    this.InitializeChannelList();
                }
            });
        }

        /// <summary>
        /// Handler for the ReadyForSendingCommands event
        /// </summary>
        private void EventQueryDispatcher_ReadyForSendingCommands(object sender, EventArgs e)
        {
            logger.Info("Event - Ready For Sending Commands");
            this.ConnectionState = ConnectionState.Connected;

            // you can only run commands on the queryrunner when this event has been raised first!
            this.EventQueryRunner = new QueryRunner(this.EventQueryDispatcher);
            this.EventQueryRunner.Notifications.TalkStatusChanged += Notifications_TalkStatusChanged;

            Task.Factory.StartNew(() =>
            {
                this.EventQueryRunner.RegisterForNotifications(ClientNotifyRegisterEvent.Any);

                // Start a timer to send a message every so often, keeping the connection open
                logger.Info("Starting poll timer");
                this.pollTimer.Start();
            });
        }

        /// <summary>
        /// Handler for the ServerClosedConnection event
        /// </summary>
        private void EventQueryDispatcher_ServerClosedConnection(object sender, EventArgs e)
        {
            logger.Info("Server closed connection");
        }

        /// <summary>
        /// Handler for the SocketError event
        /// </summary>
        private void QueryDispatcher_SocketError(object sender, SocketErrorEventArgs e)
        {
            logger.Warn("SocketError: {0}", e.SocketError);

            // Do not handle connection lost errors because they are already handled by QueryDispatcher_ServerClosedConnection
            if (e.SocketError == SocketError.ConnectionReset)
            {
                return;
            }
            else if (e.SocketError == SocketError.AccessDenied
                    || e.SocketError == SocketError.AddressAlreadyInUse
                    || e.SocketError == SocketError.ConnectionRefused)
            {
                logger.Warn("Connection refused");
                this.RaiseConnectionRefused();
            }

            // Force disconnect
            this.Disconnect();
        }

        /// <summary>
        /// Handler for the NotificationReceived event
        /// </summary>
        private void EventQueryDispatcher_NotificationReceived(object sender, EventArgs<string> e)
        {
            // Handle the notification on a background task
            Task.Factory.StartNew(() =>
                {
                    logger.Trace("Notification: {0}", e.Value.Trim());

                    var notification = e.Value.Trim().Split(' ')[0].ToLower();
                    switch (notification)
                    {
                        case Notifications.TextMessage:
                            this.HandleTextMessage(e.Value);
                            break;
                        case Notifications.ConnectStatusChange:
                            this.HandleConnectionStatusChange(e.Value);
                            break;
                        case Notifications.ClientMoved:
                            this.HandleClientMoved(e.Value);
                            break;
                        case Notifications.ClientEnterView:
                            this.HandleClientEnteredView(e.Value);
                            break;
                        case Notifications.ClientLeftView:
                            this.HandleClientExitedView(e.Value);
                            break;
                        case Notifications.ClientUpdated:
                            this.HandleClientUpdated(e.Value);
                            break;
                        case Notifications.ChannelCreated:
                            this.HandleChannelCreated(e.Value);
                            break;
                        case Notifications.ChannelDeleted:
                            this.HandleChannelDeleted(e.Value);
                            break;
                        case Notifications.ChannelEdited:
                            this.HandleChannelEdited(e.Value);
                            break;
                        case Notifications.ChannelMoved:
                            this.HandleChannelMoved(e.Value);
                            break;
                        default:
                            break;
                    }
                });
        }

        /// <summary>
        /// Handler for the TalkStatusChanged event
        /// </summary>
        private void Notifications_TalkStatusChanged(object sender, TS3QueryLib.Core.Client.Notification.EventArgs.TalkStatusEventArgs e)
        {
            string name = "Unknown";

            if (this.clients.ContainsKey(e.ClientId))
                name = this.clients[e.ClientId].Name;

            logger.Trace("TalkStatusChanged: {0} {1}", name, e.TalkStatus);

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
        /// Parses and handles a text message notification string
        /// </summary>
        /// <param name="notificationString">The text message notification string to parse</param>
        private void HandleTextMessage(string notificationString)
        {
            var notificationProperties = notificationString.Split(' ');
            uint clientId = uint.Parse(notificationProperties.First(id => id.StartsWith(Properties.InvokerID)).Substring(Properties.InvokerID.Length + 1));
            string clientNickname = DecodeUtility.DecodeString(notificationProperties.First(id => id.StartsWith(Properties.InvokerName)).Substring(Properties.InvokerName.Length + 1));
            string message = DecodeUtility.DecodeString(notificationProperties.First(id => id.StartsWith(Properties.Message)).Substring(Properties.Message.Length + 1));

            // Raise the text message received event
            logger.Trace("Text message received From {0} ({1}): {2}", clientId, clientNickname, message);
            this.RaiseTextMessageReceived(new Data.TextMessageEventArgs(clientId, clientNickname, message));
        }

        /// <summary>
        /// Handles the ConnectionStatusChanged TS notification
        /// </summary>
        /// <param name="notificationString">The notification string</param>
        private void HandleConnectionStatusChange(string notificationString)
        {
            // Connection status just changed, if it's "connected", we need to rebuild our client list, so clear it out
            var connectStatusProperties = notificationString.Split(' ');
            var status = connectStatusProperties.First(id => id.StartsWith("status")).Substring("status".Length + 1);
            if (status == "connected")
            {
                // Send a stop for everyone, to make sure clients don't get stuck as "talking"
                foreach (var client in this.clients.Values)
                    this.RaiseTalkStatusChanged(new Data.TalkStatusEventArgs(client.ID, client.Name, TalkStatus.TalkStopped, false));
                this.clients.Clear();

                // Reset our channel list
                foreach (var channel in this.channels.Values)
                {
                    this.RaiseChannelRemoved(new ChannelEventArgs(channel));
                }
                this.channels.Clear();

                // Reset our local clients
                foreach (var client in this.localClients.Values)
                {
                    this.RaiseClientExitedChannel(new ClientEventArgs(client.ID, client.Name));
                }
                this.localClients.Clear();

                var whoami = this.CommandQueryRunner.SendWhoAmI();
                this.currentClientID = whoami.ClientId;
                this.currentChannelID = whoami.ChannelId;
                logger.Trace("New Client ID: {0}", this.currentClientID);
                logger.Trace("New Channel ID: {0}", this.currentChannelID);

                this.RequestCurrentServerInfo();
                this.RequestCurrentChannelInfo();
                this.RequestClientList();
                this.InitializeChannelList();
            }
        }

        /// <summary>
        /// Handles the ClientMoved TS notification
        /// </summary>
        /// <param name="notificationString">The notification string</param>
        private void HandleClientMoved(string notificationString)
        {
            // Client moved channel
            if (this.currentClientID == DecodeUtility.DecodeUIntProperty(notificationString, Properties.ClientID))
            {
                // The current user moved channel, so update our current channel
                uint prevChannelId = this.currentChannelID;
                uint channelId = DecodeUtility.DecodeUIntProperty(notificationString, Properties.ChannelID, Properties.TargetChannelID);
                this.currentChannelID = channelId;
                logger.Trace("New Channel ID: {0}", this.currentChannelID);
                this.RequestCurrentChannelInfo();

                foreach (var client in this.localClients.Values)
                {
                    this.RaiseClientExitedChannel(new ClientEventArgs(client.ID, client.Name));
                }
                this.localClients.Clear();

                // Also raise channel updated for the channel that lost the client and the channel that gained the client
                if (this.channels.ContainsKey(prevChannelId))
                {
                    this.channels[prevChannelId].ClientsCount--;
                    this.RaiseChannelUpdated(new ChannelEventArgs(this.channels[prevChannelId]));
                }

                if (this.channels.ContainsKey(this.currentChannelID))
                {
                    this.channels[this.currentChannelID].ClientsCount++;
                    this.RaiseChannelUpdated(new ChannelEventArgs(this.channels[this.currentChannelID]));
                }

                // Request the client list for the current channel
                Task.Factory.StartNew(() =>
                {
                    string result = this.CommandQueryRunner.SendCommand(new Command("clientlist"));
                    this.AddClients(result);
                });
            }
            else
            {
                // Someone else moved - raise the client entered/exited based on what channel they moved to
                uint clientId = DecodeUtility.DecodeUIntProperty(notificationString, Properties.ClientID);
                uint newChannelId = DecodeUtility.DecodeUIntProperty(notificationString, Properties.ChannelID, Properties.TargetChannelID);
                uint prevChannelId = 0;

                if (this.clients.ContainsKey(clientId))
                {
                    prevChannelId = this.clients[clientId].ChannelID;

                    if (this.localClients.ContainsKey(clientId))
                    {
                        // Someone left the channel
                        Client throwAway;
                        this.localClients.TryRemove(clientId, out throwAway);
                        this.RaiseClientExitedChannel(new Data.ClientEventArgs(clientId, this.clients[clientId].Name));
                    }
                    else if (newChannelId == this.currentChannelID)
                    {
                        // Someone joined the channel
                        this.localClients.AddOrUpdate(clientId, this.clients[clientId], (key, oldValue) => this.clients[clientId]);
                        this.RaiseClientEnteredChannel(new Data.ClientEventArgs(clientId, this.clients[clientId].Name));
                    }

                    this.clients[clientId].ChannelID = newChannelId;
                }
                else
                {
                    this.AddClients(notificationString);
                }

                // Also raise channel updated for the channel that lost the client and the channel that gained the client
                if (this.channels.ContainsKey(prevChannelId))
                {
                    this.channels[prevChannelId].ClientsCount--;
                    this.RaiseChannelUpdated(new ChannelEventArgs(this.channels[prevChannelId]));
                }

                if (this.channels.ContainsKey(newChannelId))
                {
                    this.channels[newChannelId].ClientsCount++;
                    this.RaiseChannelUpdated(new ChannelEventArgs(this.channels[newChannelId]));
                }
            }
        }

        /// <summary>
        /// Handles the ClientEnteredView TS notification
        /// </summary>
        /// <param name="notificationString">The notification string</param>
        private void HandleClientEnteredView(string notificationString)
        {
            // Someone joined the server
            this.AddClients(notificationString);
        }

        /// <summary>
        /// Handles the ClientExitedView TS notification
        /// </summary>
        /// <param name="notificationString">The notification string</param>
        private void HandleClientExitedView(string notificationString)
        {
            // Someone left the server
            var clientId = DecodeUtility.DecodeUIntProperty(notificationString, Properties.ClientID);
            Client client;
            if (this.clients.TryRemove(clientId, out client))
            {
                if (client.ChannelID == this.currentChannelID)
                {
                    // They were in our channel, so raise the client left channel event
                    Client throwAway;
                    this.localClients.TryRemove(clientId, out throwAway);
                    this.RaiseClientExitedChannel(new Data.ClientEventArgs(clientId, client.Name));
                }
            }
        }

        /// <summary>
        /// Handles the ClientUpdated TS notification
        /// </summary>
        /// <param name="notificationString">The notification string</param>
        private void HandleClientUpdated(string notificationString)
        {
            // Someone changed their nickname
            if (notificationString.Contains(Properties.ClientNickname))
            {
                uint clientId = DecodeUtility.DecodeUIntProperty(notificationString, Properties.ClientID);
                if (this.clients.ContainsKey(clientId))
                {
                    string clientNickname = DecodeUtility.DecodeStringProperty(notificationString, true, Properties.ClientNickname);
                    this.clients[clientId].Name = clientNickname;
                }
            }
        }

        /// <summary>
        /// Handles the ChannelCreated TS notification
        /// </summary>
        /// <param name="notificationString">The notification string</param>
        private void HandleChannelCreated(string notificationString)
        {
            // Add the channel to our list of channels, raise channel list updated event
            var channelInfo = Channel.FromChannelString(notificationString);

            if (!this.channels.ContainsKey(channelInfo.ID))
            {
                if (this.channels.TryAdd(channelInfo.ID, channelInfo))
                {
                    this.RaiseChannelAdded(new ChannelEventArgs(channelInfo));
                }
            }
            else
            {
                this.channels.AddOrUpdate(channelInfo.ID, channelInfo, (key, oldValue) => channelInfo);
                this.RaiseChannelUpdated(new ChannelEventArgs(channelInfo));
            }
        }

        /// <summary>
        /// Handles the ChannelDeleted TS notification
        /// </summary>
        /// <param name="notificationString">The notification string</param>
        private void HandleChannelDeleted(string notificationString)
        {
            // Remove the channel from our list of channels, raise channel list updated event
            var channelInfo = Channel.FromChannelString(notificationString);

            Channel removed;
            if (this.channels.TryRemove(channelInfo.ID, out removed))
            {
                this.RaiseChannelRemoved(new ChannelEventArgs(removed));
            }
        }

        /// <summary>
        /// Handles the ChannelEdited TS notification
        /// </summary>
        /// <param name="notificationString">The notification string</param>
        private void HandleChannelEdited(string notificationString)
        {
            // Update the channel in our list of channels, raise channel list updated event
            var channelInfo = Channel.FromChannelString(notificationString);

            if (!this.channels.ContainsKey(channelInfo.ID))
            {
                if (this.channels.TryAdd(channelInfo.ID, channelInfo))
                {
                    this.RaiseChannelAdded(new ChannelEventArgs(channelInfo));
                }
            }
            else
            {
                this.channels.AddOrUpdate(channelInfo.ID, channelInfo, (key, oldValue) => channelInfo);
                this.RaiseChannelUpdated(new ChannelEventArgs(channelInfo));
            }
        }

        /// <summary>
        /// Handles the ChannelMoved TS notification
        /// </summary>
        /// <param name="notificationString">The notification string</param>
        private void HandleChannelMoved(string notificationString)
        {
            // Update the channel in our list of channels, raise channel list updated event
            var channelInfo = Channel.FromChannelString(notificationString);

            if (!this.channels.ContainsKey(channelInfo.ID))
            {
                if (this.channels.TryAdd(channelInfo.ID, channelInfo))
                {
                    this.RaiseChannelAdded(new ChannelEventArgs(channelInfo));
                }
            }
            else
            {
                this.channels.AddOrUpdate(channelInfo.ID, channelInfo, (key, oldValue) => channelInfo);
                this.RaiseChannelUpdated(new ChannelEventArgs(channelInfo));
            }
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
                    uint clientId = DecodeUtility.DecodeUIntProperty(clientString, Properties.ClientID);

                    string clientNickname = DecodeUtility.DecodeStringProperty(clientString, true, Properties.ClientNickname);

                    uint channelId = 0;
                    if (clientString.Contains(Properties.ChannelID) || clientString.Contains(Properties.TargetChannelID))
                        channelId = DecodeUtility.DecodeUIntProperty(clientString, Properties.ChannelID, Properties.TargetChannelID);

                    var client = new Client(clientId, clientNickname, channelId);
                    this.clients.AddOrUpdate(clientId, client, (key, oldValue) => client);

                    // If they joined the current channel, raise the client entered channel event
                    if (client.ChannelID == this.CurrentChannelID)
                    {
                        this.localClients.AddOrUpdate(clientId, this.clients[clientId], (key, oldValue) => this.clients[clientId]);
                        this.RaiseClientEnteredChannel(new Data.ClientEventArgs(clientId, this.clients[clientId].Name));
                    }
                }
            }
        }

        /// <summary>
        /// Sends a request for the current server information and raises the server connected event
        /// </summary>
        private void RequestCurrentServerInfo()
        {
            // Determine the current server information
            string result = this.CommandQueryRunner.SendCommand(new Command("servervariable " + Properties.ServerName + " " + Properties.ServerIP));
            string serverName = DecodeUtility.DecodeStringProperty(result, true, Properties.ServerName);
            string serverAddress = DecodeUtility.DecodeStringProperty(result, false, Properties.ServerIP);

            logger.Info("New Server Information: name={0} address={1}", serverName, serverAddress);
            this.RaiseNewServerInfo(new NewServerInfoEventArgs(serverName, serverAddress));
        }

        /// <summary>
        /// Sends a request for the current channel and raises the channel switched event
        /// </summary>
        private void RequestCurrentChannelInfo()
        {
            var command = new Command(string.Format("channelvariable {0}={1} {2} {3}", Properties.ChannelID, this.currentChannelID, Properties.ChannelName, Properties.ChannelDescription));
            string result = this.CommandQueryRunner.SendCommand(command);

            // Parse the channel info
            string channelName = DecodeUtility.DecodeStringProperty(result, true, Properties.ChannelName);
            string channelDescription = DecodeUtility.DecodeStringProperty(result, true, Properties.ChannelDescription);

            logger.Info("New Channel Information: name={0} description={1}", channelName, channelDescription);
            this.RaiseClientChannelChanged(new ChannelEventArgs(new Channel(this.currentChannelID, channelName) { Description = channelDescription }));
        }

        /// <summary>
        /// Updates the client list for the current channel
        /// </summary>
        private void RequestClientList()
        {
            string result = this.CommandQueryRunner.SendCommand(new Command("clientlist"));
            this.AddClients(result);
        }

        /// <summary>
        /// Updates the full channel list and raises the ChannelListUpdated event when done
        /// </summary>
        private void InitializeChannelList()
        {
            var command = new Command("channellist");
            string result = this.CommandQueryRunner.SendCommand(command);
            if (result != null)
            {
                var channelStrings = result.Split('|');
                foreach (var channelString in channelStrings)
                {
                    var channelInfo = Channel.FromChannelString(channelString);

                    if (!this.channels.ContainsKey(channelInfo.ID))
                    {
                        if (this.channels.TryAdd(channelInfo.ID, channelInfo))
                        {
                            this.RaiseChannelAdded(new ChannelEventArgs(channelInfo));
                        }
                    }
                    else
                    {
                        this.channels.AddOrUpdate(channelInfo.ID, channelInfo, (key, oldValue) => channelInfo);
                        this.RaiseChannelUpdated(new ChannelEventArgs(channelInfo));
                    }
                }
            }
        }

        #region Raise Event Helpers

        /// <summary>
        /// Raises the TalkStatusChanged event
        /// </summary>
        private void RaiseConnectionRefused()
        {
            if (this.ConnectionRefused != null)
                this.ConnectionRefused(this, new EventArgs());
        }

        /// <summary>
        /// Raises the NewServerInfo event
        /// </summary>
        private void RaiseNewServerInfo(GW2PAO.TS3.Data.NewServerInfoEventArgs args)
        {
            if (this.NewServerInfo != null)
                this.NewServerInfo(this, args);
        }

        /// <summary>
        /// Raises the ClientChannelChanged event
        /// </summary>
        private void RaiseClientChannelChanged(GW2PAO.TS3.Data.ChannelEventArgs args)
        {
            if (this.ClientChannelChanged != null)
                this.ClientChannelChanged(this, args);
        }

        /// <summary>
        /// Raises the ChannelAdded event
        /// </summary>
        private void RaiseChannelAdded(GW2PAO.TS3.Data.ChannelEventArgs args)
        {
            if (this.ChannelAdded != null)
                this.ChannelAdded(this, args);
        }

        /// <summary>
        /// Raises the ChannelRemoved event
        /// </summary>
        private void RaiseChannelRemoved(GW2PAO.TS3.Data.ChannelEventArgs args)
        {
            if (this.ChannelRemoved != null)
                this.ChannelRemoved(this, args);
        }

        /// <summary>
        /// Raises the ChannelAdded event
        /// </summary>
        private void RaiseChannelUpdated(GW2PAO.TS3.Data.ChannelEventArgs args)
        {
            if (this.ChannelUpdated != null)
                this.ChannelUpdated(this, args);
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
        private void RaiseClientEnteredChannel(GW2PAO.TS3.Data.ClientEventArgs args)
        {
            if (this.ClientEnteredChannel != null)
                this.ClientEnteredChannel(this, args);
        }

        /// <summary>
        /// Raises the ClientExitedChannel event
        /// </summary>
        private void RaiseClientExitedChannel(GW2PAO.TS3.Data.ClientEventArgs args)
        {
            if (this.ClientExitedChannel != null)
                this.ClientExitedChannel(this, args);
        }

        #endregion

        /// <summary>
        /// Method used to poll the server, keeping the connection open
        /// </summary>
        private void Poll()
        {
            // Do a non-blocking lock
            if (Monitor.TryEnter(this.pollLock))
            {
                try
                {
                    if (this.CommandQueryRunner != null && !this.CommandQueryRunner.IsDisposed)
                    {
                        var whoami = this.CommandQueryRunner.SendWhoAmI();
                        this.currentClientID = whoami.ClientId;
                        this.currentChannelID = whoami.ChannelId;
                    }

                    if (this.EventQueryRunner != null && !this.EventQueryRunner.IsDisposed)
                    {
                        var whoami = this.EventQueryRunner.SendWhoAmI();
                        this.currentClientID = whoami.ClientId;
                        this.currentChannelID = whoami.ChannelId;
                    }
                }
                finally
                {
                    Monitor.Exit(this.pollLock);
                }
            }
        }
    }
}
