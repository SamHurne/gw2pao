using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.PresentationCore;

namespace GW2PAO.ViewModels
{
    public class NewVersionNotificationViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// New version string
        /// </summary>
        public string NewVersion { get; private set; }

        /// <summary>
        /// True if update notifications should not be shown, else false
        /// </summary>
        public bool DontCheckForUpdates
        {
            get { return !Properties.Settings.Default.CheckForUpdates; }
            set
            {
                Properties.Settings.Default.CheckForUpdates = !value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Command to open the download page
        /// </summary>
        public DelegateCommand OpenDownloadPageCommad { get { return new DelegateCommand(this.OpenDownloadPage); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public NewVersionNotificationViewModel(string newVersion)
        {
            this.NewVersion = newVersion;
        }

        /// <summary>
        /// Opens the Download page (http://samhurne.github.io/gw2pao/#download) using the default browser
        /// </summary>
        private void OpenDownloadPage()
        {
            Process.Start("http://samhurne.github.io/gw2pao/#download");
        }
    }
}
