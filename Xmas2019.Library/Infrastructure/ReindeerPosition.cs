using Microsoft.Azure.Cosmos.Spatial;

namespace Xmas2019.Library.Infrastructure
{
    public class ReindeerPosition
    {
        public string Id { get; set; }

        public string CountryCode { get; set; }

        public string Name { get; set; }

        public Point Location { get; set; }

        public override string ToString()
        {
            return $"{Id}, Country:{CountryCode}, Name:{Name}, Location(lat,lon):({Location.Position.Latitude},{Location.Position.Longitude})";
        }
    }
}
