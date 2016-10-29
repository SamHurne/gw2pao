using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GW2PAO.Modules.Map.Converters
{
    public class CategoryToIsVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is string && values[1] is ICollection<string>)
            {
                var category = values[0] as string;
                var hiddenCategories = values[1] as ICollection<string>;
                return !hiddenCategories.Contains(category);
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
