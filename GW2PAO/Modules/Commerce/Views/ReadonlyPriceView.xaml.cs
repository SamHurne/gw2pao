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
        /// Gets or sets the Fill used for the text of the control
        /// </summary>
        public Brush TextFill
        {
            get { return (Brush)GetValue(TextFillProperty); }
            set { SetValue(TextFillProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty TextFillProperty = DependencyProperty.Register("TextFill", typeof(Brush), typeof(ReadonlyPriceView), new PropertyMetadata(Brushes.White));

        /// <summary>
        /// Gets or sets the Price used display in the control
        /// </summary>
        public Price Price
        {
            get { return (Price)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register("Price", typeof(Price), typeof(ReadonlyPriceView), new PropertyMetadata(new Price()));

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
