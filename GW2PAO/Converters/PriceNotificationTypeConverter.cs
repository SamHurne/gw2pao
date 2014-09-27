using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using GW2PAO.ViewModels.PriceNotification;

namespace GW2PAO.Style.Converters
{
    /// <summary>
    /// Converts a PriceNotificationType to a string
    /// (1-way conversion)
    /// </summary>
    public class PriceNotificationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is PriceNotificationType))
                throw new InvalidOperationException("Value must be a PriceNotificationTypeConverter");

            if (targetType == typeof(string))
            {
                var type = (PriceNotificationType)value;
                switch (type)
                {
                    case PriceNotificationType.BuyOrder:
                        return "Buy Order:";
                    case PriceNotificationType.SellListing:
                        return "Sell Listing:";
                    default:
                        return "Unknown";
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
