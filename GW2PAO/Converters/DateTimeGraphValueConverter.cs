using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GW2PAO.Converters
{
    /// <summary>
    /// Converts a DateTime Axis double value to a DateTime value
    /// 1-Way conversion
    /// </summary>
    public class DateTimeGraphValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double))
                throw new InvalidOperationException("Value must be a double");

            return DateTimeAxis.ToDateTime((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
