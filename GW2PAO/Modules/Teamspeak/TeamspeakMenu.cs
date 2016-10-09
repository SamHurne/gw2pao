using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Infrastructure.ViewModels;
using GW2PAO.Modules.Teamspeak.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace GW2PAO.Modules.Teamspeak
{
    [Export(typeof(IMenuItem))]
    [ExportMetadata("Order", 7)]
    public class TeamspeakMenu : IMenuItem
    {
        public ObservableCollection<IMenuItem> SubMenuItems { get; private set; }

        /// <summary>
        /// Header text of the menu item
        /// </summary>
        public string Header
        {
            get { return Properties.Resources.TeamspeakOverlay; }
        }

        /// <summary>
        /// Icon source string for the menu item, if any
        /// </summary>
        public string Icon
        {
            get { return "/Images/Title/community.png"; }
        }

        /// <summary>
        /// True if the menu item is checkable, else false
        /// </summary>
        public bool IsCheckable
        {
            get { return false; }
        }

        /// <summary>
        /// True if the menu item is checked, else false
        /// </summary>
        public bool IsChecked
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// True if the menu item does not close the menu on click, else false
        /// </summary>
        public bool StaysOpen
        {
            get { return false; }
        }

        /// <summary>
        /// The on-click command
        /// </summary>
        public ICommand OnClickCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public TeamspeakMenu(ITeamspeakViewController viewFactory)
        {
            // For now, the only option is to open the teamspeak overlay, which is this menu item itself
            this.OnClickCommand = new DelegateCommand(viewFactory.DisplayTeamspeakOverlay, viewFactory.CanDisplayTeamspeakOverlay);
        }
    }
}
