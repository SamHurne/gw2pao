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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FontAwesome.WPF;

namespace GW2PAO.Modules.Map.Views.Controls
{
    /// <summary>
    /// Interaction logic for MapSettingControl.xaml
    /// </summary>
    [ContentProperty("PanelContent")]
    public partial class MapSettingControl : UserControl
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
            typeof(FontAwesomeIcon),
            typeof(MapSettingControl),
            new PropertyMetadata(FontAwesomeIcon.MapMarker));

        public FontAwesomeIcon Icon
        {
            get { return (FontAwesomeIcon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty PanelContentProperty = DependencyProperty.Register("PanelContent",
            typeof(FrameworkElement),
            typeof(MapSettingControl),
            new PropertyMetadata(null));

        public FrameworkElement PanelContent
        {
            get { return (FrameworkElement)GetValue(PanelContentProperty); }
            set { SetValue(PanelContentProperty, value); }
        }

        public MapSettingControl()
        {
            InitializeComponent();
        }
    }
}
