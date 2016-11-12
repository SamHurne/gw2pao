using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GW2PAO.PresentationCore
{
    public class ListViewBehaviour
    {
        public static readonly DependencyProperty AutoUnselectItemProperty =
            DependencyProperty.RegisterAttached(
                "AutoUnselect",
                typeof(bool),
                typeof(ListViewBehaviour),
                new UIPropertyMetadata(false, OnAutoUnselectItemChanged));

        public static bool GetAutoUnselectItem(ListView listBox)
        {
            return (bool)listBox.GetValue(AutoUnselectItemProperty);
        }

        public static void SetAutoUnselectItem(ListView listBox, bool value)
        {
            listBox.SetValue(AutoUnselectItemProperty, value);
        }

        private static void OnAutoUnselectItemChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var listView = source as ListView;
            if (listView == null)
                return;

            if (e.NewValue is bool == false)
                listView.SelectionChanged -= OnSelectionChanged;
            else
                listView.SelectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListBox listBox = sender as ListBox;
                var valid = e.AddedItems[0];
                foreach (var item in new ArrayList(listBox.SelectedItems))
                {
                    if (item != valid)
                    {
                        listBox.SelectedItems.Remove(item);
                    }
                }
            }
        }
    }
}
