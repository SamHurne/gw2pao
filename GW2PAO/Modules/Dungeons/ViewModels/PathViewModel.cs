using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.Data;
using GW2PAO.Data.UserData;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using GW2PAO.Modules.Dungeons.Data;
using GW2PAO.Modules.WebBrowser.Interfaces;

namespace GW2PAO.Modules.Dungeons.ViewModels
{
    public class PathViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private DungeonsUserData userData;
        private bool isActivePath;

        /// <summary>
        /// The browser controller used for displaying the dungeon's wiki page
        /// </summary>
        private IWebBrowserController browserController;

        /// <summary>
        /// The primary model object containing the path's information
        /// </summary>
        public DungeonPath PathModel { get; private set; }

        /// <summary>
        /// The paths's ID
        /// </summary>
        public Guid PathId { get { return this.PathModel.ID; } }

        /// <summary>
        /// The paths's display text name
        /// </summary>
        public string DisplayName { get { return this.PathModel.PathDisplayText; } }

        /// <summary>
        /// The paths's nickname
        /// </summary>
        public string NickName { get { return this.PathModel.Nickname; } }

        /// <summary>
        /// The paths's reward Gold component
        /// </summary>
        public int RewardGold { get { return (int)this.PathModel.GoldReward; } }

        /// <summary>
        /// The paths's reward Silver component
        /// </summary>
        public int RewardSilver { get { return (int)((this.PathModel.GoldReward - RewardGold) * 100); } }

        /// <summary>
        /// True if the path has been completed, else false
        /// </summary>
        public bool IsCompleted
        {
            get { return this.userData.CompletedPaths.Contains(this.PathId); }
            set
            {
                if (value && !this.userData.CompletedPaths.Contains(this.PathId))
                {
                    logger.Debug("Adding \"{0}\" to CompletedPaths", this.PathId);
                    this.userData.CompletedPaths.Add(this.PathModel.ID);
                    this.OnPropertyChanged(() => this.IsCompleted);
                }
                else
                {
                    logger.Debug("Removing \"{0}\" from CompletedPaths", this.PathId);
                    if (this.userData.CompletedPaths.Remove(this.PathId))
                        this.OnPropertyChanged(() => this.IsCompleted);
                }
            }
        }

        /// <summary>
        /// True if the path is currently being completed by the player
        /// </summary>
        public bool IsActive
        {
            get { return this.isActivePath; }
            set { this.SetProperty(ref this.isActivePath, value); }
        }

        /// <summary>
        /// The in-game end location for the path
        /// </summary>
        public DetectionPoint EndPoint
        {
            get { return this.PathModel.EndPoint; }
        }

        /// <summary>
        /// Collection of in-game points that identify this path
        /// </summary>
        public List<DetectionPoint> IdentifyingPoints
        {
            get { return this.PathModel.IdentifyingPoints; }
        }

        /// <summary>
        /// Number of cutscenes shown while at the endpoint
        /// </summary>
        public int EndCutsceneCount
        {
            get { return this.PathModel.EndCutsceneCount; }
        }

        /// <summary>
        /// Best completion time for this path
        /// </summary>
        public PathTime BestTime
        {
            get { return this.userData.BestPathTimes.FirstOrDefault(pt => pt.PathID == this.PathId); }
        }

        /// <summary>
        /// Command to open the guide for this dungeon
        /// </summary>
        public DelegateCommand OpenGuideCommand { get { return new DelegateCommand(this.OpenGuide); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userData"></param>
        public PathViewModel(DungeonPath path, IWebBrowserController browserController, DungeonsUserData userData)
        {
            this.PathModel = path;
            this.userData = userData;
            this.browserController = browserController;
        }

        /// <summary>
        /// Opens the wiki page for this dungeon
        /// </summary>
        private void OpenGuide()
        {
            this.browserController.GoToUrl(this.PathModel.GuideUrl);
        }
    }
}
