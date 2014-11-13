using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GW2PAO.ViewModels;
using NLog;

namespace GW2PAO.Modules.Commerce.Views
{
    /// <summary>
    /// Interaction logic for PriceWatchView.xaml
    /// </summary>
    public partial class PriceWatchView : UserControl
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default constructor
        /// </summary>
        public PriceWatchView()
        {
            InitializeComponent();
        }

        private void OnIntelliboxSuggestItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.ItemsEntryBox.ChooseCurrentItem();
        }
    }
}
