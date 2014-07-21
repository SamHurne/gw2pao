using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.Style.Converters
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

            switch (zoneItem)
            {
                case ZoneItemType.HeartQuest:
                    {
                        if (isUnlocked)
                            imagePath = @"/Resources/hearts.png";
                        else
                            imagePath = @"/Resources/hearts_gray.png";
                    }
                    break;
                case ZoneItemType.PointOfInterest:
                    {
                        if (isUnlocked)
                            imagePath = @"/Resources/poi.png";
                        else
                            imagePath = @"/Resources/poi_gray.png";
                    }
                    break;
                case ZoneItemType.SkillChallenge:
                    {
                        if (isUnlocked)
                            imagePath = @"/Resources/skillpoints.png";
                        else
                            imagePath = @"/Resources/skillpoints_gray.png";
                    }
                    break;
                case ZoneItemType.Vista:
                    {
                        if (isUnlocked)
                            imagePath = @"/Resources/vistas.png";
                        else
                            imagePath = @"/Resources/vistas_gray.png";
                    }
                    break;
                case ZoneItemType.Waypoint:
                    {
                        if (isUnlocked)
                            imagePath = @"/Resources/waypoints.png";
                        else
                            imagePath = @"/Resources/waypoints_gray.png";
                    }
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
