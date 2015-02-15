using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.Converters
{
    public class WvWMapToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            WvWMap map = (WvWMap)value;

            switch (map)
            {
                case WvWMap.BlueBorderlands:
                    return "/Images/Backgrounds/Small1_b.png";
                case WvWMap.GreenBorderlands:
                    return "/Images/Backgrounds/Small1_g.png";
                case WvWMap.RedBorderlands:
                    return "/Images/Backgrounds/Small1_r.png";
                case WvWMap.EternalBattlegrounds:
                case WvWMap.Unknown:
                default:
                    return "/Images/Backgrounds/Small1.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
