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
            ObjectiveType type = (ObjectiveType)values[0];
            WorldColor color = (WorldColor)values[1];

            string imagePath = string.Empty;
            switch (type)
            {
                case ObjectiveType.Camp:
                    imagePath = @"/Resources/Camp";
                    break;
                case ObjectiveType.Tower:
                    imagePath = @"/Resources/Tower";
                    break;
                case ObjectiveType.Keep:
                    imagePath = @"/Resources/Keep";
                    break;
                case ObjectiveType.Castle:
                    imagePath = @"/Resources/Castle";
                    break;
                case ObjectiveType.TempleofLostPrayers:
                    imagePath = @"/Resources/templeofLostPrayers";
                    break;
                case ObjectiveType.BattlesHollow:
                    imagePath = @"/Resources/BattlesHollow";
                    break;
                case ObjectiveType.BauersEstate:
                    imagePath = @"/Resources/BauersEstate";
                    break;
                case ObjectiveType.OrchardOverlook:
                    imagePath = @"/Resources/OrchardOverlook";
                    break;
                case ObjectiveType.CarversAscent:
                    imagePath = @"/Resources/CarversAscent";
                    break;
                default:
                    break;
            }

            // Append the color
            switch (color)
            {
                case WorldColor.Blue:
                    imagePath += "_b";
                    break;
                case WorldColor.Green:
                    imagePath += "_g";
                    break;
                case WorldColor.Red:
                    imagePath += "_r";
                    break;
                default:
                    break;
            }

            // Append the file format (always .png)
            imagePath += ".png";

            return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
