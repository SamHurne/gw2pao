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

namespace GW2PAO.TS3.Services
{
    public class TeamspeakService : ITeamspeakService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
        /// Collection of clients on the current server
        /// </summary>
        private ConcurrentDictionary<uint, Client> clients = new ConcurrentDictionary<uint, Client>();

        /// <summary>
        /// Collection of channels on the current server
        /// </summary>
        private ConcurrentDictionary<uint, Channel> channels = new ConcurrentDictionary<uint, Channel>();

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
        /// Default constructor
        /// </summary>
        public TeamspeakService()
        {
            this.pollTimer = new Timer(5000);
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
            if (this.QueryRunner != null)
                return;

            this.ConnectionState = ConnectionState.Connecting;

            this.QueryDispatcher = new AsyncTcpDispatcher("localhost", 25639);
            this.QueryDispatcher.BanDetected += QueryDispatcher_BanDetected;
            this.QueryDispatcher.ReadyForSendingCommands += QueryDispatcher_ReadyForSendingCommands;
            this.QueryDispatcher.ServerClosedConnection += QueryDispatcher_ServerClosedConnection;
            this.QueryDispatcher.SocketError += QueryDispatcher_SocketError;
            this.QueryDispatcher.NotificationReceived += QueryDispatcher_NotificationReceived;

            logger.Info("Connecting");
            this.QueryDispatcher.Connect();
        }

        /// <summary>
        /// Disconnects from the Teamspeak Client Query interface
        /// </summary>
        public void Disconnect()
        {
            lock (this.pollLock)
            {
                logger.Info("Disconnecting and cleaning up");

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
                logger.Info("Sending text message: {0}", msg);
                var command = new Command("sendtextmessage targetmode=2 msg=" + this.EncodeString(msg));
                this.QueryRunner.SendCommand(command);
            }
        }

        /// <summary>
        /// Sends a command to change the current channel
        /// </summary>
        /// <param name="channelID"></param>
        public void ChangeChannel(uint channelID)
        {
            if (this.QueryRunner != null)
            {
                logger.Info("Moving client to channel {0}", channelID);
                var command = new Command(string.Format("clientmove cid={0} clid={1}", channelID, this.currentClientID));
                string restul = this.QueryRunner.SendCommand(command);
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
        private void QueryDispatcher_ReadyForSendingCommands(object sender, EventArgs e)
        {
            logger.Info("Ready For Sending Commands");
            this.ConnectionState = ConnectionState.Connected;
            // you can only run commands on the queryrunner when this event has been raised first!
            this.QueryRunner = new QueryRunner(QueryDispatcher);
            this.QueryRunner.Notifications.TalkStatusChanged += Notifications_TalkStatusChanged;

            // Determine who we are
            var whoami = this.QueryRunner.SendWhoAmI();
            this.currentClientID = whoami.ClientId;
            this.currentChannelID = whoami.ChannelId;
            logger.Info("Current Client ID: {0}", this.currentClientID);
            logger.Info("Current Channel ID: {0}", this.currentChannelID);

            if (this.currentClientID != 0 || this.currentChannelID != 0)
            {
                // Determine the current server and channel information
                this.UpdateServerInfo();
                this.UpdateChannelInfo();

                // Send a request for the full list of clients
                logger.Info("Sending request for client list");
                string result = this.QueryRunner.SendCommand(new Command("clientlist"));
                this.AddClients(result);
            }

            // Start handling notifications
            logger.Info("Registering for notifications");
            this.QueryRunner.RegisterForNotifications(ClientNotifyRegisterEvent.Any);

            // Start a timer to send a message every so often, keeping the connection open
            logger.Info("Starting poll timer");
            this.pollTimer.Start();

            // Request the channel list last - this can take a little while...
            if (this.currentClientID != 0 || this.currentChannelID != 0)
            {
                this.InitializeChannelList();
            }
        }

        /// <summary>
        /// Handler for the ServerClosedConnection event
        /// </summary>
        private void QueryDispatcher_ServerClosedConnection(object sender, EventArgs e)
        {
            logger.Info("Server closed connection");

            // Reconnect
            this.Disconnect();
            this.Connect();
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
        private void QueryDispatcher_NotificationReceived(object sender, EventArgs<string> e)
        {
            logger.Trace("Notification: {0}", e.Value.Trim());

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

                    // Also figure out our new server, client, and channel
                    System.Threading.Thread.Sleep(250);
                    var whoami = this.QueryRunner.SendWhoAmI();
                    this.currentClientID = whoami.ClientId;
                    this.currentChannelID = whoami.ChannelId;
                    logger.Trace("New Client ID: {0}", this.currentClientID);
                    logger.Trace("New Channel ID: {0}", this.currentChannelID);

                    this.UpdateServerInfo();
                    this.UpdateChannelInfo();

                    // Reset our channel list
                    foreach (var channel in this.channels.Values)
                    {
                        this.RaiseChannelRemoved(new ChannelEventArgs(channel));
                    }
                    this.channels.Clear();
                    this.InitializeChannelList();
                }
            }
            else if (e.Value.StartsWith(Notifications.ClientMoved))
            {
                // Client moved channel
                if (this.currentClientID == this.ParseUintProperty(e.Value, Properties.ClientID))
                {
                    // The current user moved channel, so update our current channel
                    uint channelId = this.ParseUintProperty(e.Value, Properties.ChannelID, Properties.TargetChannelID);
                    this.currentChannelID = channelId;
                    logger.Trace("New Channel ID: {0}", this.currentChannelID);
                    this.UpdateChannelInfo();
                }
                else
                {
                    // Someone else moved - raise the client entered/exited based on what channel they moved to
                    uint clientId = this.ParseUintProperty(e.Value, Properties.ClientID);
                    uint newChannelId = this.ParseUintProperty(e.Value, Properties.ChannelID, Properties.TargetChannelID);
                    if (this.clients.ContainsKey(clientId))
                    {
                        if (this.clients[clientId].ChannelID != this.currentChannelID && newChannelId == this.currentChannelID)
                        {
                            // Someone joined the channel
                            this.RaiseClientEnteredChannel(new Data.ClientEventArgs(clientId, this.clients[clientId].Name));
                        }
                        else if (this.clients[clientId].ChannelID == this.currentChannelID && newChannelId != this.currentChannelID)
                        {
                            // Someone left the channel
                            this.RaiseClientExitedChannel(new Data.ClientEventArgs(clientId, this.clients[clientId].Name));
                        }
                        this.clients[clientId].ChannelID = newChannelId;
                    }
                }
            }
            else if (e.Value.StartsWith(Notifications.ClientEnterView))
            {
                // Someone joined the server
                this.AddClients(e.Value);

                // If they joined the current channel, raise the client entered channel event
                uint channelId = this.ParseUintProperty(e.Value, Properties.ChannelID, Properties.TargetChannelID);
                if (channelId == this.currentChannelID)
                {
                    // Someone joined the channel
                    uint clientId = this.ParseUintProperty(e.Value, Properties.ClientID);
                    this.RaiseClientEnteredChannel(new Data.ClientEventArgs(clientId, this.clients[clientId].Name));
                }
            }
            else if (e.Value.StartsWith(Notifications.ClientLeftView))
            {
                // Someone left the server
                var clientId = this.ParseUintProperty(e.Value, Properties.ClientID);
                Client client;
                if (this.clients.TryRemove(clientId, out client))
                {
                    if (client.ChannelID == this.currentChannelID)
                    {
                        // They were in our channel, so raise the client left channel event
                        this.RaiseClientExitedChannel(new Data.ClientEventArgs(clientId, client.Name));
                    }
                }
            }
            else if (e.Value.StartsWith(Notifications.ClientUpdated))
            {
                // Someone changed their nickname
                if (e.Value.Contains(Properties.ClientNickname))
                {
                    uint clientId = this.ParseUintProperty(e.Value, Properties.ClientID);
                    if (this.clients.ContainsKey(clientId))
                    {
                        string clientNickname = this.ParseStringProperty(e.Value, true, Properties.ClientNickname);
                        this.clients[clientId].Name = clientNickname;
                    }
                }
            }
            else if (e.Value.StartsWith(Notifications.ChannelList))
            {
                // ???
            }
            else if (e.Value.StartsWith(Notifications.ChannelCreated))
            {
                // Add the channel to our list of channels, raise channel list updated event
                var channelInfo = this.ProcessChannelInformation(e.Value);

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
            else if (e.Value.StartsWith(Notifications.ChannelDeleted))
            {
                // Remove the channel from our list of channels, raise channel list updated event
                var channelInfo = this.ProcessChannelInformation(e.Value);

                Channel removed;
                if (this.channels.TryRemove(channelInfo.ID, out removed))
                {
                    this.RaiseChannelRemoved(new ChannelEventArgs(removed));
                }
            }
            else if (e.Value.StartsWith(Notifications.ChannelEdited))
            {
                // Update the channel in our list of channels, raise channel list updated event
                var channelInfo = this.ProcessChannelInformation(e.Value);

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
            else if (e.Value.StartsWith(Notifications.ChannelMoved))
            {
                // Update the channel in our list of channels, raise channel list updated event
                var channelInfo = this.ProcessChannelInformation(e.Value);

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
                    uint clientId = this.ParseUintProperty(clientString, Properties.ClientID);

                    string clientNickname = this.ParseStringProperty(clientString, true, Properties.ClientNickname);

                    uint channelId = 0;
                    if (clientString.Contains(Properties.ChannelID) || clientString.Contains(Properties.TargetChannelID))
                        channelId = this.ParseUintProperty(clientString, Properties.ChannelID, Properties.TargetChannelID);

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
            logger.Trace("Text message received From {0} ({1}): {2}", clientId, clientNickname, message);
            this.RaiseTextMessageReceived(new Data.TextMessageEventArgs(clientId, clientNickname, this.DecodeString(message)));
        }

        /// <summary>
        /// Sends a request for the current server information and raises the server connected event
        /// </summary>
        private void UpdateServerInfo()
        {
            // Determine the current server information
            string result = this.QueryRunner.SendCommand(new Command("servervariable " + Properties.ServerName + " " + Properties.ServerIP));
            string serverName = this.ParseStringProperty(result, true, Properties.ServerName);
            string serverAddress = this.ParseStringProperty(result, false, Properties.ServerIP);

            logger.Info("New Server Information: name={0} address={1}", serverName, serverAddress);
            this.RaiseNewServerInfo(new NewServerInfoEventArgs(serverName, serverAddress));
        }

        /// <summary>
        /// Sends a request for the current channel and raises the channel switched event
        /// </summary>
        private void UpdateChannelInfo()
        {
            var command = new Command(string.Format("channelvariable {0}={1} {2} {3}", Properties.ChannelID, this.currentChannelID, Properties.ChannelName, Properties.ChannelDescription));
            string result = this.QueryRunner.SendCommand(command);

            // Parse the channel info
            string channelName = this.ParseStringProperty(result, true, Properties.ChannelName);
            string channelDescription = this.ParseStringProperty(result, true, Properties.ChannelDescription);

            logger.Info("New Channel Information: name={0} description={1}", channelName, channelDescription);
            this.RaiseClientChannelChanged(new ChannelEventArgs(new Channel(this.currentChannelID, channelName) { Description = channelDescription }));
        }

        /// <summary>
        /// Updates the full channel list and raises the ChannelListUpdated event when done
        /// </summary>
        private void InitializeChannelList()
        {
            Task.Factory.StartNew(() =>
                {
                    var command = new Command("channellist");
                    string result = this.QueryRunner.SendCommand(command);

                    var channelStrings = result.Split('|');
                    foreach (var channelString in channelStrings)
                    {
                        var channelInfo = this.ProcessChannelInformation(channelString);

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
                });
        }

        /// <summary>
        /// Parses a string for channel information and returns the resulting channel object
        /// </summary>
        /// <param name="channelString">The string to parse</param>
        /// <returns>The resulting channel object</returns>
        private Channel ProcessChannelInformation(string channelString)
        {
            uint id = this.ParseUintProperty(channelString, Properties.ChannelID);

            uint parentId = 0;
            if (channelString.Contains(Properties.ParentChannelID))
                parentId = this.ParseUintProperty(channelString, Properties.ParentChannelID);

            uint order = 0;
            if (channelString.Contains(Properties.ChannelOrder))
                order = this.ParseUintProperty(channelString, Properties.ChannelOrder);

            string name = string.Empty;
            if (channelString.Contains(Properties.ParentChannelID))
                name = this.ParseStringProperty(channelString, true, Properties.ChannelName);

            uint clientsCount = 0;
            if (channelString.Contains(Properties.ChannelClientsCount))
                clientsCount = this.ParseUintProperty(channelString, Properties.ChannelClientsCount);

            Channel channelInfo = new Channel(id, name)
            {
                ParentID = parentId,
                Order = order,
                ClientsCount = clientsCount
            };

            return channelInfo;
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
        /// Parses a uint property out of the given input string
        /// </summary>
        /// <param name="input">the input string to parse</param>
        /// <param name="propertyName">the full list of possible property names to parse the value of. the first one that works is used.</param>
        /// <returns>The parsed value of the property.</returns>
        /// <exception cref="InvalidOperationException">Thrown when none of the given property names results in a valid uint value</exception>
        private uint ParseUintProperty(string input, params string[] propertyNames)
        {
            var properties = input.Split(' ', '\n', '\r');

            foreach (var propertyName in propertyNames)
            {
                string value = properties.FirstOrDefault(id => id.StartsWith(propertyName));
                if (value != null)
                {
                    value = value.Substring(propertyName.Length + 1);
                    return uint.Parse(value);
                }
            }

            throw new InvalidOperationException("Invalid propertyNames for given input string");
        }

        /// <summary>
        /// Parses a uint property out of the given input string
        /// </summary>
        /// <param name="input">the input string to parse</param>
        /// <param name="decodeValue">True to decode the string, else false to return it as-is</param>
        /// <param name="propertyName">the full list of possible property names to parse the value of. the first one that works is used.</param>
        /// <returns>The parsed value of the property.</returns>
        /// <exception cref="InvalidOperationException">Thrown when none of the given property names results in a valid uint value</exception>
        private string ParseStringProperty(string input, bool decodeValue, params string[] propertyNames)
        {
            var properties = input.Split(' ', '\n', '\r');

            foreach (var propertyName in propertyNames)
            {
                string value = properties.FirstOrDefault(id => id.StartsWith(propertyName));
                if (value != null)
                {
                    if (value.Length > propertyName.Length)
                        value = value.Substring(propertyName.Length + 1);
                    else
                        value = string.Empty;

                    if (decodeValue)
                        return this.DecodeString(value);
                    else
                        return value;
                }
            }

            throw new InvalidOperationException("Invalid propertyNames for given input string");
        }

        /// <summary>
        /// Parses out all special characters (such as /s) from the input string
        /// </summary>
        /// <param name="input">String to clean up</param>
        /// <returns>The normal string representation of the input</returns>
        private string DecodeString(string input)
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
