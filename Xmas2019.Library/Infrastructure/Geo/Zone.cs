﻿using Xmas2019.Library.Infrastructure.Geo;

namespace Xmas2019.Library.Infrastructure.Geo
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
