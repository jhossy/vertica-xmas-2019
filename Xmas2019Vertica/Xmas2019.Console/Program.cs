using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using Xmas2019.Library.Infrastructure;
using Xmas2019.Library.Infrastructure.Search;

namespace Xmas2019.TestConsole
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            string userName = "Participant";
            string password = "fr5ZS6NT2gQE1VL0hLZmB1X8HhGAW4";
            string cloudId = "xmas2019:ZXUtY2VudHJhbC0xLmF3cy5jbG91ZC5lcy5pbyRlZWJjNmYyNzcxM2Q0NTE5OTcwZDc1Yzg2MDUwZTM2MyQyNDFmMzQ3OWNkNzg0ZTUyOTRkODk5OTViMjg0MjAyYg==";

            string index = "santa-trackings";
            string documentId = "e8c79e4f-8468-44ba-b69d-975fcb244a42";

            Console.WriteLine("Creating search client...");

            SearchClient client = new SearchClient(userName, password, cloudId);

            Console.WriteLine("Fetching document...");

            SantaInformation santaInfo = client.GetDocument(index, documentId);

            //Console.WriteLine("Santainfo: " + santaInfo.ToString());

            Console.WriteLine("Converting to meters...");

            IEnumerable<SantaMovement> alignedSantaMovements = santaInfo.ConvertAllMovementsToMeters();

            Console.WriteLine("Moving santa...");

            Console.WriteLine($"Starting from: {santaInfo.CanePosition.ToString()}");

            CanePosition currentPosition = santaInfo.CanePosition;
            CanePosition newPosition = new CanePosition() { lat = 0, lon = 0 };            

            foreach (SantaMovement move in alignedSantaMovements)
            {
                double xMeters = 0d;
                double yMeters = 0d;

                switch (move.Direction)
                {
                    case "left":
                        xMeters -= move.Value;
                        break;
                    case "right":
                        xMeters += move.Value;
                        break;
                    case "up":
                        yMeters += move.Value;
                        break;
                    case "down":
                        yMeters -= move.Value;
                        break;
                    default:
                        throw new Exception("direction not supported");
                }

                newPosition.lon = LocationCalculator.MoveX(currentPosition.lon, xMeters, currentPosition.lat);
                newPosition.lat = LocationCalculator.MoveY(currentPosition.lat, yMeters);

                currentPosition = new CanePosition() { lat = newPosition.lat, lon = newPosition.lon };
            }

            Console.WriteLine($"lat:{currentPosition.lat.ToString(CultureInfo.GetCultureInfo("en-US"))}, lon: {currentPosition.lon.ToString(CultureInfo.GetCultureInfo("en-US"))}");

            var res = PostAnswer(documentId, newPosition);

            //Test();

            Console.ReadLine();
        }

        private static HttpContent PostAnswer(string id, CanePosition pos)
        {
            SendAnswer answer = new SendAnswer() { Id = id, Position = pos };
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();


            string str = JsonConvert.SerializeObject(answer, serializerSettings);

            //StringContent content = new StringContent(JsonConvert.SerializeObject(answer, serializerSettings), System.Text.Encoding.UTF8, "application/json");

            //HttpResponseMessage response = client.PostAsync("https://vertica-xmas2019.azurewebsites.net/api/santarescue", content).Result;
            //   return response.Content;
            return null;
        }

        private static void Test()
        {
            double currentLon = 71.639566053691;
            double currentLat = -51.1902823595313;

            //CanePosition currPos = new CanePosition() { lat = currentLat, lon = currentLon };
            //var res = LocationCalculator.MoveSanta(currPos, new SantaMovement() { Direction = "right", Unit = "meter", Value = 10000 });
            //var res1 = LocationCalculator.MoveSanta(res, new SantaMovement() { Direction = "down", Unit = "meter", Value = 7500 });
            double lon = LocationCalculator.MoveX(currentLat, 10000, currentLon);
            double lat = LocationCalculator.MoveY(currentLon, -7500);

            Console.WriteLine($"lat:{lat}, lon: {lon}");
        }
    }

    public class SendAnswer
    {
        public string Id { get; set; }

        public CanePosition Position { get; set; }
    }
}
