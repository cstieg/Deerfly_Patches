using Deerfly_Patches.Models;

namespace Deerfly_Patches.Modules.Geography
{
    public class GeoRange
    {
        public LatLng TopLeft { get; set; }

        public LatLng BottomRight { get; set; }

        public GeoRange(float minLat, float leftLng, float maxLat, float rightLng)
        {
            TopLeft = new LatLng(minLat, leftLng);
            BottomRight = new LatLng(maxLat, rightLng);
        }

        public GeoRange(LatLng topLeft, LatLng bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }
    }
}