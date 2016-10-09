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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GW2PAO.Controls
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor",
            typeof(string),
            typeof(ColorPicker),
            new PropertyMetadata("#FFF"));

        public string SelectedColor
        {
            get { return (string)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty ColorOptionsProperty = DependencyProperty.Register("ColorOptions",
            typeof(ICollection<string>),
            typeof(ColorPicker),
            new PropertyMetadata(null));

        public ICollection<string> ColorOptions
        {
            get { return (ICollection<string>)GetValue(ColorOptionsProperty); }
            set { SetValue(ColorOptionsProperty, value); }
        }

        public ColorPicker()
        {
            InitializeComponent();
            this.MainPanel.DataContext = this;
        }
    }
}
