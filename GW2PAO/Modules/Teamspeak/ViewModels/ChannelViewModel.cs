using System.Collections.ObjectModel;
using GW2PAO.PresentationCore;
using GW2PAO.TS3.Data;
using GW2PAO.TS3.Services.Interfaces;
using NLog;

namespace GW2PAO.Modules.Teamspeak.ViewModels
{
    public class ChannelViewModel : NotifyPropertyChangedBase
    {
        private ITeamspeakService teamspeakService;
        private Channel modelData;

        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The channel's Channel ID
        /// </summary>
        public uint ID
        {
            get { return this.modelData.ID; }
            set
            {
                if (this.modelData.ID != value)
                {
                    this.modelData.ID = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// The channel's Parent Channel ID
        /// </summary>
        public uint ParentID
        {
            get { return this.modelData.ParentID; }
            set
            {
                if (this.modelData.ParentID != value)
                {
                    this.modelData.ParentID = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Name of the channel
        /// </summary>
        public string Name
        {
            get { return this.modelData.Name; }
            set
            {
                if (this.modelData.Name != value)
                {
                    this.modelData.Name = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Order-index for the channel
        /// </summary>
        public uint OrderIndex
        {
            get { return this.modelData.Order; }
            set
            {
                if (this.modelData.Order != value)
                {
                    this.modelData.Order = value;
                    this.RaisePropertyChanged();
                }
            }
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
            this.modelData = channelData;
            this.teamspeakService = teamspeakService;
            this.Subchannels = new ObservableCollection<ChannelViewModel>();
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
