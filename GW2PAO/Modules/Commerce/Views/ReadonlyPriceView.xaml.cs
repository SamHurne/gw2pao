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
using GW2PAO.Data;
using GW2PAO.Modules.Commerce.Models;
using GW2PAO.ViewModels;
using NLog;

namespace GW2PAO.Modules.Commerce.Views
{
    /// <summary>
    /// Interaction logic for PriceView.xaml
    /// </summary>
    public partial class ReadonlyPriceView : UserControl
    {
        /// <summary>
        /// Gets or sets the Price used display in the control
        /// </summary>
        public Price Price
        {
            get { return (Price)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        /// <summary>
        /// Identified the Price dependency property
        /// </summary>
        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register("Price", typeof(Price), typeof(ReadonlyPriceView), new PropertyMetadata(new Price()));

        /// <summary>
        /// If true, the silver and copper values will include spacing
        /// so that multiple ReadonlyPriceViews will have aligned silver & copper values
        /// Defaults to False
        /// </summary>
        public bool AlignSilverCopper
        {
            get { return (bool)GetValue(AlignSilverCopperProperty); }
            set { SetValue(AlignSilverCopperProperty, value); }
        }

        /// <summary>
        /// Identified the AlignSilverCopper dependency property
        /// </summary>
        public static readonly DependencyProperty AlignSilverCopperProperty = DependencyProperty.Register("AlignSilverCopper", typeof(bool), typeof(ReadonlyPriceView), new PropertyMetadata(false));

        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default constructor
        /// </summary>
        public ReadonlyPriceView()
        {
            InitializeComponent();
            this.LayoutRoot.DataContext = this;
        }
    }
}
