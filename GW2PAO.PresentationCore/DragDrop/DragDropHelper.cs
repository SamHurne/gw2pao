using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

///
/// Adapted from http://www.zagstudio.com/blog/488#.Vh5YKvlVhBc
/// Licensed under MS-PL (see http://opensource.org/licenses/MS-PL)
///

namespace GW2PAO.PresentationCore.DragDrop
{
    public delegate void OnDropEventHandler(object sender, OnDropEventArgs e);

    public class DragDropHelper
    {
        // Source and Target
        private DataFormat format = DataFormats.GetDataFormat("DragDropItemsControl");
        private Point initialMousePosition;
        private Vector initialMouseOffset;
        private object draggedData;
        private DraggedAdorner draggedAdorner;
        private InsertionAdorner insertionAdorner;
        private Window topWindow;

        // Source
        private ItemsControl sourceItemsControl;
        private FrameworkElement sourceItemContainer;

        // Target
        private ItemsControl targetItemsControl;
        private FrameworkElement targetItemContainer;
        private bool hasVerticalOrientation;
        private int insertionIndex;
        private bool isInFirstHalf;

        // Singleton DragDropHelper instance
        private static DragDropHelper instance;
        private static DragDropHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DragDropHelper();
                }
                return instance;
            }
        }

        /// <summary>
        /// Returns True if the given object is a drag source
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if obj is a drag source, else false</returns>
        public static bool GetIsDragSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragSourceProperty);
        }

        /// <summary>
        /// Sets the IsDragSource property of the given object
        /// </summary>
        /// <param name="obj">The object to set the property on</param>
        /// <param name="value">The value to set</param>
        public static void SetIsDragSource(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragSourceProperty, value);
        }

        /// <summary>
        /// DependencyProperty for the IsDragSource property
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached("IsDragSource",
                typeof(bool),
                typeof(DragDropHelper),
                new UIPropertyMetadata(false, IsDragSourceChanged));

        /// <summary>
        /// Returnes True if the given object is a drop target
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if the given object is a drop target</returns>
        public static bool GetIsDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDropTargetProperty);
        }

        /// <summary>
        /// Sets the IsDropTarget property of the given object
        /// </summary>
        /// <param name="obj">The object to set the property on</param>
        /// <param name="value">The value to set</param>
        public static void SetIsDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDropTargetProperty, value);
        }

        /// <summary>
        /// DependencyProperty for the IsDropTarget property
        /// </summary>
        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached("IsDropTarget",
                typeof(bool),
                typeof(DragDropHelper),
                new UIPropertyMetadata(false, IsDropTargetChanged));

        /// <summary>
        /// Returns the drop target items control property for the given object
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>The drop target items control for the given object</returns>
        public static FrameworkElement GetDropTargetItemsControl(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(DropTargetItemsControlProperty);
        }

        /// <summary>
        /// Sets the drop target items control property of the given object
        /// </summary>
        /// <param name="obj">The object to set the property on</param>
        /// <param name="value">The ItemsControl that will receive dropped items</param>
        public static void SetTargetItemsControl(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(DropTargetItemsControlProperty, value);
        }

        /// <summary>
        /// DependencyProperty for the DropTargetItemsControl property
        /// </summary>
        /// <remarks>
        /// The DropTargetItemsControl is the items control that will receive the dropped items
        /// If not set, it is assumed that the DropTarget itself is an items control
        /// </remarks>
        public static readonly DependencyProperty DropTargetItemsControlProperty =
            DependencyProperty.RegisterAttached("DropTargetItemsControl",
                typeof(FrameworkElement),
                typeof(DragDropHelper),
                new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the DragDrop DataTemplate for the given object
        /// </summary>
        /// <param name="obj">The object to get the property value from</param>
        /// <returns>The DragDrop DataTemplate for the object</returns>
        public static DataTemplate GetDragDropTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(DragDropTemplateProperty);
        }

        /// <summary>
        /// Sets the DragDrop DataTemplate for the given object
        /// </summary>
        /// <param name="obj">The object to set the DragDropTemplate property on</param>
        /// <param name="value">The DragDrop DataTemplate to set</param>
        public static void SetDragDropTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DragDropTemplateProperty, value);
        }

        /// <summary>
        /// DependencyProperty for the DragDropTemplate property 
        /// </summary>
        public static readonly DependencyProperty DragDropTemplateProperty =
            DependencyProperty.RegisterAttached("DragDropTemplate",
                typeof(DataTemplate),
                typeof(DragDropHelper),
                new UIPropertyMetadata(null));

        /// <summary>
        /// Returns True if the Insertion Adorner is enabled for the given object
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if the Insertion Adorner is enabled for the given object</returns>
        public static bool GetIsInsertionAdornerEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsInsertionAdornerEnabledProperty);
        }

        /// <summary>
        /// Sets the IsInsertionAdornerEnabled property of the given object
        /// </summary>
        /// <param name="obj">The object to set the property on</param>
        /// <param name="value">The value to set</param>
        public static void SetIsInsertionAdornerEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsInsertionAdornerEnabledProperty, value);
        }

        /// <summary>
        /// DependencyProperty for the IsDropTarget property
        /// </summary>
        public static readonly DependencyProperty IsInsertionAdornerEnabledProperty =
            DependencyProperty.RegisterAttached("IsInsertionAdornerEnabled",
                typeof(bool),
                typeof(DragDropHelper),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Adds a new handler to the OnDrop attached event
        /// </summary>
        public static void AddOnDropHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.AddHandler(DragDropHelper.OnDropEvent, handler);
            }
        }

        /// <summary>
        /// Removes a handler from the OnDrop attached event
        /// </summary>
        public static void RemoveOnDropHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(DragDropHelper.OnDropEvent, handler);
            }
        }

        /// <summary>
        /// OnDrop Routed Event, raised when an object is sucessfully dropped
        ///  onto a target and added to a target items control
        /// </summary>
        public static readonly RoutedEvent OnDropEvent =
            EventManager.RegisterRoutedEvent("OnDrop",
                RoutingStrategy.Direct,
                typeof(OnDropEventHandler),
                typeof(DragDropHelper));

        /// <summary>
        /// Handler for the IsDragSource Changed event
        /// </summary>
        private static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var dragSource = obj as FrameworkElement;
            if (dragSource != null)
            {
                if (Object.Equals(e.NewValue, true))
                {
                    dragSource.PreviewMouseLeftButtonDown += Instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp += Instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove += Instance.DragSource_PreviewMouseMove;
                }
                else
                {
                    dragSource.PreviewMouseLeftButtonDown -= Instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp -= Instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove -= Instance.DragSource_PreviewMouseMove;
                }
            }
        }

        /// <summary>
        /// Handler for the IsDropTarget Changed event
        /// </summary>
        private static void IsDropTargetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var dropTarget = obj as FrameworkElement;
            if (dropTarget != null)
            {
                if (Object.Equals(e.NewValue, true))
                {
                    dropTarget.AllowDrop = true;
                    dropTarget.PreviewDrop += Instance.DropTarget_PreviewDrop;
                    dropTarget.PreviewDragEnter += Instance.DropTarget_PreviewDragEnter;
                    dropTarget.PreviewDragOver += Instance.DropTarget_PreviewDragOver;
                    dropTarget.PreviewDragLeave += Instance.DropTarget_PreviewDragLeave;
                }
                else
                {
                    dropTarget.AllowDrop = false;
                    dropTarget.PreviewDrop -= Instance.DropTarget_PreviewDrop;
                    dropTarget.PreviewDragEnter -= Instance.DropTarget_PreviewDragEnter;
                    dropTarget.PreviewDragOver -= Instance.DropTarget_PreviewDragOver;
                    dropTarget.PreviewDragLeave -= Instance.DropTarget_PreviewDragLeave;
                }
            }
        }

        /// <summary>
        /// Handler for the PreviewMouseLeftButtonDown event of a DragSource ItemsControl
        /// </summary>
        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sourceItemContainer is ItemsControl)
            {
                this.sourceItemsControl = (ItemsControl)sender;
                Visual visual = e.OriginalSource as Visual;
                this.sourceItemContainer = sourceItemsControl.ContainerFromElement(visual) as FrameworkElement;
                this.topWindow = Window.GetWindow(this.sourceItemsControl);
            }
            else
            {
                this.sourceItemsControl = null;
                this.sourceItemContainer = (FrameworkElement)sender;
                this.topWindow = Window.GetWindow(this.sourceItemContainer);
            }

            this.initialMousePosition = e.GetPosition(this.topWindow);

            if (this.sourceItemContainer != null)
            {
                this.draggedData = this.sourceItemContainer.DataContext;
            }
        }

        /// <summary>
        /// Handler for the PreviewMouseMove event of a DragSource element
        /// </summary>
        private void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Drag = mouse down + move by a certain amount
            if (this.draggedData != null)
            {
                // Only drag when user moved the mouse by a reasonable amount
                Point currentPos = e.GetPosition(this.topWindow);
                if (Math.Abs(currentPos.X - this.initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(currentPos.Y - this.initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    this.initialMouseOffset = this.initialMousePosition - this.sourceItemContainer.TranslatePoint(new Point(0, 0), this.topWindow);

                    DataObject data = new DataObject(this.format.Name, this.draggedData);

                    // Add events to the window to make sure dragged adorner comes up when mouse is not over a drop target.
                    bool previousAllowDrop = this.topWindow.AllowDrop;
                    this.topWindow.AllowDrop = true;
                    this.topWindow.DragEnter += TopWindow_DragEnter;
                    this.topWindow.DragOver += TopWindow_DragOver;
                    this.topWindow.DragLeave += TopWindow_DragLeave;

                    DragDropEffects effects = System.Windows.DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);

                    // Without this call, there would be a bug in the following scenario: Click on a data item, and drag
                    // the mouse very fast outside of the window. When doing this really fast, for some reason I don't get 
                    // the Window leave event, and the dragged adorner is left behind.
                    // With this call, the dragged adorner will disappear when we release the mouse outside of the window,
                    // which is when the DoDragDrop synchronous method returns.
                    RemoveDraggedAdorner();

                    this.topWindow.AllowDrop = previousAllowDrop;
                    this.topWindow.DragEnter -= TopWindow_DragEnter;
                    this.topWindow.DragOver -= TopWindow_DragOver;
                    this.topWindow.DragLeave -= TopWindow_DragLeave;

                    this.draggedData = null;
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonUp event of a DragSource element
        /// </summary>
        private void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.draggedData = null;
        }

        /// <summary>
        /// Handles the PreviewDragEnter event of a DropTarget ItemsControl
        /// </summary>
        private void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            var fe = sender as Visual;

            var altTarget = GetDropTargetItemsControl(fe);
            if (altTarget == null)
            {
                // No alternate target itemscontrol set, assume the sender is the items control
                this.targetItemsControl = (ItemsControl)sender;
            }
            else
            {
                this.targetItemsControl = (ItemsControl)altTarget;
            }
            object draggedItem = e.Data.GetData(this.format.Name);

            DecideDropTarget(e);
            if (draggedItem != null)
            {
                // Dragged Adorner is created on the first enter only.
                ShowDraggedAdorner(e.GetPosition(this.topWindow));

                if (GetIsInsertionAdornerEnabled(fe))
                    CreateInsertionAdorner();
            }
            e.Handled = true;
        }

        /// <summary>
        /// Handles the PreviewDragOver event of a DropTarget ItemsControl
        /// </summary>
        private void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            object draggedItem = e.Data.GetData(this.format.Name);

            DecideDropTarget(e);
            if (draggedItem != null)
            {
                // Dragged Adorner is only updated here - it has already been created in DragEnter.
                ShowDraggedAdorner(e.GetPosition(this.topWindow));

                if (GetIsInsertionAdornerEnabled(sender as Visual))
                    UpdateInsertionAdornerPosition();
            }
            e.Handled = true;
        }

        /// <summary>
        /// Handles the PreviewDrop event of a DropTarget ItemsControl
        /// </summary>
        private void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            object draggedItem = e.Data.GetData(this.format.Name);
            int indexRemoved = -1;

            if (draggedItem != null)
            {
                if (this.sourceItemsControl != null)
                {
                    if ((e.Effects & DragDropEffects.Move) != 0)
                    {
                        indexRemoved = DragDropUtilities.RemoveItemFromItemsControl(this.sourceItemsControl, draggedItem);
                    }
                    // This happens when we drag an item to a later position within the same ItemsControl.
                    if (indexRemoved != -1 && this.sourceItemsControl == this.targetItemsControl && indexRemoved < this.insertionIndex)
                    {
                        this.insertionIndex--;
                    }
                }
                DragDropUtilities.InsertItemInItemsControl(this.targetItemsControl, draggedItem, this.insertionIndex);

                RemoveDraggedAdorner();

                if (GetIsInsertionAdornerEnabled(sender as Visual))
                    RemoveInsertionAdorner();

                var element = sender as FrameworkElement;
                if (element != null)
                {
                    var args = new OnDropEventArgs(OnDropEvent, e, draggedItem);
                    element.RaiseEvent(args);
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Handles the PreviewDragLeave event of a DropTarget ItemsControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            // Dragged Adorner is only created once on DragEnter + every time we enter the window. 
            // It's only removed once on the DragDrop, and every time we leave the window. (so no need to remove it here)
            object draggedItem = e.Data.GetData(this.format.Name);

            if (draggedItem != null)
            {
                if (GetIsInsertionAdornerEnabled(sender as Visual))
                    RemoveInsertionAdorner();
            }
            e.Handled = true;
        }

        /// <summary>
        /// Determines the values of the following properties:
        /// targetItemContainer, insertionIndex, and isInFirstHalf
        /// </summary>
        /// <remarks>
        /// If the types of the dragged data and ItemsControl's source are compatible, 
        /// there are 3 situations to have into account when deciding the drop target:
        /// 1. mouse is over an items container
        /// 2. mouse is over the empty part of an ItemsControl, but ItemsControl is not empty
        /// 3. mouse is over an empty ItemsControl.
        /// The goal of this method is to decide on the values of the following properties: 
        /// targetItemContainer, insertionIndex and isInFirstHalf.
        /// </remarks>
        private void DecideDropTarget(DragEventArgs e)
        {
            int targetItemsControlCount = this.targetItemsControl.Items.Count;
            object draggedItem = e.Data.GetData(this.format.Name);

            if (IsDropDataTypeAllowed(draggedItem))
            {
                if (targetItemsControlCount > 0)
                {
                    this.hasVerticalOrientation = DragDropUtilities.HasVerticalOrientation(this.targetItemsControl.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement);
                    this.targetItemContainer = targetItemsControl.ContainerFromElement((DependencyObject)e.OriginalSource) as FrameworkElement;

                    if (this.targetItemContainer != null)
                    {
                        Point positionRelativeToItemContainer = e.GetPosition(this.targetItemContainer);
                        this.isInFirstHalf = DragDropUtilities.IsInFirstHalf(this.targetItemContainer, positionRelativeToItemContainer, this.hasVerticalOrientation);
                        this.insertionIndex = this.targetItemsControl.ItemContainerGenerator.IndexFromContainer(this.targetItemContainer);

                        if (!this.isInFirstHalf)
                        {
                            this.insertionIndex++;
                        }
                    }
                    else
                    {
                        this.targetItemContainer = this.targetItemsControl.ItemContainerGenerator.ContainerFromIndex(targetItemsControlCount - 1) as FrameworkElement;
                        this.isInFirstHalf = false;
                        this.insertionIndex = targetItemsControlCount;
                    }
                }
                else
                {
                    this.targetItemContainer = null;
                    this.insertionIndex = 0;
                }
            }
            else
            {
                this.targetItemContainer = null;
                this.insertionIndex = -1;
                e.Effects = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Determines if the DropData type is allowed
        /// </summary>
        /// <param name="draggedItem">The DropData item</param>
        /// <returns>True if allowed, else false</returns>
        /// <remarks>
        /// Can the dragged data be added to the destination collection?
        /// It can if destination is bound to IList<allowed type>, IList or not data bound.
        /// </remarks>
        private bool IsDropDataTypeAllowed(object draggedItem)
        {
            bool isDropDataTypeAllowed;
            IEnumerable collectionSource = this.targetItemsControl.ItemsSource;
            if (draggedItem != null)
            {
                if (collectionSource != null)
                {
                    Type draggedType = draggedItem.GetType();
                    Type collectionType = collectionSource.GetType();

                    Type genericIListType = collectionType.GetInterface("IList`1");
                    if (genericIListType != null)
                    {
                        Type[] genericArguments = genericIListType.GetGenericArguments();
                        isDropDataTypeAllowed = genericArguments[0].IsAssignableFrom(draggedType);
                    }
                    else if (typeof(IList).IsAssignableFrom(collectionType))
                    {
                        isDropDataTypeAllowed = true;
                    }
                    else
                    {
                        isDropDataTypeAllowed = false;
                    }
                }
                else // the ItemsControl's ItemsSource is not data bound.
                {
                    isDropDataTypeAllowed = true;
                }
            }
            else
            {
                isDropDataTypeAllowed = false;
            }
            return isDropDataTypeAllowed;
        }

        /// <summary>
        /// Handles the DragEnter event of the top Window
        /// </summary>
        private void TopWindow_DragEnter(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(this.topWindow));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Handles the DragOver event of the top Window
        /// </summary>
        private void TopWindow_DragOver(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(this.topWindow));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Handles the DragLeave event of the top Window
        /// </summary>
        private void TopWindow_DragLeave(object sender, DragEventArgs e)
        {
            RemoveDraggedAdorner();
            e.Handled = true;
        }

        /// <summary>
        /// Creates or updates the dragged Adorner 
        /// </summary>
        /// <param name="currentPosition">Current position of the dragged Adorner</param>
        private void ShowDraggedAdorner(Point currentPosition)
        {
            if (this.draggedAdorner == null)
            {
                if (this.sourceItemsControl != null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(this.sourceItemsControl);
                    this.draggedAdorner = new DraggedAdorner(this.draggedData, GetDragDropTemplate(this.sourceItemsControl), this.sourceItemsControl, adornerLayer);

                }
                else
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(this.sourceItemContainer);
                    this.draggedAdorner = new DraggedAdorner(this.draggedData, GetDragDropTemplate(this.sourceItemContainer), this.sourceItemContainer, adornerLayer);
                }
            }
            this.draggedAdorner.SetPosition(currentPosition.X - this.initialMousePosition.X + this.initialMouseOffset.X, currentPosition.Y - this.initialMousePosition.Y + this.initialMouseOffset.Y);
        }

        /// <summary>
        /// Removes the dragged Adorner
        /// </summary>
        private void RemoveDraggedAdorner()
        {
            if (this.draggedAdorner != null)
            {
                this.draggedAdorner.Detach();
                this.draggedAdorner = null;
            }
        }

        /// <summary>
        /// Creates an insertion Adorner
        /// </summary>
        private void CreateInsertionAdorner()
        {
            if (this.targetItemContainer != null)
            {
                // Here, I need to get adorner layer from targetItemContainer and not targetItemsControl. 
                // This way I get the AdornerLayer within ScrollContentPresenter, and not the one under AdornerDecorator (Snoop is awesome).
                // If I used targetItemsControl, the adorner would hang out of ItemsControl when there's a horizontal scroll bar.
                var adornerLayer = AdornerLayer.GetAdornerLayer(this.targetItemContainer);
                this.insertionAdorner = new InsertionAdorner(this.hasVerticalOrientation, this.isInFirstHalf, this.targetItemContainer, adornerLayer);
            }
        }

        /// <summary>
        /// Updates the insertion Adorner position
        /// </summary>
        private void UpdateInsertionAdornerPosition()
        {
            if (this.insertionAdorner != null)
            {
                this.insertionAdorner.IsInFirstHalf = this.isInFirstHalf;
                this.insertionAdorner.InvalidateVisual();
            }
        }

        /// <summary>
        /// Removes the insertion Adorner
        /// </summary>
        private void RemoveInsertionAdorner()
        {
            if (this.insertionAdorner != null)
            {
                this.insertionAdorner.Detach();
                this.insertionAdorner = null;
            }
        }
    }
}
