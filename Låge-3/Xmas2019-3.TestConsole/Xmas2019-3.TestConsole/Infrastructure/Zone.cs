using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xmas2019_3.TestConsole.Infrastructure
{
    public class Zone
    {
        public string Reindeer { get; set; }

        public string CountryCode { get; set; }

        public string CityName { get; set; }

        public GeoPoint Center { get; set; }

        public Radius Radius { get; set; }

        public Zone NormalizeRadius()
        {
            Zone zone = new Zone();
            zone.Reindeer = Reindeer;
            zone.CountryCode = CountryCode;
            zone.CityName = CityName;
            zone.Center = Center;
            zone.Radius = Radius.Normalize();
            return zone;
        }

        public override string ToString()
        {
            return $"{CountryCode}-{CityName} {Reindeer}, Center: ({Center.lat},{Center.lon}), Radius: {Radius.Value} {Radius.Unit}";
        }
    }
}
