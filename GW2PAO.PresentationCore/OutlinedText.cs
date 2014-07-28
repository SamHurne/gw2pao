using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace GW2PAO.PresentationCore
{
    [ContentProperty("Text")]
    public class OutlinedTextBlock : FrameworkElement
    {
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill",
            typeof(Brush),
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke",
            typeof(SolidColorBrush),
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness",
            typeof(double),
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeOpacityProperty = DependencyProperty.Register(
            "StrokeOpacity",
            typeof(double),
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(OnFormattedTextInvalidated));

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            "TextAlignment",
            typeof(TextAlignment),
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(
            "TextDecorations",
            typeof(TextDecorationCollection),
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
            "TextTrimming",
            typeof(TextTrimming),
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
            "TextWrapping",
            typeof(TextWrapping),
            typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(TextWrapping.NoWrap, OnFormattedTextUpdated));

        private FormattedText formattedText;
        private Geometry textGeometry;

        public OutlinedTextBlock()
        {
            this.TextDecorations = new TextDecorationCollection();
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public double StrokeOpacity
        {
            get { return (double)GetValue(StrokeOpacityProperty); }
            set { SetValue(StrokeOpacityProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)this.GetValue(TextDecorationsProperty); }
            set { this.SetValue(TextDecorationsProperty, value); }
        }

        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            this.EnsureGeometry();

            SolidColorBrush strokeBrush = new SolidColorBrush(this.Stroke.Color);
            strokeBrush.Opacity = this.StrokeOpacity;

            Pen pop = new Pen(strokeBrush, StrokeThickness);
            pop.LineJoin = PenLineJoin.Round;

            drawingContext.DrawGeometry(null, pop, this.textGeometry);
            drawingContext.DrawText(this.formattedText, new Point(0, 0));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Text == null)
            {
                return new Size(0, 0);
            }
            else
            {
                this.EnsureFormattedText();
                this.UpdateFormattedText();

                // constrain the formatted text according to the available size
                // the Math.Min call is important - without this constraint (which seems arbitrary, but is the maximum allowable text width), things blow up when availableSize is infinite in both directions
                // the Math.Max call is to ensure we don't hit zero, which will cause MaxTextHeight to throw
                this.formattedText.MaxTextWidth = Math.Min(3579139, availableSize.Width);
                this.formattedText.MaxTextHeight = Math.Max(0.0001d, availableSize.Height);

                // return the desired size
                return new Size(this.formattedText.Width, this.formattedText.Height);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Text == null)
            {
                return finalSize;
            }
            else
            {
                this.EnsureFormattedText();
                this.UpdateFormattedText();

                // update the formatted text with the final size
                this.formattedText.MaxTextWidth = finalSize.Width;
                this.formattedText.MaxTextHeight = finalSize.Height;

                // need to re-generate the geometry now that the dimensions have changed
                this.textGeometry = null;

                return finalSize;
            }
        }

        private static void OnFormattedTextInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (OutlinedTextBlock)dependencyObject;
            outlinedTextBlock.formattedText = null;
            outlinedTextBlock.textGeometry = null;

            outlinedTextBlock.InvalidateMeasure();
            outlinedTextBlock.InvalidateVisual();
        }

        private static void OnFormattedTextUpdated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (OutlinedTextBlock)dependencyObject;
            outlinedTextBlock.EnsureFormattedText();
            outlinedTextBlock.UpdateFormattedText();
            outlinedTextBlock.textGeometry = null;

            outlinedTextBlock.InvalidateMeasure();
            outlinedTextBlock.InvalidateVisual();
        }

        private void EnsureFormattedText()
        {
            if (this.formattedText != null || this.Text == null)
            {
                return;
            }

            this.formattedText = new FormattedText(
                this.Text,//.Replace(' ', System.Convert.ToChar(160)),
                CultureInfo.CurrentUICulture,
                this.FlowDirection,
                new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, FontStretches.Normal),
                this.FontSize,
                this.Fill,
                null,
                TextFormattingMode.Display);
        }

        private void UpdateFormattedText()
        {
            if (this.formattedText == null)
            {
                return;
            }

            this.formattedText.TextAlignment = this.TextAlignment;
            if (this.TextWrapping != System.Windows.TextWrapping.NoWrap)
                this.formattedText.Trimming = System.Windows.TextTrimming.None;

            this.formattedText.SetFontSize(this.FontSize);
            this.formattedText.SetFontStyle(this.FontStyle);
            this.formattedText.SetFontWeight(this.FontWeight);
            this.formattedText.SetFontFamily(this.FontFamily);
            this.formattedText.SetFontStretch(this.FontStretch);
            this.formattedText.SetTextDecorations(this.TextDecorations);
            this.formattedText.SetForegroundBrush(this.Fill);
        }

        private void EnsureGeometry()
        {
            if (this.textGeometry != null || this.Text == null)
            {
                return;
            }

            this.EnsureFormattedText();
            this.textGeometry = this.formattedText.BuildGeometry(new Point(0, 0));
        }
    }
}
