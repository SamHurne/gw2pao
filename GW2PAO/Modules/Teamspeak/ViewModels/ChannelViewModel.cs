using GW2PAO.PresentationCore;
using GW2PAO.TS3.Data;
using GW2PAO.TS3.Services.Interfaces;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GW2PAO.Modules.Teamspeak.ViewModels
{
    public class ChannelViewModel : BindableBase
    {
        private ITeamspeakService teamspeakService;
        private uint id;
        private uint parentId;
        private string name;
        private uint orderIndex;
        private uint clientsCount;

        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The channel's Channel ID
        /// </summary>
        public uint ID
        {
            get { return this.id; }
            set { this.SetProperty(ref this.id, value); }
        }

        /// <summary>
        /// The channel's Parent Channel ID
        /// </summary>
        public uint ParentID
        {
            get { return this.parentId; }
            set { this.SetProperty(ref this.parentId, value); }
        }

        /// <summary>
        /// Name of the channel
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.SetProperty(ref this.name, value); }
        }

        /// <summary>
        /// Order-index for the channel
        /// </summary>
        public uint OrderIndex
        {
            get { return this.orderIndex; }
            set { this.SetProperty(ref this.orderIndex, value); }
        }

        /// <summary>
        /// Number of clients in the channel
        /// </summary>
        public uint ClientsCount
        {
            get { return this.clientsCount; }
            set { this.SetProperty(ref this.clientsCount, value); }
        }

        /// Collection of WvW objectives for the configured map
        /// </summary>
        public AutoRefreshCollectionViewSource ChannelsSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of sub channels for this channel
        /// </summary>
        public ObservableCollection<ChannelViewModel> Subchannels { get; private set; }

        /// <summary>
        /// Command to hide the objective
        /// </summary>
        public DelegateCommand SelectChannelCommand { get { return new DelegateCommand(this.SelectChannel); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="channelData">The channel's data</param>
        public ChannelViewModel(Channel channelData, ITeamspeakService teamspeakService)
        {
            this.ID = channelData.ID;
            this.ParentID = channelData.ParentID;
            this.Name = channelData.Name;
            this.OrderIndex = channelData.Order;
            this.ClientsCount = channelData.ClientsCount;
            this.teamspeakService = teamspeakService;
            this.Subchannels = new ObservableCollection<ChannelViewModel>();

            var subChannelsSource = new AutoRefreshCollectionViewSource();
            subChannelsSource.Source = this.Subchannels;
            this.ChannelsSource = subChannelsSource;
            this.ChannelsSource.SortDescriptions.Add(new SortDescription("OrderIndex", ListSortDirection.Ascending));
        }

        /// <summary>
        /// Switches to/selects this channel in TS
        /// </summary>
        private void SelectChannel()
        {
            logger.Info("New channel selected: {0} - {1}", this.ID, this.Name);
            this.teamspeakService.ChangeChannel(this.ID);
        }
    }
}
