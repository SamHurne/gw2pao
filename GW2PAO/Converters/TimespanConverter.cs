using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GW2PAO.Style.Converters
{
    /// <summary>
    /// Converts a Timespan value to a formatted string
    /// Parameter = bool - true if the hour count should be excluded
    /// (1-way conversion)
    /// </summary>
    public class TimespanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool excludeHours = false;
            if (parameter != null)
                bool.TryParse(parameter.ToString(), out excludeHours);

            if (targetType == typeof(string))
            {
                TimeSpan timespan = (TimeSpan)value;
                if (timespan < TimeSpan.Zero)
                {
                    if (excludeHours)
                        return timespan.ToString("mm\\:ss");
                    else
                        return timespan.ToString("hh\\:mm\\:ss");
                }
                else
                {
                    if (excludeHours)
                        return timespan.ToString("mm\\:ss");
                    else
                        return timespan.ToString("hh\\:mm\\:ss");
                }
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
