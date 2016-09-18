using MapControl;

namespace GW2PAO.Modules.Map.Models
{
    public class MapMarker
    {
        /// <summary>
        /// Path to the icon image for the marker
        /// This can be either a path to an embedded image, or the path to an image on the web
        /// </summary>
        public string Icon;

        /// <summary>
        /// Name or Description to show for the marker
        /// </summary>
        public string Name;

        /// <summary>
        /// Location of the MapMarker on the Map
        /// </summary>
        public Location Location;
    }
}
