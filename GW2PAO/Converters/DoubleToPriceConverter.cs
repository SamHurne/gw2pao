using GW2PAO.Modules.Commerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GW2PAO.Converters
{
    /// <summary>
    /// Converts a double value to a Price object
    /// 1-Way conversion
    /// </summary>
    public class DoubleToPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double))
                throw new InvalidOperationException("Value must be a double");

            return new Price() { Value = (double)value };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
