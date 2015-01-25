using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.Converters
{
    public class WvWMapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            WvWMap map = (WvWMap)value;

            if (targetType == typeof(string))
            {
                switch (map)
                {
                    case WvWMap.BlueBorderlands:
                        return Properties.Resources.BlueBorderlands;
                    case WvWMap.GreenBorderlands:
                        return Properties.Resources.GreenBorderlands;
                    case WvWMap.RedBorderlands:
                        return Properties.Resources.RedBorderlands;
                    case WvWMap.EternalBattlegrounds:
                        return Properties.Resources.EternalBattlegrounds;
                    case WvWMap.Unknown:
                        return Properties.Resources.Unknown;
                    default:
                        return map.ToString();
                }
            }
            else if (targetType == typeof(System.Windows.Media.Color))
            {
                switch (map)
                {
                    case WvWMap.BlueBorderlands:
                        return System.Windows.Media.Colors.SkyBlue;
                    case WvWMap.GreenBorderlands:
                        return System.Windows.Media.Colors.LightGreen;
                    case WvWMap.RedBorderlands:
                        return System.Windows.Media.Colors.Pink;
                    case WvWMap.EternalBattlegrounds:
                        return System.Windows.Media.Colors.White;
                    default:
                        return System.Windows.Media.Colors.White;
                }
            }
            else if (targetType == typeof(System.Windows.Media.Brush)
                    || targetType == typeof(System.Windows.Media.SolidColorBrush))
            {
                switch (map)
                {
                    case WvWMap.BlueBorderlands:
                        return System.Windows.Media.Brushes.SkyBlue;
                    case WvWMap.GreenBorderlands:
                        return System.Windows.Media.Brushes.LightGreen;
                    case WvWMap.RedBorderlands:
                        return System.Windows.Media.Brushes.Pink;
                    case WvWMap.EternalBattlegrounds:
                        return System.Windows.Media.Brushes.White;
                    default:
                        return System.Windows.Media.Brushes.White;
                }
            }
            else
            {
                return map;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
