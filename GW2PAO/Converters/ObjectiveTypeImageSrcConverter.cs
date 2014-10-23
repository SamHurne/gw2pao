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
    /// Converts an ObjectiveType to an image source path
    /// Note: The first value must be of type ObjectiveType, and the second value must be of type WorldColor
    /// (1-way conversion)
    /// </summary>
    public class ObjectiveTypeImageSrcConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is ObjectiveType
                && values[1] is WorldColor)
            {

                ObjectiveType type = (ObjectiveType)values[0];
                WorldColor color = (WorldColor)values[1];

                string imagePath = @"/Images/WvW/";

                // Append the color path
                switch (color)
                {
                    case WorldColor.Blue:
                        imagePath += "Blue/";
                        break;
                    case WorldColor.Green:
                        imagePath += "Green/";
                        break;
                    case WorldColor.Red:
                        imagePath += "Red/";
                        break;
                    default:
                        imagePath += "Neutral/";
                        break;
                }

                switch (type)
                {
                    case ObjectiveType.Camp:
                        imagePath += "Camp";
                        break;
                    case ObjectiveType.Tower:
                        imagePath += "Tower";
                        break;
                    case ObjectiveType.Keep:
                        imagePath += "Keep";
                        break;
                    case ObjectiveType.Castle:
                        imagePath += "Castle";
                        break;
                    case ObjectiveType.TempleofLostPrayers:
                        imagePath += "TempleofLostPrayers";
                        break;
                    case ObjectiveType.BattlesHollow:
                        imagePath += "BattlesHollow";
                        break;
                    case ObjectiveType.BauersEstate:
                        imagePath += "BauersEstate";
                        break;
                    case ObjectiveType.OrchardOverlook:
                        imagePath += "OrchardOverlook";
                        break;
                    case ObjectiveType.CarversAscent:
                        imagePath += "CarversAscent";
                        break;
                    default:
                        break;
                }

                // Append the file format (always .png)
                imagePath += ".png";

                return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            }
            else
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
