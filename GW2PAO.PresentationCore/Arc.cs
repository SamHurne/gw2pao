using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GW2PAO.PresentationCore
{
    /// <summary>
    /// Basic Arc shape
    /// 
    /// Based on http://stackoverflow.com/a/23047288
    /// </summary>
    public class Arc : Shape
    {
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register("StartAngle",
                typeof(double),
                typeof(Arc),
                new UIPropertyMetadata(0.0, new PropertyChangedCallback(UpdateArc)));

        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register("EndAngle",
                typeof(double),
                typeof(Arc),
                new UIPropertyMetadata(90.0, new PropertyChangedCallback(UpdateArc)));

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction",
            typeof(SweepDirection),
            typeof(Arc),
            new UIPropertyMetadata(SweepDirection.Clockwise));

        public static readonly DependencyProperty OriginRotationDegreesProperty = DependencyProperty.Register("OriginRotationDegrees",
            typeof(double),
            typeof(Arc),
            new UIPropertyMetadata(270.0, new PropertyChangedCallback(UpdateArc)));

        /// <summary>
        /// The starting point angle of the arc
        /// </summary>
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        /// <summary>
        /// The ending point angle of the arc
        /// </summary>
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        /// <summary>
        /// Sweep direction (clockwise vs counter-clockwise) of the arc
        /// </summary>
        public SweepDirection Direction
        {
            get { return (SweepDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Start/end rotation of the arc
        /// Ex: to start at 12:00, this value should be 270 (clockwise) or 90 (counterclockwise)
        /// </summary>
        public double OriginRotationDegrees
        {
            get { return (double)GetValue(OriginRotationDegreesProperty); }
            set { SetValue(OriginRotationDegreesProperty, value); }
        }

        /// <summary>
        /// Gets a value that represents the System.Windows.Media.Geometry of the System.Windows.Shapes.Shape.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get { return this.GetArcGeometry(); }
        }


        protected static void UpdateArc(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Arc arc = d as Arc;
            arc.InvalidateVisual();
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(null, new Pen(this.Stroke, this.StrokeThickness), this.GetArcGeometry());
        }

        private Geometry GetArcGeometry()
        {
            Point startPoint = PointAtAngle(Math.Min(this.StartAngle, this.EndAngle), this.Direction);
            Point endPoint = PointAtAngle(Math.Max(this.StartAngle, this.EndAngle), this.Direction);

            Size arcSize = new Size(
                Math.Max(0, (this.RenderSize.Width - this.StrokeThickness) / 2),
                Math.Max(0, (this.RenderSize.Height - this.StrokeThickness) / 2));

            bool isLargeArc = Math.Abs(this.EndAngle - this.StartAngle) > 180;

            StreamGeometry geom = new StreamGeometry();
            using (StreamGeometryContext context = geom.Open())
            {
                context.BeginFigure(startPoint, false, false);
                context.ArcTo(endPoint, arcSize, 0, isLargeArc, this.Direction, true, false);
            }
            geom.Transform = new TranslateTransform(this.StrokeThickness / 2, this.StrokeThickness / 2);
            return geom;
        }

        private Point PointAtAngle(double angle, SweepDirection sweep)
        {
            double translatedAngle = angle + OriginRotationDegrees;
            double radAngle = translatedAngle * (Math.PI / 180);
            double xr = (this.RenderSize.Width - this.StrokeThickness) / 2;
            double yr = (this.RenderSize.Height - this.StrokeThickness) / 2;

            double x = xr + xr * Math.Cos(radAngle);
            double y = yr * Math.Sin(radAngle);

            if (sweep == SweepDirection.Counterclockwise)
            {
                y = yr - y;
            }
            else
            {
                y = yr + y;
            }

            return new Point(x, y);
        }
    }
}