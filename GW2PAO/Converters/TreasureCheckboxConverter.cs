using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GW2PAO.Style.Converters
{
    /// <summary>
    /// Converts a boolean value to a treasure image path
    /// (1-way conversion)
    /// </summary>
    public class TreasureCheckboxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (isChecked)
                return @"/Images/Misc/treasure.png";
            else
                return @"/Images/Misc/treasure_gray.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
