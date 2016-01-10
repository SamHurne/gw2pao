using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Services.Interfaces;
using NLog;

namespace GW2PAO.API.Services
{
    /// <summary>
    /// Service class for system information
    /// </summary>
    [Export(typeof(ISystemService))]
    public class SystemService : ISystemService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The GW2 Process
        /// </summary>
        private Process gw2Process;

        /// <summary>
        /// The current primary screen's resolution
        /// </summary>
        public Resolution ScreenResolution { get; private set; }

        /// <summary>
        /// The center point of the primary screen
        /// </summary>
        public Point ScreenCenter { get { return new Point(ScreenResolution.Width / 2, ScreenResolution.Height / 2); } }

        /// <summary>
        /// Retrieves the current GW2 Process, if running.
        /// If not running, returns NULL
        /// </summary>
        public Process Gw2Process
        {

            get
            {
                string[] processNames = new string[2] { "Gw2", "Gw2-64" };
                foreach (string processName in processNames)
                {
                    this.gw2Process = Process.GetProcessesByName(processName).FirstOrDefault();
                    if (gw2Process != null)
                        return this.gw2Process;
                }
                return this.gw2Process;
            }
        }

        /// <summary>
        /// True if the GW2 Process is running, else false
        /// </summary>
        public bool IsGw2Running
        {
            get
            {
                if (this.Gw2Process != null)
                {
                    return !this.gw2Process.HasExited;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// True if the GW2 Process is the focused process, else false
        /// </summary>
        public bool Gw2HasFocus
        {
            get
            {
                if (!this.IsGw2Running)
                {
                    return false;
                }
                else
                {
                    var fgWindow = GetForegroundWindow();
                    uint activePID;
                    GetWindowThreadProcessId(fgWindow, out activePID);

                    return activePID == gw2Process.Id;
                }
            }
        }

        /// <summary>
        /// True if the current application (GW2 PAO) Process is the focused process, else false
        /// </summary>
        public bool MyAppHasFocus
        {
            get
            {
                var fgWindow = GetForegroundWindow();
                uint activePID;
                GetWindowThreadProcessId(fgWindow, out activePID);

                return activePID == Process.GetCurrentProcess().Id;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SystemService()
        {
            logger.Info("Creating System Service");
            this.ScreenResolution = new Resolution()
            {
                Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height,
                Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width
            };
            logger.Info("Resolution detected as " + this.ScreenResolution.ToString());
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    }
}
