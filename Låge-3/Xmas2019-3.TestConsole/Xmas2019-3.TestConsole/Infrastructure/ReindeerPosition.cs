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
            return $"{Id}, Country:{CountryCode}, Name:{Name}, Location(lat,lon):({Location.Position.Latitude},{Location.Position.Longitude})";
        }
    }

    public class ReindeerLocation
    {
        public string Name { get; set; }
        public GeoPoint Position { get; set; }
    }

    //public class ReindeerLocated
    //{
    //    public string name { get; set; }

    //    public GeoPoint position { get; set; }
    //}

    //public class ReindeerRequest
    //{
    //    public string id { get; set; }

    //    public List<ReindeerLocated> locations { get; set; }

    //    public ReindeerRequest(string inputId, List<ReindeerPosition> positions)
    //    {
    //        id = inputId;

    //        locations = new List<ReindeerLocated>();

    //        foreach(var pos in positions)
    //        {
    //            locations.Add(new ReindeerLocated() { name = pos.Name, position = new GeoPoint() { lat = pos.Location.Position.Latitude, lon = pos.Location.Position.Longitude } });
    //        }
    //    }
    //}

    //public class Location
    //{
    //    public string Type { get; set; }

    //    public GeoPoint Coordinates { get; set; }
    //}
}
