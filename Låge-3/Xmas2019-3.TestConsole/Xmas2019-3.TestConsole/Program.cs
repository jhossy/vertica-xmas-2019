using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xmas2019_3.Library.Infrastructure;
using Xmas2019_3.Library.Infrastructure.Geo;
using Xmas2019_3.Library.Infrastructure.Search;

namespace Xmas2019_3.Library
{
    partial class Program
    {
        private static FileReader _fileReader = new FileReader();

        private static readonly string _endpointUri = "https://xmas2019.documents.azure.com/";
        private static readonly string _authKey = "OUn2BREa6Iqmi2hgVMQIQiBHh2ANSmVw1ygtySs56tRnuIrMQjHiS5mQw4dBMxlGEYy5tqGZecaqmCQwXqWU7Q==";
        private static readonly string _databaseId = "World";
        private static readonly string _collectionId = "Objects";

        private static readonly string _id = "1c8e4cef-2d2c-47af-9a6b-c760575b55aa";

        private static readonly string userName = "Participant";
        private static readonly string password = "fr5ZS6NT2gQE1VL0hLZmB1X8HhGAW4";
        private static readonly string cloudId = "xmas2019:ZXUtY2VudHJhbC0xLmF3cy5jbG91ZC5lcy5pbyRlZWJjNmYyNzcxM2Q0NTE5OTcwZDc1Yzg2MDUwZTM2MyQyNDFmMzQ3OWNkNzg0ZTUyOTRkODk5OTViMjg0MjAyYg==";

        private static readonly string index = "santa-trackings";

        static async Task Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://vertica-xmas2019.azurewebsites.net");

                HttpResponseMessage apiResponse = await httpClient.PostAsJsonAsync("/api/participate", new { fullName = "Jesper Hossy", emailAddress = "jesh@widex.com", subscribeToNewsletter = false });

                if (!apiResponse.IsSuccessStatusCode)
                    throw new InvalidOperationException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

                var participateResponse = await apiResponse.Content.ReadAsAsync<ParticipateResponse>();

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

                apiResponse = await httpClient.PostAsJsonAsync("/api/santarescue", new { id = participateResponse.Id, position = currentPosition });

                if (!apiResponse.IsSuccessStatusCode)
                    throw new InvalidOperationException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

                Console.WriteLine(await apiResponse.Content.ReadAsStringAsync());

                InputData data = await apiResponse.Content.ReadAsAsync<InputData>();

                //---- låge 3 ----

                List<Zone> normalizedZones = new List<Zone>();

                CosmosConnector cosmosConnector = new CosmosConnector(_endpointUri, _authKey, _databaseId, _collectionId);

                List<ReindeerLocation> positions = new List<ReindeerLocation>();

                foreach (Zone zone in data.Zones)
                {
                    Zone normalized = zone.NormalizeRadius();
                    normalizedZones.Add(normalized);

                    //Console.WriteLine($"Zone - {zone.ToString()}");
                    Console.WriteLine($"Normalized - {normalized.ToString()}");

                    Console.WriteLine("Connecting to CosmosDb...");

                    ReindeerPosition locatedReindeer = await cosmosConnector.GetDocumentByZoneAsync(normalized);
                    positions.Add(new ReindeerLocation() { Name = locatedReindeer.Name, Position = new GeoPoint() { lat = locatedReindeer.Location.Position.Latitude, lon = locatedReindeer.Location.Position.Longitude } });
                    Console.WriteLine("-----------------------------------------------------------");
                }

                try
                {
                    HttpResponseMessage apiReindeerResponse = await httpClient.PostAsJsonAsync("/api/reindeerrescue", new { id = _id, locations = positions });

                    if (!apiResponse.IsSuccessStatusCode)
                        throw new InvalidOperationException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

                    Console.WriteLine("##############################################################################");

                    var result = await apiResponse.Content.ReadAsStringAsync();

                    Console.WriteLine($"Result: {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }

                Console.ReadLine();
            }
        }
    }
}
