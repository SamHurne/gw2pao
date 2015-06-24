using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.Dungeons.Data;
using NLog;

namespace GW2PAO.Modules.Dungeons
{
    /// <summary>
    /// User data for the Dungeons Tracker
    /// </summary>
    [Serializable]
    public class DungeonsUserData : UserData<DungeonsUserData>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public const string Filename = "DungeonsUserData.xml";

        private bool autoStartDungeonTimer;
        private bool autoStopDungeonTimer;
        private bool autoCompleteDungeons;

        private DateTime lastResetDateTime;
        private ObservableCollection<Guid> hiddenDungeons = new ObservableCollection<Guid>();
        private ObservableCollection<Guid> completedPaths = new ObservableCollection<Guid>();
        private ObservableCollection<PathCompletionData> pathCompletionData = new ObservableCollection<PathCompletionData>();

        /// <summary>
        /// True if the dungeon timer should automatically start, else false
        /// </summary>
        public bool AutoStartDungeonTimer
        {
            get { return this.autoStartDungeonTimer; }
            set { this.SetProperty(ref this.autoStartDungeonTimer, value); }
        }

        /// <summary>
        /// True if the dungeon timer should automatically stop, else false
        /// </summary>
        public bool AutoStopDungeonTimer
        {
            get { return this.autoStopDungeonTimer; }
            set { this.SetProperty(ref this.autoStopDungeonTimer, value); }
        }

        /// <summary>
        /// True if dungeons will automatically be marked as completed, else false
        /// </summary>
        public bool AutoCompleteDungeons
        {
            get { return this.autoCompleteDungeons; }
            set { this.SetProperty(ref this.autoCompleteDungeons, value); }
        }

        /// <summary>
        /// The last recorded server-reset date/time
        /// </summary>
        public DateTime LastResetDateTime
        {
            get { return this.lastResetDateTime; }
            set { SetProperty(ref this.lastResetDateTime, value); }
        }

        /// <summary>
        /// Collection of user-configured Hidden Dungeons
        /// </summary>
        public ObservableCollection<Guid> HiddenDungeons { get { return this.hiddenDungeons; } }

        /// <summary>
        /// Collection of user-configured completed dungeon paths (completed for the current day)
        /// </summary>
        public ObservableCollection<Guid> CompletedPaths { get { return this.completedPaths; } }

        /// <summary>
        /// Collection of dungeon path completion data
        /// </summary>
        public ObservableCollection<PathCompletionData> PathCompletionData { get { return this.pathCompletionData; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DungeonsUserData()
        {
            // Defaults:
            this.LastResetDateTime = DateTime.UtcNow;
            this.AutoStartDungeonTimer = true;
            this.AutoStopDungeonTimer = true;
            this.AutoCompleteDungeons = true;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => DungeonsUserData.SaveData(this, DungeonsUserData.Filename);
            this.HiddenDungeons.CollectionChanged += (o, e) => DungeonsUserData.SaveData(this, DungeonsUserData.Filename);
            this.CompletedPaths.CollectionChanged += (o, e) => DungeonsUserData.SaveData(this, DungeonsUserData.Filename);
            this.PathCompletionData.CollectionChanged += (o, e) =>
                {
                    DungeonsUserData.SaveData(this, DungeonsUserData.Filename);
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (PathCompletionData pt in e.NewItems)
                        {
                            pt.PropertyChanged += (obj, arg) => DungeonsUserData.SaveData(this, DungeonsUserData.Filename);
                            pt.CompletionTimes.CollectionChanged += (obj, arg) => DungeonsUserData.SaveData(this, DungeonsUserData.Filename);
                        }
                    }
                };
            foreach (PathCompletionData pt in this.PathCompletionData)
            {
                pt.PropertyChanged += (obj, arg) => DungeonsUserData.SaveData(this, DungeonsUserData.Filename);
            }
        }
    }
}
