using System;
using System.Collections.Generic;
using System.Linq;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.PresentationCore;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Events.ViewModels.MetaEventTimers
{
    /// <summary>
    /// View model for an event shown by the event tracker
    /// </summary>
    public class MetaEventViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private MetaEvent metaEventData;
        private string mapName;
        private MetaEventStage currentStage;
        private MetaEventStage nextStage;
        private TimeSpan prevUpdateTimeUtc;
        private TimeSpan timeUntilNextStage;
        private TimeSpan timeSinceStageStarted;
        private bool isVisible;

        /// <summary>
        /// ID of the meta event
        /// </summary>
        public Guid EventId
        {
            get { return this.metaEventData.ID; }
        }

        /// <summary>
        /// ID of the zone for the meta event
        /// </summary>
        public int MapID
        {
            get { return this.metaEventData.MapID; }
        }

        /// <summary>
        /// Name of the zone for the meta event
        /// </summary>
        public string MapName
        {
            get { return this.mapName; }
            set { SetProperty(ref this.mapName, value); }
        }

        /// <summary>
        /// Current stage of the meta event
        /// </summary>
        public MetaEventStage CurrentStage
        {
            get { return this.currentStage; }
            set { SetProperty(ref this.currentStage, value); }
        }

        /// <summary>
        /// Next stage of the meta event
        /// </summary>
        public MetaEventStage NextStage
        {
            get { return this.nextStage; }
            set { SetProperty(ref this.nextStage, value); }
        }

        /// <summary>
        /// Time until the next meta event stage begins
        /// </summary>
        public TimeSpan TimeUntilNextStage
        {
            get { return this.timeUntilNextStage; }
            set { SetProperty(ref this.timeUntilNextStage, value); }
        }

        /// <summary>
        /// Time since the current meta event stage started
        /// </summary>
        public TimeSpan TimeSinceStageStarted
        {
            get { return this.timeSinceStageStarted; }
            set { SetProperty(ref this.timeSinceStageStarted, value); }
        }

        /// <summary>
        /// The general events-related user settings/data
        /// </summary>
        public EventsUserData UserData { get; private set; }

        /// <summary>
        /// Visibility of the meta event
        /// Visibility is based on multiple properties, including:
        ///     - EventState and the user configuration for what states are shown
        ///     - Whether or not the event is user-configured as hidden
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { SetProperty(ref this.isVisible, value); }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MetaEventViewModel(MetaEvent metaEventData)
        {
            this.metaEventData = metaEventData;
            this.IsVisible = true;

            var currentTime = DateTime.UtcNow.TimeOfDay;
            this.InitializeStagesAndTimers(currentTime);
            this.prevUpdateTimeUtc = currentTime;
        }

        /// <summary>
        /// Updates the TimeUntilNextStage, TimeSinceStageStarted, CurrentStage, and NextStage
        /// </summary>
        public void Update(TimeSpan currentTimeUtc)
        {
            // Update our current stage

            // Since this will be called often, we'll use an approach to reduce computation time
            // Determine the change since we were last called, apply it to our timers, then see if we need
            // to move to our next stage

            TimeSpan changeInTime = currentTimeUtc.Subtract(this.prevUpdateTimeUtc);

            Threading.BeginInvokeOnUI(() =>
            {
                this.TimeUntilNextStage -= changeInTime;
                this.TimeSinceStageStarted += changeInTime;

                if (this.TimeUntilNextStage <= TimeSpan.Zero)
                {
                    // Time to move to the next stage
                    this.CurrentStage = this.NextStage;

                    var stageIndex = this.metaEventData.Stages.FindIndex(stage => stage.Name == this.NextStage.Name);
                    if ((stageIndex + 1) >= this.metaEventData.Stages.Count)
                        this.NextStage = this.metaEventData.Stages.First();
                    else
                        this.NextStage = this.metaEventData.Stages[stageIndex + 1];

                    this.TimeSinceStageStarted = TimeSpan.Zero;
                    this.TimeUntilNextStage = this.CurrentStage.Duration.Time;

                    logger.Info("New stage for meta event {0} - Current Stage: {1} - Next Stage: {2} - Next stage in {3}",
                        this.metaEventData.Name, this.CurrentStage.Name, this.NextStage.Name, this.TimeUntilNextStage);
                }
            });

            this.prevUpdateTimeUtc = currentTimeUtc;
        }

        /// <summary>
        /// Initializes the current and next stage of this meta event based on the provided current time 
        /// </summary>
        /// <param name="currentTime">The time to use when determining the current/next stage</param>
        private void InitializeStagesAndTimers(TimeSpan currentTime)
        {
            if (currentTime < this.metaEventData.StartOffset.Time)
            {
                // Current time is before the first stage.
                // The last stage often wraps around to the very start of the day,
                // so we must be in the last stage
                this.CurrentStage = this.metaEventData.Stages.Last();
                this.NextStage = this.metaEventData.Stages.First();
                
                this.TimeUntilNextStage = this.metaEventData.StartOffset.Time.Subtract(currentTime);
                this.TimeSinceStageStarted = this.CurrentStage.Duration.Time.Subtract(this.TimeUntilNextStage);
            }
            else
            {
                // Walk through the stages until we find the one we are in
                int stageIndex = 0;
                TimeSpan nextStageStartTime = this.metaEventData.StartOffset.Time;
                while (currentTime > nextStageStartTime)
                {
                    nextStageStartTime += this.metaEventData.Stages[stageIndex].Duration.Time;

                    stageIndex++;
                    if (stageIndex >= this.metaEventData.Stages.Count)
                        stageIndex = 0;
                }

                // We now have the next active stage
                this.NextStage = this.metaEventData.Stages[stageIndex];

                // Determine the current stage
                if (stageIndex == 0)
                    stageIndex = this.metaEventData.Stages.Count - 1;
                else
                    stageIndex--;

                this.CurrentStage = this.metaEventData.Stages[stageIndex];

                // Calculate timers
                this.TimeUntilNextStage = nextStageStartTime.Subtract(currentTime);
                this.TimeSinceStageStarted = currentTime.Subtract(nextStageStartTime.Subtract(this.CurrentStage.Duration.Time));
            }

            logger.Info("Meta Event {0} - Current Stage: {1} - Next Stage: {2} - Next stage in {3}",
                this.metaEventData.Name, this.CurrentStage.Name, this.NextStage.Name, this.TimeUntilNextStage);
        }
    }
}
