using GW2PAO.TS3.Data.Enums;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TS3QueryLib.Core;
using TS3QueryLib.Core.Client;
using TS3QueryLib.Core.Client.Entities;
using TS3QueryLib.Core.Client.Notification.EventArgs;
using TS3QueryLib.Core.Client.Responses;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Communication;

namespace GW2PAO.TS3.Services
{
    public class TeamspeakConnection
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
        /// The current connected state
        /// </summary>
        public ConnectionState ConnectionState { get; private set; }

        /// <summary>
        /// Timer used for polling the server, keeping the connection open
        /// </summary>
        private System.Timers.Timer pollTimer;

        /// <summary>
        /// Locking object for interacting with the various dispatchers and query runners
        /// </summary>
        private readonly object connectionLock = new object();

        /// <summary>
        /// Raised when the socket connected successfully and the greeting (TS3) was received.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Raised when the connection is closed, either by the server or by the client
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Event raised when connecting to teamspeak fails
        /// </summary>
        public event EventHandler ConnectionRefused;

        /// <summary>
        /// Event raised when a notification is received from teamspeak
        /// </summary>
        public event EventHandler<string> NotificationReceived;

        /// <summary>
        /// Event raise when the talk status of a client in teamspeak has changed
        /// </summary>
        public event EventHandler<TalkStatusEventArgs> TalkStatusChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TeamspeakConnection()
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
            lock (this.connectionLock)
            {
                // do not connect when already connected or during connection establishing
                if (this.CommandQueryRunner != null || this.EventQueryRunner != null)
                    return;

                this.ConnectionState = ConnectionState.Connecting;

                this.CommandQueryDispatcher = new AsyncTcpDispatcher(TeamspeakConnection.Host, TeamspeakConnection.Port);
                this.CommandQueryDispatcher.ReadyForSendingCommands += CommandQueryDispatcher_ReadyForSendingCommands;
                this.CommandQueryDispatcher.SocketError += QueryDispatcher_SocketError;

                this.EventQueryDispatcher = new AsyncTcpDispatcher(TeamspeakConnection.Host, TeamspeakConnection.Port);
                this.EventQueryDispatcher.BanDetected += QueryDispatcher_BanDetected;
                this.EventQueryDispatcher.ReadyForSendingCommands += EventQueryDispatcher_ReadyForSendingCommands;
                this.EventQueryDispatcher.ServerClosedConnection += EventQueryDispatcher_ServerClosedConnection;
                this.EventQueryDispatcher.SocketError += QueryDispatcher_SocketError;
                this.EventQueryDispatcher.NotificationReceived += EventQueryDispatcher_NotificationReceived;

                logger.Info("Connecting");
                this.CommandQueryDispatcher.Connect();
                this.EventQueryDispatcher.Connect();
            }
        }

        /// <summary>
        /// Disconnects from the Teamspeak Client Query interface
        /// </summary>
        public void Disconnect()
        {
            logger.Debug("Disconnecting and cleaning up");

            lock (this.connectionLock)
            {
                // Stop the poll timer
                this.pollTimer.Stop();

                // QueryRunner disposes the Dispatcher too
                if (this.CommandQueryRunner != null && !this.CommandQueryRunner.IsDisposed)
                {
                    this.CommandQueryDispatcher.SocketError -= QueryDispatcher_SocketError;
                    this.CommandQueryRunner.Dispose();
                }

                if (this.EventQueryRunner != null && !this.EventQueryRunner.IsDisposed)
                {
                    this.EventQueryDispatcher.BanDetected -= QueryDispatcher_BanDetected;
                    this.EventQueryDispatcher.ReadyForSendingCommands -= EventQueryDispatcher_ReadyForSendingCommands;
                    this.EventQueryDispatcher.ServerClosedConnection -= EventQueryDispatcher_ServerClosedConnection;
                    this.EventQueryDispatcher.SocketError -= QueryDispatcher_SocketError;
                    this.EventQueryDispatcher.NotificationReceived -= EventQueryDispatcher_NotificationReceived;
                    this.EventQueryRunner.Notifications.TalkStatusChanged -= EventQueryRunner_TalkStatusChanged;
                    this.EventQueryRunner.Dispose();
                }

                this.CommandQueryDispatcher = null;
                this.CommandQueryRunner = null;
                this.EventQueryDispatcher = null;
                this.EventQueryRunner = null;

                if (this.ConnectionState != ConnectionState.Disconnected)
                {
                    this.ConnectionState = ConnectionState.Disconnected;
                    this.RaiseDisconnected();
                }
            }
        }

        /// <summary>
        /// Sends a command to teamspeak
        /// </summary>
        /// <param name="command">The command to send</param>
        /// <returns>The result returned from teamspeak</returns>
        public string SendCommand(Command command)
        {
            lock (connectionLock)
            {
                if (this.CommandQueryRunner != null)
                {
                    logger.Debug("Sending command: {0}", command);
                    return this.CommandQueryRunner.SendCommand(command);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Sends the WhoAmI command
        /// </summary>
        /// <returns>The response, including information about
       ///  the current server, client, etc, or null if not yet connected</returns>
        public WhoAmIResponse SendWhoAmI()
        {
            lock (connectionLock)
            {
                if (this.CommandQueryRunner != null)
                {
                    logger.Debug("Sending WhoAmI");
                    return this.CommandQueryRunner.SendWhoAmI();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Handler for the BanDetected event
        /// </summary>
        private void QueryDispatcher_BanDetected(object sender, EventArgs<SimpleResponse> e)
        {
            logger.Warn("Ban detected!");
        }

        /// <summary>
        /// Handler for the ReadyForSendingCommands event
        /// </summary>
        private void CommandQueryDispatcher_ReadyForSendingCommands(object sender, EventArgs e)
        {
            lock (connectionLock)
            {
                if (this.CommandQueryDispatcher == null)
                    return;

                logger.Info("CommandQueryDispatcher - Connected!");
                this.ConnectionState = ConnectionState.Connected;

                // you can only run commands on the queryrunner when this event has been raised first!
                this.CommandQueryRunner = new QueryRunner(this.CommandQueryDispatcher);

                this.RaiseConnected();
            }
        }

        /// <summary>
        /// Handler for the ReadyForSendingCommands event
        /// </summary>
        private void EventQueryDispatcher_ReadyForSendingCommands(object sender, EventArgs e)
        {
            lock (connectionLock)
            {
                logger.Info("EventQueryDispatcher - Ready For Sending Commands");
                this.ConnectionState = ConnectionState.Connected;

                // you can only run commands on the queryrunner when this event has been raised first!
                this.EventQueryRunner = new QueryRunner(this.EventQueryDispatcher);
                this.EventQueryRunner.Notifications.TalkStatusChanged += EventQueryRunner_TalkStatusChanged;
                this.EventQueryRunner.RegisterForNotifications(ClientNotifyRegisterEvent.Any);

                // Start a timer to send a message every so often, keeping the connection open
                logger.Info("Starting poll timer");
                this.pollTimer.Start();
            }
        }

        /// <summary>
        /// Handler for the ServerClosedConnection event
        /// </summary>
        private void EventQueryDispatcher_ServerClosedConnection(object sender, EventArgs e)
        {
            logger.Info("Server closed connection");

            // TODO: Is this correct?
            Task.Factory.StartNew(() => this.Disconnect());
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
            Task.Factory.StartNew(() => this.Disconnect());
        }

        /// <summary>
        /// Handler for the NotificationReceived event
        /// </summary>
        private void EventQueryDispatcher_NotificationReceived(object sender, EventArgs<string> e)
        {
            // Handle the notification on a background task
            Task.Factory.StartNew(() =>
            {
                lock (this.connectionLock)
                {
                    this.RaiseNotificationReceived(e.Value);
                }
            });
        }

        /// <summary>
        /// Handler for the TalkStatusChanged event
        /// </summary>
        private void EventQueryRunner_TalkStatusChanged(object sender, TS3QueryLib.Core.Client.Notification.EventArgs.TalkStatusEventArgs e)
        {
            // Handle the notification on a background task
            Task.Factory.StartNew(() =>
            {
                lock (this.connectionLock)
                {
                    this.RaiseTalkStatusChanged(e);
                }
            });
        }

        /// <summary>
        /// Method used to poll the server, keeping the connection open
        /// </summary>
        private void Poll()
        {
            // Do a non-blocking lock
            if (Monitor.TryEnter(this.connectionLock))
            {
                try
                {
                    if (this.CommandQueryRunner != null && !this.CommandQueryRunner.IsDisposed)
                        this.CommandQueryRunner.SendWhoAmI();

                    if (this.EventQueryRunner != null && !this.EventQueryRunner.IsDisposed)
                        this.EventQueryRunner.SendWhoAmI();
                }
                catch (Exception ex)
                {
                    logger.Warn(ex, "Exception thrown from TS Poll thread");
                }
                finally
                {
                    Monitor.Exit(this.connectionLock);
                }
            }
        }

        #region Raise Event Helpers

        /// <summary>
        /// Raises the TalkStatusChanged event
        /// </summary>
        private void RaiseConnected()
        {
            if (this.Connected != null)
                this.Connected(this, new EventArgs());
        }

        /// <summary>
        /// Raises the TalkStatusChanged event
        /// </summary>
        private void RaiseDisconnected()
        {
            if (this.Disconnected != null)
                this.Disconnected(this, new EventArgs());
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
        /// Raises the NotificationReceived event
        /// </summary>
        private void RaiseNotificationReceived(string notification)
        {
            if (this.NotificationReceived != null)
                this.NotificationReceived(this, notification);
        }

        /// <summary>
        /// Raises the TalkStatusChanged event
        /// </summary>
        private void RaiseTalkStatusChanged(TalkStatusEventArgs args)
        {
            if (this.TalkStatusChanged != null)
                this.TalkStatusChanged(this, args);
        }

        #endregion
    }
}
