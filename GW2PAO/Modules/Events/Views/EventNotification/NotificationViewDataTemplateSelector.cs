using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GW2PAO.Modules.Events.ViewModels.EventNotification;

namespace GW2PAO.Modules.Events.Views.EventNotification
{
    public class NotificationViewDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                if (item is WorldBossEventNotificationViewModel)
                {
                    return element.FindResource("WorldBossEventDataTemplate") as DataTemplate;
                }
                else if (item is MetaEventNotificationViewModel)
                {
                    return element.FindResource("MetaEventDataTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}
