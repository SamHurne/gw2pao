using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GW2PAO.ViewModels.Teamspeak;

namespace GW2PAO.Style.Converters
{
    /// <summary>
    /// Converts a ClientNotificationType to an image source
    /// </summary>
    public class TSNotificationTypeImageSrcConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var notificationType = (TSNotificationType)value;

            switch (notificationType)
            {
                case TSNotificationType.CannotConnect:
                    return @"/Images/Misc/warning.png";
                case TSNotificationType.Text:
                    return @"/Images/Teamspeak/chat_bubble.png";
                case TSNotificationType.Speech:
                    return @"/Images/Teamspeak/speaker.png";
                case TSNotificationType.UserEntered:
                    return @"/Images/Teamspeak/enter.png";
                case TSNotificationType.UserExited:
                    return @"/Images/Teamspeak/exit.png";
                default:
                    return string.Empty;
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
