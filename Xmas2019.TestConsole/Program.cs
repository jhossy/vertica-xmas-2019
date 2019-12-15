using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Xmas2019.Library.Infrastructure;
using Xmas2019.Library.Infrastructure.ApiReponse;
using Xmas2019.Library.Infrastructure.Geo;
using Xmas2019.Library.Infrastructure.Movement;
using Xmas2019.Library.Infrastructure.Search;

namespace Xmas2019_3.Library
{
    partial class Program
    {
        private static readonly string _documentdbEndpointUri = "https://xmas2019.documents.azure.com/";
        private static readonly string _authKey = "OUn2BREa6Iqmi2hgVMQIQiBHh2ANSmVw1ygtySs56tRnuIrMQjHiS5mQw4dBMxlGEYy5tqGZecaqmCQwXqWU7Q==";
        private static readonly string _databaseId = "World";
        private static readonly string _collectionId = "Objects";
        
        private static readonly string cloudId = "xmas2019:ZXUtY2VudHJhbC0xLmF3cy5jbG91ZC5lcy5pbyRlZWJjNmYyNzcxM2Q0NTE5OTcwZDc1Yzg2MDUwZTM2MyQyNDFmMzQ3OWNkNzg0ZTUyOTRkODk5OTViMjg0MjAyYg==";

        private static readonly string index = "santa-trackings";

        static async Task Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            using (var httpClient = new HttpClient())
            {
                //låge 1
                httpClient.BaseAddress = new Uri("https://vertica-xmas2019.azurewebsites.net");

                HttpResponseMessage apiResponse = await httpClient.PostAsJsonAsync("/api/participate", new { fullName = "Jesper Hossy", emailAddress = "jesh@widex.com", subscribeToNewsletter = true });

                if (!apiResponse.IsSuccessStatusCode) throw new InvalidOperationException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

                ParticipateResponse participateResponse = await apiResponse.Content.ReadAsAsync<ParticipateResponse>();

                //låge 2

                string documentId = participateResponse.Id.ToString();

                Console.WriteLine("Creating search client...");

                SearchClient client = new SearchClient(participateResponse.Credentials.Username, participateResponse.Credentials.Password, cloudId);

                Console.WriteLine("Fetching document...");

                SantaInformation santaInfo = client.GetDocument(index, documentId);

                Console.WriteLine("Converting to meters...");

                IEnumerable<SantaMovement> alignedSantaMovements = santaInfo.ConvertAllMovementsToMeters();

                Console.WriteLine($"Moving santa...starting from: {santaInfo.CanePosition.ToString()}");

                GeoPoint santaEndlocation = SantaMover.Move(santaInfo.CanePosition, alignedSantaMovements);

                Console.WriteLine($"lat:{santaEndlocation.lat.ToString(CultureInfo.GetCultureInfo("en-US"))}, lon: {santaEndlocation.lon.ToString(CultureInfo.GetCultureInfo("en-US"))}");

                apiResponse = await httpClient.PostAsJsonAsync("/api/santarescue", new { id = participateResponse.Id, position = santaEndlocation });

                if (!apiResponse.IsSuccessStatusCode) throw new InvalidOperationException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

                Console.WriteLine(await apiResponse.Content.ReadAsStringAsync());

                SantaResponse data = await apiResponse.Content.ReadAsAsync<SantaResponse>();

                //---- låge 3 ----
                CosmosConnector cosmosConnector = new CosmosConnector(_documentdbEndpointUri, _authKey, _databaseId, _collectionId);

                List<ReindeerLocation> positions = new List<ReindeerLocation>();                
                foreach (Zone zone in data.Zones)
                {
                    Zone normalized = zone.NormalizeRadius();                
                    Console.WriteLine($"Normalized - {normalized.ToString()}");
                    
                    ReindeerPosition locatedReindeer = await cosmosConnector.GetDocumentByZoneAsync(normalized);
                    positions.Add(new ReindeerLocation() { Name = locatedReindeer.Name, Position = new GeoPoint(locatedReindeer.Location.Position.Latitude, locatedReindeer.Location.Position.Longitude)});
                    Console.WriteLine("-----------------------------------------------------------");
                }

                try
                {
                    HttpResponseMessage apiReindeerResponse = await httpClient.PostAsJsonAsync("/api/reindeerrescue", new { id = participateResponse.Id, locations = positions });

                    if (!apiReindeerResponse.IsSuccessStatusCode) throw new InvalidOperationException($"{apiReindeerResponse.StatusCode}: {(await apiReindeerResponse.Content.ReadAsStringAsync())}");

                    Console.WriteLine("##############################################################################");

                    string result = await apiReindeerResponse.Content.ReadAsStringAsync();

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
