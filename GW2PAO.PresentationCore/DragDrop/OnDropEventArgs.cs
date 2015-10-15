using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GW2PAO.PresentationCore.DragDrop
{
    public class OnDropEventArgs : RoutedEventArgs
    {
        private DragEventArgs dragEventArgs;

        /// <summary>
        /// Data associated with the dropped item
        /// </summary>
        public object Data
        {
            get;
            private set;
        }

        public OnDropEventArgs(DragEventArgs dragEventArgs, object data)
            : base()
        {
            this.dragEventArgs = dragEventArgs;
            this.Data = data;
        }

        public OnDropEventArgs(RoutedEvent routedEvent, DragEventArgs dragEventArgs, object data)
            : base(routedEvent)
        {
            this.dragEventArgs = dragEventArgs;
            this.Data = data;
        }

        public OnDropEventArgs(RoutedEvent routedEvent, object source, DragEventArgs dragEventArgs, object data)
            : base(routedEvent, source)
        {
            this.dragEventArgs = dragEventArgs;
            this.Data = data;
        }

        public Point GetPosition(IInputElement relativeTo)
        {
            return this.dragEventArgs.GetPosition(relativeTo);
        }
    }
}
