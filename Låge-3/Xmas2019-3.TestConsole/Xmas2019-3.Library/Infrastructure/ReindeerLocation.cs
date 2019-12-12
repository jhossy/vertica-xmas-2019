using Xmas2019_3.Library.Infrastructure.Geo;

namespace Xmas2019_3.Library.Infrastructure
{
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
