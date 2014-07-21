#define USE_WEAK_EVENTS

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace GW2PAO.PresentationCore
{
    /// <summary>
    ///  Class used to implement a collection view source which is able to automatically request a smart refresh an the associated view when an item changes.
    /// </summary>
    public class AutoRefreshCollectionViewSource : CollectionViewSource
    {
        /// <summary>
        ///  Called when the source has changed.
        /// </summary>
        /// <param name = "oldSource">The old source.</param>
        /// <param name = "newSource">The new source.</param>
        protected override void OnSourceChanged(object oldSource, object newSource)
        {
            if (oldSource != null)
                UnsubscribeSourceEvents(oldSource);

            if (newSource != null)
                SubscribeSourceEvents(newSource);

            base.OnSourceChanged(oldSource, newSource);
        }

        /// <summary>
        ///  Unsubscribes from the source events.
        /// </summary>
        /// <param name = "source">The source.</param>
        private void UnsubscribeSourceEvents(object source)
        {
            var notify = source as INotifyCollectionChanged;

            if (notify != null)
#if USE_WEAK_EVENTS
                NotifyCollectionChangedWeakEventManager.RemoveListener(notify, this);
#else
        notify.CollectionChanged -= OnSourceCollectionChanged;
#endif

            if (source is IEnumerable)
                UnsubscribeItemsEvents((IEnumerable)source);
        }

        /// <summary>
        ///  Subscribes to the source events.
        /// </summary>
        /// <param name = "source">The source.</param>
        private void SubscribeSourceEvents(object source)
        {
            var notify = source as INotifyCollectionChanged;

            if (notify != null)
#if USE_WEAK_EVENTS
                NotifyCollectionChangedWeakEventManager.AddListener(notify, this);
#else
        notify.CollectionChanged += OnSourceCollectionChanged;
#endif

            if (source is IEnumerable)
                SubscribeItemsEvents((IEnumerable)source);
        }

        /// <summary>
        ///  Unsubscribes from the item events.
        /// </summary>
        /// <param name = "item">The item.</param>
        private void UnsubscribeItemEvents(object item)
        {
            var notify = item as INotifyPropertyChanged;

            if (notify != null)
#if USE_WEAK_EVENTS
                NotifyPropertyChangedWeakEventManager.RemoveListener(notify, this);
#else
        notify.PropertyChanged -= OnItemPropertyChanged;
#endif
        }

        /// <summary>
        ///  Subscribes to the item events.
        /// </summary>
        /// <param name = "item">The item.</param>
        private void SubscribeItemEvents(object item)
        {
            var notify = item as INotifyPropertyChanged;

            if (notify != null)
#if USE_WEAK_EVENTS
                NotifyPropertyChangedWeakEventManager.AddListener(notify, this);
#else
        notify.PropertyChanged += OnItemPropertyChanged;
#endif
        }

        /// <summary>
        ///  Unsubscribes from the items events.
        /// </summary>
        /// <param name = "items">The items.</param>
        private void UnsubscribeItemsEvents(IEnumerable items)
        {
            foreach (object item in items)
                UnsubscribeItemEvents(item);
        }

        /// <summary>
        ///  Subscribes to the items events.
        /// </summary>
        /// <param name = "items">The items.</param>
        private void SubscribeItemsEvents(IEnumerable items)
        {
            foreach (object item in items)
                SubscribeItemEvents(item);
        }

#if USE_WEAK_EVENTS
        /// <summary>
        ///  Handles events from the centralized event table.
        /// </summary>
        /// <param name = "managerType">The type of the <see cref = "T:System.Windows.WeakEventManager" /> calling this method. This only recognizes manager objects of type <see cref = "T:System.Windows.Data.DataChangedEventManager" />.</param>
        /// <param name = "sender">Object that originated the event.</param>
        /// <param name = "e">Event data.</param>
        /// <returns>
        ///  true if the listener handled the event; otherwise, false.
        /// </returns>
        protected override bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(NotifyPropertyChangedWeakEventManager))
            {
                OnItemPropertyChanged(sender, (PropertyChangedEventArgs)e);
                return true;
            }
            if (managerType == typeof(NotifyCollectionChangedWeakEventManager))
            {
                OnSourceCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
                return true;
            }
            return base.ReceiveWeakEvent(managerType, sender, e);
        }
#endif

        /// <summary>
        ///  Called when a source collection has changed.
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#if !USE_WEAK_EVENTS
      //If the collection view is cleared, events should be detached.
      //Failing to do so would case the collection view itself to linger in memory until all the removed items are garbage collected.
      //If weak events are used, there is not such risk, so it safe to ignore the problem.
      //Note anyway that if both the collection view and the removed objects are used after a Reset operation, it is possible
      //for the collection view to process unneeded events. This should just a bit of unneeded overhead.
      if (e.Action == NotifyCollectionChangedAction.Reset)
        throw new InvalidOperationException(string.Format("The action {0} is not supported by {1}", e.Action, GetType()));
#endif
            if (e.NewItems != null)
                SubscribeItemsEvents(e.NewItems);
            if (e.OldItems != null)
                UnsubscribeItemsEvents(e.OldItems);
        }

        /// <summary>
        ///  Called when an item property has changed.
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsViewRefreshNeeded(e.PropertyName))
            {
                var view = View;
                if (view != null)
                {
                    var current = view.CurrentItem;
                    var editableCollectionView = view as IEditableCollectionView;

                    if (editableCollectionView != null)
                    {
                        editableCollectionView.EditItem(sender);
                        editableCollectionView.CommitEdit();
                    }
                    else
                        view.Refresh();
                    view.MoveCurrentTo(current);
                    //Ensure that the previously current item is maintained after the refresh operation
                }
            }
        }

        /// <summary>
        ///  Determines whether a view refresh is needed.
        /// </summary>
        /// <param name = "propertyName">The name of the changed property.</param>
        /// <returns>
        ///  <c>True</c> if a view refresh needed; otherwise, <c>false</c>.
        /// </returns>
        private bool IsViewRefreshNeeded(string propertyName)
        {
            return SortDescriptions.Any(sort => string.Equals(sort.PropertyName, propertyName)) || GroupDescriptions.OfType<PropertyGroupDescription>().Where(g => string.Equals(g.PropertyName, propertyName)).Any();
        }

#if USE_WEAK_EVENTS

        #region Nested type: NotifyCollectionChangedWeakEventManager
        /// <summary>
        ///  Class used to create a weak event manager able to handle <see cref = "INotifyCollectionChanged.CollectionChanged" /> events.
        /// </summary>
        protected class NotifyCollectionChangedWeakEventManager : WeakEventManager
        {
            #region Static Properties
            /// <summary>
            ///  Gets the current manager.
            /// </summary>
            /// <value>The current manager.</value>
            public static NotifyCollectionChangedWeakEventManager CurrentManager
            {
                get
                {
                    var manager = (NotifyCollectionChangedWeakEventManager)GetCurrentManager(typeof(NotifyCollectionChangedWeakEventManager));

                    if (manager == null)
                        SetCurrentManager(typeof(NotifyCollectionChangedWeakEventManager), (manager = new NotifyCollectionChangedWeakEventManager()));

                    return manager;
                }
            }
            #endregion

            #region Static Members
            /// <summary>
            ///  Adds the provided listener to the provided source for the event being managed.
            /// </summary>
            /// <param name = "source">The source.</param>
            /// <param name = "listener">The listener.</param>
            public static void AddListener(INotifyCollectionChanged source, IWeakEventListener listener)
            {
                CurrentManager.ProtectedAddListener(source, listener);
            }

            /// <summary>
            ///  Removes a previously added listener from the provided source.
            /// </summary>
            /// <param name = "source">The source.</param>
            /// <param name = "listener">The listener.</param>
            public static void RemoveListener(INotifyCollectionChanged source, IWeakEventListener listener)
            {
                CurrentManager.ProtectedRemoveListener(source, listener);
            }
            #endregion

            /// <summary>
            ///  When overridden in a derived class, starts listening for the event being managed. After <see cref = "M:System.Windows.WeakEventManager.StartListening(System.Object)" /> is first called, the manager should be in the state of calling <see cref = "M:System.Windows.WeakEventManager.DeliverEvent(System.Object,System.EventArgs)" /> or <see cref = "M:System.Windows.WeakEventManager.DeliverEventToList(System.Object,System.EventArgs,System.Windows.WeakEventManager.ListenerList)" /> whenever the relevant event from the provided source is handled.
            /// </summary>
            /// <param name = "source">The source to begin listening on.</param>
            protected override void StartListening(object source)
            {
                var sender = source as INotifyCollectionChanged;
                if (sender != null)
                    sender.CollectionChanged += OnCollectionChanged;
            }

            /// <summary>
            ///  Called when the collection has changed.
            /// </summary>
            /// <param name = "sender">The sender.</param>
            /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                DeliverEvent(sender, e);
            }

            /// <summary>
            ///  When overridden in a derived class, stops listening on the provided source for the event being managed.
            /// </summary>
            /// <param name = "source">The source to stop listening on.</param>
            protected override void StopListening(object source)
            {
                var sender = source as INotifyCollectionChanged;
                if (sender != null)
                    sender.CollectionChanged -= OnCollectionChanged;
            }
        }
        #endregion

        #region Nested type: NotifyPropertyChangedWeakEventManager
        /// <summary>
        ///  Class used to create a weak event manager able to handle <see cref = "INotifyPropertyChanged.PropertyChanged" /> events.
        /// </summary>
        protected class NotifyPropertyChangedWeakEventManager : WeakEventManager
        {
            #region Static Properties
            /// <summary>
            ///  Gets the current manager.
            /// </summary>
            /// <value>The current manager.</value>
            public static NotifyPropertyChangedWeakEventManager CurrentManager
            {
                get
                {
                    var manager = (NotifyPropertyChangedWeakEventManager)GetCurrentManager(typeof(NotifyPropertyChangedWeakEventManager));

                    if (manager == null)
                        SetCurrentManager(typeof(NotifyPropertyChangedWeakEventManager), (manager = new NotifyPropertyChangedWeakEventManager()));

                    return manager;
                }
            }
            #endregion

            #region Static Members
            /// <summary>
            ///  Adds the provided listener to the provided source for the event being managed.
            /// </summary>
            /// <param name = "source">The source.</param>
            /// <param name = "listener">The listener.</param>
            public static void AddListener(INotifyPropertyChanged source, IWeakEventListener listener)
            {
                CurrentManager.ProtectedAddListener(source, listener);
            }

            /// <summary>
            ///  Removes a previously added listener from the provided source.
            /// </summary>
            /// <param name = "source">The source.</param>
            /// <param name = "listener">The listener.</param>
            public static void RemoveListener(INotifyPropertyChanged source, IWeakEventListener listener)
            {
                CurrentManager.ProtectedRemoveListener(source, listener);
            }
            #endregion

            /// <summary>
            ///  When overridden in a derived class, starts listening for the event being managed. After <see cref = "M:System.Windows.WeakEventManager.StartListening(System.Object)" /> is first called, the manager should be in the state of calling <see cref = "M:System.Windows.WeakEventManager.DeliverEvent(System.Object,System.EventArgs)" /> or <see cref = "M:System.Windows.WeakEventManager.DeliverEventToList(System.Object,System.EventArgs,System.Windows.WeakEventManager.ListenerList)" /> whenever the relevant event from the provided source is handled.
            /// </summary>
            /// <param name = "source">The source to begin listening on.</param>
            protected override void StartListening(object source)
            {
                var sender = source as INotifyPropertyChanged;
                if (sender != null)
                    sender.PropertyChanged += OnPropertyChanged;
            }

            /// <summary>
            ///  Called when the property has changed.
            /// </summary>
            /// <param name = "sender">The sender.</param>
            /// <param name = "e">The <see cref = "System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                DeliverEvent(sender, e);
            }

            /// <summary>
            ///  When overridden in a derived class, stops listening on the provided source for the event being managed.
            /// </summary>
            /// <param name = "source">The source to stop listening on.</param>
            protected override void StopListening(object source)
            {
                var sender = source as INotifyPropertyChanged;
                if (sender != null)
                    sender.PropertyChanged -= OnPropertyChanged;
            }
        }
        #endregion

#endif
    }
}