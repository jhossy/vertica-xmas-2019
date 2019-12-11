using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Xmas2019.Library.Infrastructure;
using Xmas2019.Library.Infrastructure.Search;

namespace Xmas2019.TestConsole
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://vertica-xmas2019.azurewebsites.net");

                HttpResponseMessage apiResponse = await httpClient.PostAsJsonAsync("/api/participate", new { fullName = "Jesper Hossy", emailAddress = "jesh@widex.com", subscribeToNewsletter = false });

                if (!apiResponse.IsSuccessStatusCode)
                    throw new InvalidOperationException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

                var participateResponse = await apiResponse.Content.ReadAsAsync<ParticipateResponse>();

                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

                string userName = "Participant";
                string password = "fr5ZS6NT2gQE1VL0hLZmB1X8HhGAW4";
                string cloudId = "xmas2019:ZXUtY2VudHJhbC0xLmF3cy5jbG91ZC5lcy5pbyRlZWJjNmYyNzcxM2Q0NTE5OTcwZDc1Yzg2MDUwZTM2MyQyNDFmMzQ3OWNkNzg0ZTUyOTRkODk5OTViMjg0MjAyYg==";

                string index = "santa-trackings";
                string documentId = participateResponse.Id.ToString();

                Console.WriteLine("Creating search client...");

                SearchClient client = new SearchClient(userName, password, cloudId);

                Console.WriteLine("Fetching document...");

                SantaInformation santaInfo = client.GetDocument(index, documentId);

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

                //Test();



                apiResponse = await httpClient.PostAsJsonAsync("/api/santarescue", new { id = participateResponse.Id, position = currentPosition });

                if (!apiResponse.IsSuccessStatusCode)
                    throw new InvalidOperationException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

                Console.WriteLine(await apiResponse.Content.ReadAsStringAsync());

                Console.ReadLine();
            }
        }

        private static void Test()
        {
            double currentLon = 71.639566053691;
            double currentLat = -51.1902823595313;

            double lon = LocationCalculator.MoveX(currentLat, 10000, currentLon);
            double lat = LocationCalculator.MoveY(currentLon, -7500);

            Console.WriteLine($"lat:{lat}, lon: {lon}");
        }
    }

    public class ParticipateResponse
    {
        public Guid Id { get; set; }
        public Credentials Credentials { get; set; }
    }

    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
