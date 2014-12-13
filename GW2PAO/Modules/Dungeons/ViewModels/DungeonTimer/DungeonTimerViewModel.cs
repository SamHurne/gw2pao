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
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Dungeons.ViewModels.DungeonTimer
{
    [Export(typeof(DungeonTimerViewModel))]
    public class DungeonTimerViewModel : BindableBase
    {
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
        /// The actual timer value
        /// </summary>
        public TimeSpan TimerValue
        {
            get { return this.timerValue; }
            set { this.SetProperty(ref this.timerValue, value); }
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
        /// Default constructor
        /// </summary>
        public DungeonTimerViewModel()
        {
            this.stopWatch = new Stopwatch();
            this.timer = new Timer(TimerInterval.TotalMilliseconds);
            this.timer.AutoReset = true;
            this.timer.Elapsed += (o, e) => Threading.BeginInvokeOnUI(() => this.TimerValue = this.stopWatch.Elapsed);

            this.StartTimerCommand = new DelegateCommand(this.StartTimer);
            this.PauseTimerCommand = new DelegateCommand(this.PauseTimer);
            this.StopTimerCommand = new DelegateCommand(this.StopTimer);
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
