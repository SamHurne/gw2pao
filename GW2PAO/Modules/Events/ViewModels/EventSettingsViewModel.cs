using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Properties;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Events.ViewModels
{
    [Export(typeof(EventSettingsViewModel))]
    public class EventSettingsViewModel : BindableBase, ISettingsViewModel
    {
        /// <summary>
        /// Heading for the settings view
        /// </summary>
        public string SettingsHeader
        {
            get { return Resources.Events; }
        }

        /// <summary>
        /// The event user data
        /// </summary>
        public EventsUserData UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userData">The events user data</param>
        [ImportingConstructor]
        public EventSettingsViewModel(EventsUserData userData)
        {
            this.UserData = userData;
        }
    }
}
