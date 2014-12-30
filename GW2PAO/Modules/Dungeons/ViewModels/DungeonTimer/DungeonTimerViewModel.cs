using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows.Threading;
using GW2PAO.Infrastructure;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Dungeons.ViewModels.DungeonTimer
{
    public class DungeonTimerViewModel : BindableBase
    {
        private DungeonViewModel currentDungeon;
        private PathViewModel currentPath;
        private TimeSpan timerValue;
        private static readonly TimeSpan TimerInterval = TimeSpan.FromMilliseconds(50);

        /// <summary>
        /// The timer that updates the TimerValue property with the stopwatch's elapsed time
        /// </summary>
        private Timer timer;

        /// <summary>
        /// The stopwatch object that provides the accurate timer values
        /// </summary>
        private Stopwatch stopWatch;

        /// <summary>
        /// Dungeons user data object
        /// </summary>
        public DungeonsUserData UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// The actual timer value
        /// </summary>
        public TimeSpan TimerValue
        {
            get { return this.timerValue; }
            set { this.SetProperty(ref this.timerValue, value); }
        }

        /// <summary>
        /// The current dungeon
        /// </summary>
        public DungeonViewModel CurrentDungeon
        {
            get { return this.currentDungeon; }
            set
            { 
                if (this.currentDungeon != value)
                {
                    if (this.currentDungeon != null)
                        this.currentDungeon.IsActive = false;

                    this.currentDungeon = value;

                    if (this.currentDungeon != null)
                        this.currentDungeon.IsActive = true;

                    this.OnPropertyChanged(() => this.CurrentDungeon);
                }
            }
        }

        /// <summary>
        /// The current dungeon path
        /// </summary>
        public PathViewModel CurrentPath
        {
            get { return this.currentPath; }
            set
            {
                if (this.currentPath != value)
                {
                    if (this.currentPath != null)
                        this.currentPath.IsActive = false;

                    this.currentPath = value;

                    if (this.currentPath != null)
                        this.currentPath.IsActive = true;

                    this.OnPropertyChanged(() => this.CurrentPath);
                }
            }
        }

        /// <summary>
        /// True if the timer is currently running, else false
        /// </summary>
        public bool IsTimerRunning
        {
            get
            {
                return this.stopWatch.IsRunning;
            }
        }

        /// <summary>
        /// Command to start the timer
        /// </summary>
        public ICommand StartTimerCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to pause the timer
        /// </summary>
        public ICommand PauseTimerCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to pause the timer
        /// </summary>
        public ICommand StopTimerCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to open the dungeon timer settings
        /// </summary>
        public ICommand OpenSettingsCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userData">The dungeons user data</param>
        public DungeonTimerViewModel(DungeonsUserData userData)
        {
            this.UserData = userData;
            this.stopWatch = new Stopwatch();
            this.timer = new Timer(TimerInterval.TotalMilliseconds);
            this.timer.AutoReset = true;
            this.timer.Elapsed += (o, e) => Threading.BeginInvokeOnUI(() => this.TimerValue = this.stopWatch.Elapsed);

            this.StartTimerCommand = new DelegateCommand(this.StartTimer);
            this.PauseTimerCommand = new DelegateCommand(this.PauseTimer);
            this.StopTimerCommand = new DelegateCommand(this.StopTimer);
            this.OpenSettingsCommand = new DelegateCommand(() => Commands.OpenDungeonSettingsCommand.Execute(null));
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void StartTimer()
        {
            if (!this.timer.Enabled)
            {
                this.timer.Start();
                this.stopWatch.Start();
            }
        }

        /// <summary>
        /// Pauses the timer
        /// </summary>
        public void PauseTimer()
        {
            if (this.timer.Enabled)
            {
                this.timer.Stop();
                this.stopWatch.Stop();
            }
        }

        /// <summary>
        /// Stops the timer and clears the value
        /// </summary>
        public void StopTimer()
        {
            this.PauseTimer();
            this.stopWatch.Reset();
            Threading.BeginInvokeOnUI(() => this.TimerValue = this.stopWatch.Elapsed);
        }
    }
}
