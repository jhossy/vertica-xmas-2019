using Microsoft.Azure.Cosmos.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xmas2019_3.TestConsole.Infrastructure
{
    public class ReindeerPosition
    {
        public string Id { get; set; }

        public string CountryCode { get; set; }

        public string Name { get; set; }

        public Point Location { get; set; }

        public override string ToString()
        {
            return $"{Id}, {CountryCode}, {Name}";//, ({Location.Position.Latitude},{Location.Position.Longitude})";
        }
    }

    //public class Location
    //{
    //    public string Type { get; set; }

    //    public GeoPoint Coordinates { get; set; }
    //}
}
