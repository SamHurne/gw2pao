using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.PresentationCore;
using GW2PAO.TS3.Services.Interfaces;
using GW2PAO.Utility;
using NLog;

namespace GW2PAO.ViewModels.Teamspeak
{
    public class TeamspeakViewModel : NotifyPropertyChangedBase
    {
        private string messageText;

        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// True if the viewmodel is shutting down the connection, else false
        /// </summary>
        private bool isShuttingDown;

        /// <summary>
        /// The teamspeak service
        /// </summary>
        public ITeamspeakService TeamspeakService { get; private set; }

        /// <summary>
        /// The text to send in a message
        /// </summary>
        public string MessageText
        {
            get { return this.messageText; }
            set { this.SetField(ref this.messageText, value); }
        }

        /// <summary>
        /// Collection of client notifications (speaking users, messages, users entering, etc)
        /// </summary>
        public ObservableCollection<TSNotificationViewModel> Notifications { get; private set; }

        /// <summary>
        /// Command to reset all hidden objectives
        /// </summary>
        public DelegateCommand SendMessageCommand { get { return new DelegateCommand(this.SendMessage); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TeamspeakViewModel(ITeamspeakService teamspeakService)
        {
            this.isShuttingDown = false;
            this.Notifications = new ObservableCollection<TSNotificationViewModel>();

            this.TeamspeakService = teamspeakService;
            this.TeamspeakService.ConnectionRefused += TeamspeakService_ConnectionRefused;
            this.TeamspeakService.TalkStatusChanged += TeamspeakService_TalkStatusChanged;
            this.TeamspeakService.TextMessageReceived += TeamspeakService_TextMessageReceived;
            this.TeamspeakService.ClientEnteredChannel += TeamspeakService_ClientEnteredChannel;
            this.TeamspeakService.ClientExitedChannel += TeamspeakService_ClientExitedChannel;
            this.TeamspeakService.Connect();
        }

        /// <summary>
        /// Shutsdown the teamspeak view model, including connection to the Teamspeak Application
        /// </summary>
        public void Shutdown()
        {
            this.isShuttingDown = true;
            this.TeamspeakService.Disconnect();
        }

        /// <summary>
        /// Sends a message to the teamspeak channel's chat
        /// Uses the value of the MessageText property
        /// </summary>
        private void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(MessageText))
            {
                this.TeamspeakService.SendChannelMessage(this.MessageText);
                this.MessageText = string.Empty;
            }
        }

        /// <summary>
        /// Connection was refused, meaning teamspeak is not running
        /// </summary>
        private void TeamspeakService_ConnectionRefused(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
                {
                    this.TeamspeakService.ConnectionRefused -= TeamspeakService_ConnectionRefused;

                    var cannotConnectNotification = new TSNotificationViewModel(0, "Please start Teamspeak", TSNotificationType.CannotConnect);
                    Threading.InvokeOnUI(() => this.Notifications.Add(cannotConnectNotification));

                    // Start a loop attempting to connect
                    while (!this.isShuttingDown && this.TeamspeakService.ConnectionState != TS3.Data.Enums.ConnectionState.Connected)
                    {
                        Threading.InvokeOnUI(() => this.TeamspeakService.Connect());
                        Thread.Sleep(5000); // attempt to connect once every 5 seconds
                    }

                    if (!this.isShuttingDown)
                    {
                        this.TeamspeakService.ConnectionRefused += TeamspeakService_ConnectionRefused;
                        Threading.InvokeOnUI(() => this.Notifications.Remove(cannotConnectNotification));
                    }
                });
        }

        /// <summary>
        /// Handles the talk status changed event
        /// </summary>
        private void TeamspeakService_TalkStatusChanged(object sender, TS3.Data.TalkStatusEventArgs e)
        {
            var speachNotification = new TSNotificationViewModel(e.ClientID, e.ClientName, TSNotificationType.Speech);
            switch (e.Status)
            {
                case TS3.Data.Enums.TalkStatus.TalkStarted:
                    // Add to our collection of speaking users
                    Threading.BeginInvokeOnUI(() => this.Notifications.Add(speachNotification));
                    break;
                case TS3.Data.Enums.TalkStatus.TalkStopped:
                    // Remove from our collection of speaking users
                    var vm = this.Notifications.FirstOrDefault(notification => notification.Equals(speachNotification));
                    if (vm != null)
                        Threading.BeginInvokeOnUI(() => this.Notifications.Remove(vm));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the text message received event
        /// </summary>
        private void TeamspeakService_TextMessageReceived(object sender, TS3.Data.TextMessageEventArgs e)
        {
            Task.Factory.StartNew(() =>
                {
                    var messageNotification = new TSNotificationViewModel(e.ClientID, e.ClientName, TSNotificationType.Text, e.Message);
                    Threading.InvokeOnUI(() => this.Notifications.Add(messageNotification));
                    Thread.Sleep(10000); // Let text messages stay for 10 seconds
                    Threading.InvokeOnUI(() => this.Notifications.Remove(messageNotification));
                });
        }

        /// <summary>
        /// Handler for the Client Entered Channel event
        /// </summary>
        private void TeamspeakService_ClientEnteredChannel(object sender, TS3.Data.ChannelEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var notification = new TSNotificationViewModel(e.ClientID, e.ClientName, TSNotificationType.UserEntered);
                Threading.InvokeOnUI(() => this.Notifications.Add(notification));
                Thread.Sleep(5000); // Let channel notifications stay for 5 seconds
                Threading.InvokeOnUI(() => this.Notifications.Remove(notification));
            });
        }

        /// <summary>
        /// Handler for the Client Exited Channel event
        /// </summary>
        private void TeamspeakService_ClientExitedChannel(object sender, TS3.Data.ChannelEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var notification = new TSNotificationViewModel(e.ClientID, e.ClientName, TSNotificationType.UserExited);
                Threading.InvokeOnUI(() => this.Notifications.Add(notification));
                Thread.Sleep(5000); // Let channel notifications stay for 5 seconds
                Threading.InvokeOnUI(() => this.Notifications.Remove(notification));
            });
        }
    }
}
