using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

///
/// Adapted from http://www.zagstudio.com/blog/488#.Vh5YKvlVhBc
/// Licensed under MS-PL (see http://opensource.org/licenses/MS-PL)
///

namespace GW2PAO.PresentationCore.DragDrop
{
    public static class DragDropUtilities
    {
        // Finds the orientation of the panel of the ItemsControl that contains the itemContainer passed as a parameter.
        // The orientation is needed to figure out where to draw the adorner that indicates where the item will be dropped.
        public static bool HasVerticalOrientation(FrameworkElement itemContainer)
        {
            bool hasVerticalOrientation = true;
            if (itemContainer != null)
            {
                Panel panel = VisualTreeHelper.GetParent(itemContainer) as Panel;
                StackPanel stackPanel;
                WrapPanel wrapPanel;

                if ((stackPanel = panel as StackPanel) != null)
                {
                    hasVerticalOrientation = (stackPanel.Orientation == Orientation.Vertical);
                }
                else if ((wrapPanel = panel as WrapPanel) != null)
                {
                    hasVerticalOrientation = (wrapPanel.Orientation == Orientation.Vertical);
                }
                // You can add support for more panel types here.
            }
            return hasVerticalOrientation;
        }

        public static void InsertItemInItemsControl(ItemsControl itemsControl, object itemToInsert, int insertionIndex)
        {
            if (itemToInsert != null)
            {
                IEnumerable itemsSource = itemsControl.ItemsSource;

                if (itemsSource == null)
                {
                    itemsControl.Items.Insert(insertionIndex, itemToInsert);
                }
                // Is the ItemsSource IList or IList<T>? If so, insert the dragged item in the list.
                else if (itemsSource is IList)
                {
                    ((IList)itemsSource).Insert(insertionIndex, itemToInsert);
                }
                else
                {
                    Type type = itemsSource.GetType();
                    Type genericIListType = type.GetInterface("IList`1");
                    if (genericIListType != null)
                    {
                        type.GetMethod("Insert").Invoke(itemsSource, new object[] { insertionIndex, itemToInsert });
                    }
                }
            }
        }

        public static int RemoveItemFromItemsControl(ItemsControl itemsControl, object itemToRemove)
        {
            int indexToBeRemoved = -1;
            if (itemToRemove != null)
            {
                indexToBeRemoved = itemsControl.Items.IndexOf(itemToRemove);

                if (indexToBeRemoved != -1)
                {
                    IEnumerable itemsSource = itemsControl.ItemsSource;
                    if (itemsSource == null)
                    {
                        itemsControl.Items.RemoveAt(indexToBeRemoved);
                    }
                    // Is the ItemsSource IList or IList<T>? If so, remove the item from the list.
                    else if (itemsSource is IList)
                    {
                        ((IList)itemsSource).RemoveAt(indexToBeRemoved);
                    }
                    else
                    {
                        Type type = itemsSource.GetType();
                        Type genericIListType = type.GetInterface("IList`1");
                        if (genericIListType != null)
                        {
                            type.GetMethod("RemoveAt").Invoke(itemsSource, new object[] { indexToBeRemoved });
                        }
                    }
                }
            }
            return indexToBeRemoved;
        }

        public static bool IsInFirstHalf(FrameworkElement container, Point clickedPoint, bool hasVerticalOrientation)
        {
            if (hasVerticalOrientation)
            {
                return clickedPoint.Y < container.ActualHeight / 2;
            }
            return clickedPoint.X < container.ActualWidth / 2;
        }
    }
}
