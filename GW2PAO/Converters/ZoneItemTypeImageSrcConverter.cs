using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.Converters
{
    /// <summary>
    /// Converts a ZoneItemtype to an image patch, using a second, boolean-value to determine the exact image to use
    /// Note: The first value must be of type ZoneItemType, and the second value must be of type Boolean
    /// (1-way conversion)
    /// </summary>
    public class ZoneItemTypeImageSrcConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ZoneItemType zoneItem = (ZoneItemType)values[0];
            bool isUnlocked = (bool)values[1];

            string imagePath = string.Empty;

            if (isUnlocked)
                imagePath = @"/Images/Zone/";
            else
                imagePath = @"/Images/Zone/Gray/";

            switch (zoneItem)
            {
                case ZoneItemType.HeartQuest:
                    imagePath += "hearts.png";
                    break;
                case ZoneItemType.PointOfInterest:
                    imagePath += "poi.png";
                    break;
                case ZoneItemType.HeroPoint:
                    imagePath += "heropoint.png";
                    break;
                case ZoneItemType.Vista:
                    imagePath += "vistas.png";
                    break;
                case ZoneItemType.Waypoint:
                    imagePath += "waypoints.png";
                    break;
                default:
                    break;
            }

            return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)); ;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
