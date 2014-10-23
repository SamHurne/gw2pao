using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.Converters
{
    /// <summary>
    /// Converts a WorlColor to a color or brush
    /// (1-way conversion)
    /// </summary>
    public class WorldColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            WorldColor worldColor = (WorldColor)value;

            if (targetType == typeof(System.Windows.Media.Color))
            {
                switch (worldColor)
                {
                    case WorldColor.None:
                        return System.Windows.Media.Colors.White;
                    case WorldColor.Red:
                        return System.Windows.Media.Color.FromRgb(252, 0, 0);
                    case WorldColor.Blue:
                        return System.Windows.Media.Color.FromRgb(0, 213, 255);
                    case WorldColor.Green:
                        return System.Windows.Media.Color.FromRgb(0, 252, 126);
                    default:
                        return System.Windows.Media.Colors.White;
                }
            }
            else if (targetType == typeof(System.Windows.Media.Brush)
                    || targetType == typeof(System.Windows.Media.SolidColorBrush))
            {
                switch (worldColor)
                {
                    case WorldColor.None:
                        return System.Windows.Media.Brushes.White;
                    case WorldColor.Red:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(252, 0, 0));
                    case WorldColor.Blue:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 213, 255));
                    case WorldColor.Green:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 252, 126));
                    default:
                        return System.Windows.Media.Brushes.White;
                }
            }
            else
            {
                return worldColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
