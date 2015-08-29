using GW2PAO.PresentationCore;
using GW2PAO.TS3.Services.Interfaces;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GW2PAO.Modules.Teamspeak.ViewModels
{
    [Export(typeof(TeamspeakViewModel))]
    public class TeamspeakViewModel : BindableBase
    {
        private string messageText;
        private string serverName;
        private string serverAddress;
        private string clientChannelName;
        private string clientChannelDescription;

        /// <summary>
        /// List of 'orphan' channels - channels that have been added but who's parent has not been added 'yet'
        /// </summary>
        private List<ChannelViewModel> orphanChannels = new List<ChannelViewModel>();

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
            set { this.SetProperty(ref this.messageText, value); }
        }

        /// <summary>
        /// The currently connected server's name
        /// </summary>
        public string ServerName
        {
            get { return this.serverName; }
            set { this.SetProperty(ref this.serverName, value); }
        }

        /// <summary>
        /// The address of the currently connected server
        /// </summary>
        public string ServerAddress
        {
            get { return this.serverAddress; }
            set { this.SetProperty(ref this.serverAddress, value); }
        }

        /// <summary>
        /// The currently connected channel's name
        /// </summary>
        public string ClientChannelName
        {
            get { return this.clientChannelName; }
            set { this.SetProperty(ref this.clientChannelName, value); }
        }

        /// <summary>
        /// The description of the currently connected channel
        /// </summary>
        public string ClientChannelDescription
        {
            get { return this.clientChannelDescription; }
            set { this.SetProperty(ref this.clientChannelDescription, value); }
        }

        /// <summary>
        /// User data for the Teamspeak Overlay
        /// </summary>
        public TeamspeakUserData UserData { get; private set; }

        /// <summary>
        /// True if channels have been loaded, else false
        /// </summary>
        public bool AreChannelsLoaded
        {
            get { return this.Channels.Count > 0; }
        }

        /// <summary>
        /// Observable collection of sent/received chat messages
        /// </summary>
        public ObservableCollection<ChatMsgViewModel> ChatMessages { get; private set; }

        /// <summary>
        /// Collection of client notifications (speaking users, messages, users entering, etc)
        /// </summary>
        public ObservableCollection<TSNotificationViewModel> Notifications { get; private set; }

        /// <summary>
        /// Collection of sub channels for this channel
        /// </summary>
        public ObservableCollection<ChannelViewModel> Channels { get; private set; }

        /// <summary>
        /// Collection view of channels
        /// </summary>
        public AutoRefreshCollectionViewSource ChannelsSource { get; private set; }

        /// <summary>
        /// Collection clients in the current channel
        /// </summary>
        public ObservableCollection<ClientViewModel> CurrentChannelClients { get; private set; }

        /// <summary>
        /// Command to reset all hidden objectives
        /// </summary>
        public DelegateCommand SendMessageCommand { get { return new DelegateCommand(this.SendMessage); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public TeamspeakViewModel(ITeamspeakService teamspeakService, TeamspeakUserData userData)
        {
            this.isShuttingDown = false;
            this.UserData = userData;
            this.ChatMessages = new ObservableCollection<ChatMsgViewModel>();
            this.Notifications = new ObservableCollection<TSNotificationViewModel>();
            this.Channels = new ObservableCollection<ChannelViewModel>();
            this.CurrentChannelClients = new ObservableCollection<ClientViewModel>();

            var channelsSource = new AutoRefreshCollectionViewSource();
            channelsSource.Source = this.Channels;
            this.ChannelsSource = channelsSource;
            this.ChannelsSource.SortDescriptions.Add(new SortDescription("OrderIndex", ListSortDirection.Ascending));

            this.TeamspeakService = teamspeakService;
            this.TeamspeakService.NewServerInfo += TeamspeakService_NewServerInfo;
            this.TeamspeakService.ClientChannelChanged += TeamspeakService_ClientChannelChanged;
            this.TeamspeakService.ConnectionRefused += TeamspeakService_ConnectionRefused;
            this.TeamspeakService.TalkStatusChanged += TeamspeakService_TalkStatusChanged;
            this.TeamspeakService.TextMessageReceived += TeamspeakService_TextMessageReceived;
            this.TeamspeakService.ClientEnteredChannel += TeamspeakService_ClientEnteredChannel;
            this.TeamspeakService.ClientExitedChannel += TeamspeakService_ClientExitedChannel;
            this.TeamspeakService.ChannelAdded += TeamspeakService_ChannelAdded;
            this.TeamspeakService.ChannelRemoved += TeamspeakService_ChannelRemoved;
            this.TeamspeakService.ChannelUpdated += TeamspeakService_ChannelUpdated;

            Task.Factory.StartNew((state) =>
                {
                    // Start this on another thread so that we don't hold up anything creating us
                    this.TeamspeakService.Connect();
                }, null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext() );
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
            logger.Info("Attempting to send message: {0}", MessageText);
            if (!string.IsNullOrWhiteSpace(MessageText))
            {
                this.TeamspeakService.SendChannelMessage(this.MessageText);
                this.MessageText = string.Empty;
            }
        }

        /// <summary>
        /// Handles the New Server Info event of the Teamspeak Service
        /// </summary>
        private void TeamspeakService_NewServerInfo(object sender, TS3.Data.NewServerInfoEventArgs e)
        {
            Threading.InvokeOnUI(() =>
                {
                    this.ServerName = e.ServerName;
                    this.ServerAddress = e.ServerAddress;
                    this.ChatMessages.Clear();
                    this.Notifications.Clear();
                });
        }

        /// <summary>
        /// Handles the New Channel Info event of the Teamspeak Service
        /// </summary>
        private void TeamspeakService_ClientChannelChanged(object sender, TS3.Data.ChannelEventArgs e)
        {
            Threading.InvokeOnUI(() =>
                {
                    this.ClientChannelName = e.Channel.Name;
                    this.ClientChannelDescription = e.Channel.Description;
                });
        }

        /// <summary>
        /// Connection was refused, meaning teamspeak is not running
        /// </summary>
        private void TeamspeakService_ConnectionRefused(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
                {
                    this.TeamspeakService.ConnectionRefused -= TeamspeakService_ConnectionRefused;

                    var cannotConnectNotification = new TSNotificationViewModel(0, Properties.Resources.StartTeamspeak, TSNotificationType.CannotConnect);
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
            Threading.BeginInvokeOnUI(() =>
                {
                    this.ChatMessages.Insert(0, new ChatMsgViewModel(DateTime.Now, e.ClientName, e.Message));
                });
        }

        /// <summary>
        /// Handler for the Client Entered Channel event
        /// </summary>
        private void TeamspeakService_ClientEnteredChannel(object sender, TS3.Data.ClientEventArgs e)
        {
            if (this.UserData.ShowEnterExitChannelNotifications)
            {
                Task.Factory.StartNew(() =>
                {
                    var notification = new TSNotificationViewModel(e.ClientID, e.ClientName, TSNotificationType.UserEntered);
                    Threading.InvokeOnUI(() => this.Notifications.Add(notification));
                    Thread.Sleep(5000); // Let channel notifications stay for 5 seconds
                    Threading.InvokeOnUI(() => this.Notifications.Remove(notification));
                });
                Threading.InvokeOnUI(() =>
                    {
                        this.CurrentChannelClients.Add(new ClientViewModel(e.ClientID, e.ClientName));
                    });
            }
        }

        /// <summary>
        /// Handler for the Client Exited Channel event
        /// </summary>
        private void TeamspeakService_ClientExitedChannel(object sender, TS3.Data.ClientEventArgs e)
        {
            if (this.UserData.ShowEnterExitChannelNotifications)
            {
                Task.Factory.StartNew(() =>
                {
                    var notification = new TSNotificationViewModel(e.ClientID, e.ClientName, TSNotificationType.UserExited);
                    Threading.InvokeOnUI(() => this.Notifications.Add(notification));
                    Thread.Sleep(5000); // Let channel notifications stay for 5 seconds
                    Threading.InvokeOnUI(() => this.Notifications.Remove(notification));
                });
                Threading.InvokeOnUI(() =>
                {
                    var client = this.CurrentChannelClients.FirstOrDefault(c => c.ID == e.ClientID);
                    if (client != null)
                        this.CurrentChannelClients.Remove(client);
                });
            }
        }

        /// <summary>
        /// Handler for the Channel Added event
        /// </summary>
        private void TeamspeakService_ChannelAdded(object sender, TS3.Data.ChannelEventArgs e)
        {
            Threading.BeginInvokeOnUI(() =>
                {
                    if (e.Channel.IsSpacer)
                        return; // Totally ignore spacers

                    var newChannel = new ChannelViewModel(e.Channel, this.TeamspeakService);

                    // Check if we have any orphans that are a subchannel of this new channel
                    var matchingOrphans = this.orphanChannels.Where(c => c.ParentID == newChannel.ID);
                    foreach (var orphan in matchingOrphans)
                    {
                        newChannel.Subchannels.Add(orphan);
                    }
                    this.orphanChannels.RemoveAll(c => c.ParentID == newChannel.ID);

                    if (newChannel.ParentID != 0)
                    {
                        // This has a parent channel - find it
                        var parentChannel = this.FindParentChannel(this.Channels, newChannel);

                        if (parentChannel != null)
                        {
                            parentChannel.Subchannels.Add(newChannel);
                        }
                        else
                        {
                            // This is an orphan channel... add it to our orphan list for now
                            this.orphanChannels.Add(newChannel);
                        }
                    }
                    else
                    {
                        // No parent
                        this.Channels.Insert(0, newChannel);
                    }

                    this.OnPropertyChanged(() => this.AreChannelsLoaded);
                });
        }

        /// <summary>
        /// Handler for the Channel Removed event
        /// </summary>
        private void TeamspeakService_ChannelRemoved(object sender, TS3.Data.ChannelEventArgs e)
        {
            Threading.BeginInvokeOnUI(() =>
                {
                    var removedChannel = new ChannelViewModel(e.Channel, this.TeamspeakService);

                    if (removedChannel.ParentID != 0)
                    {
                        // This has a parent channel - find it
                        var parentChannel = this.FindParentChannel(this.Channels, removedChannel);

                        if (parentChannel != null)
                        {
                            var toRemove = parentChannel.Subchannels.FirstOrDefault(channel => channel.ID == removedChannel.ID);
                            parentChannel.Subchannels.Remove(toRemove);
                        }
                    }
                    else
                    {
                        // No parent
                        var toRemove = this.Channels.FirstOrDefault(channel => channel.ID == removedChannel.ID);
                        this.Channels.Remove(toRemove);
                    }

                    this.OnPropertyChanged(() => this.AreChannelsLoaded);
                });
        }

        /// <summary>
        /// Handler for the Channel Updated event
        /// </summary>
        private void TeamspeakService_ChannelUpdated(object sender, TS3.Data.ChannelEventArgs e)
        {
            Threading.BeginInvokeOnUI(() =>
                {
                    // Find the matching existing channel
                    ChannelViewModel existingChannel = this.FindChannel(this.Channels, e.Channel.ID);

                    if (existingChannel == null)
                    {
                        // This shouldn't happen, but if it does, don't crash, just treat it as a "channel added"
                        this.TeamspeakService_ChannelAdded(sender, e);
                        return;
                    }

                    existingChannel.Name = e.Channel.Name;
                    existingChannel.OrderIndex = e.Channel.Order;
                    existingChannel.ClientsCount = e.Channel.ClientsCount;

                    // Check to see if the parent ID has changed. If so, update it and move the channel
                    if (existingChannel.ParentID != e.Channel.ParentID)
                    {
                        // Find the existing parent
                        ChannelViewModel existingParent = this.FindParentChannel(this.Channels, existingChannel);

                        if (existingParent != null)
                        {
                            // Remove it from the parent
                            existingParent.Subchannels.Remove(existingChannel);
                        }

                        // Update the parent ID
                        existingChannel.ParentID = e.Channel.ParentID;

                        // Find the new parent
                        ChannelViewModel newParent = this.FindParentChannel(this.Channels, existingChannel);

                        if (newParent != null)
                        {
                            // Add it to the parent
                            newParent.Subchannels.Add(existingChannel);
                        }
                        else
                        {
                            // Orphan...
                            this.orphanChannels.Add(existingChannel);
                        }
                    }

                    this.OnPropertyChanged(() => this.AreChannelsLoaded);
                });
        }

        /// <summary>
        /// Recursively searches through a collection of channels to find a subChannel's parent ChannelViewModel object
        /// </summary>
        /// <param name="channelCollection">The collection to search</param>
        /// <param name="subChannel">The subChannel of which to the find the parent of</param>
        /// <returns>The parent of the subchannel, or null if not found</returns>
        private ChannelViewModel FindParentChannel(ICollection<ChannelViewModel> channelCollection, ChannelViewModel subChannel)
        {
            foreach (var channel in channelCollection)
            {
                if (channel.ID == subChannel.ParentID)
                {
                    return channel;
                }
                else
                {
                    // Recurse within the subchannels of this channel
                    var parent = this.FindParentChannel(channel.Subchannels, subChannel);
                    if (parent != null)
                    {
                        return parent;
                    }
                }
            }

            // Didn't find parent
            return null;
        }

        /// <summary>
        /// Recursively searches through a collection of channels to find a specific ChannelViewModel object
        /// </summary>
        /// <param name="channelCollection">The collection to search</param>
        /// <param name="subChannel">The subChannel of which to the find the parent of</param>
        /// <returns>The parent of the subchannel, or null if not found</returns>
        private ChannelViewModel FindChannel(ICollection<ChannelViewModel> channelCollection, uint channelId)
        {
            foreach (var channel in channelCollection)
            {
                if (channel.ID == channelId)
                {
                    return channel;
                }
                else
                {
                    // Recurse within the subchannels of this channel
                    var foundChannel = this.FindChannel(channel.Subchannels, channelId);
                    if (foundChannel != null)
                    {
                        return foundChannel;
                    }
                }
            }

            // Didn't find channel
            return null;
        }
    }
}
