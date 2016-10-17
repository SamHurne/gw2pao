using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GW2PAO.Modules.Map.Views.Controls
{
    public class Map : MapBase
    {
        public static readonly DependencyProperty ManipulationModeProperty = DependencyProperty.Register(
            "ManipulationMode", typeof(ManipulationModes), typeof(Map), new PropertyMetadata(ManipulationModes.All));

        private static readonly DependencyProperty mouseWheelZoomDeltaProperty = DependencyProperty.Register(
            "MouseWheelZoomDelta", typeof(double), typeof(Map), new PropertyMetadata(1d));

        private System.Windows.Point? mousePosition;

        static Map()
        {
            IsManipulationEnabledProperty.OverrideMetadata(typeof(Map), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Gets or sets a value that specifies how the map control handles manipulations.
        /// </summary>
        public ManipulationModes ManipulationMode
        {
            get { return (ManipulationModes)GetValue(ManipulationModeProperty); }
            set { SetValue(ManipulationModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the amount by which the ZoomLevel property changes during a MouseWheel event.
        /// </summary>
        public double MouseWheelZoomDelta
        {
            get { return (double)GetValue(MouseWheelZoomDeltaProperty); }
            set { SetValue(MouseWheelZoomDeltaProperty, value); }
        }

        public static DependencyProperty MouseWheelZoomDeltaProperty
        {
            get
            {
                return mouseWheelZoomDeltaProperty;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            var zoomDelta = MouseWheelZoomDelta * (double)e.Delta / 120d;
            ZoomMap(e.GetPosition(this), TargetZoomLevel + zoomDelta);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (this.ManipulationMode != ManipulationModes.None)
            {
                if (CaptureMouse())
                {
                    mousePosition = e.GetPosition(this);
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (mousePosition.HasValue)
            {
                mousePosition = null;
                ReleaseMouseCapture();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (mousePosition.HasValue)
            {
                var position = e.GetPosition(this);
                TranslateMap((System.Windows.Point)(position - mousePosition.Value));
                mousePosition = position;
            }
        }

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            Manipulation.SetManipulationMode(this, ManipulationMode);
            base.OnManipulationStarted(e);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);

            TransformMap(e.ManipulationOrigin,
                (System.Windows.Point)e.DeltaManipulation.Translation, e.DeltaManipulation.Rotation,
                (e.DeltaManipulation.Scale.X + e.DeltaManipulation.Scale.Y) / 2d);
        }
    }
}
