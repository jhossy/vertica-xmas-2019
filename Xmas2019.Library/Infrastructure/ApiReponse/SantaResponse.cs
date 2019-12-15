using Xmas2019.Library.Infrastructure.Geo;

namespace Xmas2019.Library.Infrastructure.ApiReponse
{
    public class SantaResponse
    {
        public Zone[] Zones { get; set; }
        public string Token { get; set; }

        public static SantaResponse Empty()
        {
            return new SantaResponse() { Zones = new Zone[0], Token = string.Empty };
        }
    }
}
