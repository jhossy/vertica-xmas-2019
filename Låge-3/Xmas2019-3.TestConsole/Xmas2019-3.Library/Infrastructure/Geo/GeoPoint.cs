namespace Xmas2019_3.Library.Infrastructure.Geo
{
    public class GeoPoint
    {
        public double lat { get; set; }

        public double lon { get; set; }

        public GeoPoint(double inputLat, double inputLon)
        {
            lat = inputLat;
            lon = inputLon;
        }

        public override string ToString()
        {
            return $"({lat},{lon})";
        }
    }
}
