using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace GW2PAO.Converters
{
    /// <summary>
    /// From: http://stackoverflow.com/a/804378
    /// </summary>
    public class TabSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TabControl tabControl = value as TabControl;
            double height = tabControl.ActualHeight / tabControl.Items.Count;
            return (height <= 1) ? 0 : height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
