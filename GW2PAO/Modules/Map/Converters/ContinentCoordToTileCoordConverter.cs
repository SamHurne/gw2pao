
using MapControl;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GW2PAO.Modules.Map.Converters
{
    /// <summary>
    /// Converts a location from a continent's coordinant
    /// system to latitude/longitude coordinates for use in a map
    /// </summary>
    public class ContinentCoordToTileCoordConverter : IMultiValueConverter
    {
        private MercatorTransform transform = new MercatorTransform();

        /// <summary>
        /// Converts from Continent Coordinates to a Location object containing Latitude/Longitude data
        /// </summary>
        /// <param name="values">
        /// values[0] should be the Point data to convert
        /// values[1] should be the Continent that the Point is located within
        /// </param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >=2
                && values[0] is GW2PAO.API.Data.Entities.Point
                && values[1] is API.Data.Entities.Continent)
            {
                var point = values[0] as GW2PAO.API.Data.Entities.Point;
                var continent = values[1] as API.Data.Entities.Continent;

                var location = transform.Transform(new System.Windows.Point(
                        (point.X - (continent.Width / 2)) / continent.Width * 360.0,
                        ((continent.Height / 2) - point.Y) / continent.Height * 360.0));

                return location;
            }
            else
            {
                throw new InvalidOperationException("Invalid input to MapCoordToWorldCoordConverter!");
            }
        }

        /// <summary>
        /// Not Supported
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
