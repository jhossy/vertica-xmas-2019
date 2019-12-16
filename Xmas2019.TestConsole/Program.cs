using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xmas2019.Library.Infrastructure;
using Xmas2019.Library.Infrastructure.ApiReponse;
using Xmas2019.Library.Infrastructure.Geo;
using Xmas2019.Library.Infrastructure.Movement;
using Xmas2019.Library.Infrastructure.Search;
using Xmas2019.Library.Infrastructure.Toys;

namespace Xmas2019.TestConsole
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
                httpClient.BaseAddress = new Uri("https://vertica-xmas2019.azurewebsites.net");

                //låge 1
                ParticipateResponse participateResponse = await Låge1(httpClient);

                //låge 2
                SantaResponse data = await Låge2(participateResponse, httpClient);

                //låge 3
                ReindeerResponse result = await Låge3(participateResponse, data, httpClient);

                //låge 4
                await Låge4(participateResponse, result.ToyDistributionXmlUrl, httpClient);

                Console.ReadLine();
            }
        }

        private static async Task<ParticipateResponse> Låge1(HttpClient httpClient)
        {
            HttpResponseMessage apiResponse = await httpClient.PostAsJsonAsync("/api/participate", new { fullName = "Jesper Hossy", emailAddress = "jesh@widex.com", subscribeToNewsletter = true });

            if (!apiResponse.IsSuccessStatusCode) throw new ChristmasException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

            return await apiResponse.Content.ReadAsAsync<ParticipateResponse>();
        }

        private static async Task<SantaResponse> Låge2(ParticipateResponse participateResponse, HttpClient httpClient)
        {
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

            HttpResponseMessage apiResponse = await httpClient.PostAsJsonAsync("/api/santarescue", new { id = participateResponse.Id, position = santaEndlocation });

            if (!apiResponse.IsSuccessStatusCode) throw new ChristmasException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

            Console.WriteLine(await apiResponse.Content.ReadAsStringAsync());

            return await apiResponse.Content.ReadAsAsync<SantaResponse>();
        }

        private static async Task<ReindeerResponse> Låge3(ParticipateResponse participateResponse, SantaResponse data, HttpClient httpClient)
        {
            CosmosConnector cosmosConnector = new CosmosConnector(_documentdbEndpointUri, _authKey, _databaseId, _collectionId);

            List<ReindeerLocation> positions = new List<ReindeerLocation>();
            foreach (Zone zone in data.Zones)
            {
                Zone normalized = zone.NormalizeRadius();
                Console.WriteLine($"Normalized - {normalized.ToString()}");

                ReindeerPosition locatedReindeer = await cosmosConnector.GetDocumentByZoneAsync(normalized);
                positions.Add(new ReindeerLocation() { Name = locatedReindeer.Name, Position = new GeoPoint(locatedReindeer.Location.Position.Latitude, locatedReindeer.Location.Position.Longitude) });
                Console.WriteLine("-----------------------------------------------------------");
            }

            HttpResponseMessage apiReindeerResponse = await httpClient.PostAsJsonAsync("/api/reindeerrescue", new { id = participateResponse.Id, locations = positions });

            if (!apiReindeerResponse.IsSuccessStatusCode) throw new ChristmasException($"{apiReindeerResponse.StatusCode}: {(await apiReindeerResponse.Content.ReadAsStringAsync())}");

            Console.WriteLine("##############################################################################");

            ReindeerResponse result = await apiReindeerResponse.Content.ReadAsAsync<ReindeerResponse>();

            Console.WriteLine($"Result: {result}");

            return result;
        }

        private static async Task Låge4(ParticipateResponse participateResponse, string xmlUrl, HttpClient httpClient)
        {
            Console.WriteLine("Parsing XML...");

            XDocument xmlDocument = XDocument.Load(xmlUrl);

            ToyDistributionProblem parsedXml = XmlSerializerUtil.Deserialize<ToyDistributionProblem>(xmlDocument);

            Console.WriteLine($"Xml parsed to object...{parsedXml.Children.Count} children and {parsedXml.Toys.Count} toys...");

            Console.WriteLine("Starting to distribute toys to children...");

            Queue<Child> remainingChilren = new Queue<Child>(parsedXml.Children);

            int counter = 0;
            ToyDistributorHelper toyDistributor = new ToyDistributorHelper(parsedXml.Toys);
            //ToyDistributionResult result = new ToyDistributionResult();
            List<ToyDistribution> distributionResult = new List<ToyDistribution>();

            try
            {
                while (remainingChilren.Count > 0)
                {
                    Console.WriteLine($"Iteration: {counter}");

                    Child currentChild = remainingChilren.Dequeue();

                    Toy foundToy;
                    bool resolved = toyDistributor.TryResolve(currentChild, out foundToy);

                    if (resolved)
                    {
                        distributionResult.Add(new ToyDistribution() { ChildName = currentChild.Name, ToyName = foundToy.Name });
                        toyDistributor.RemoveToy(foundToy);
                    }
                    else
                    {
                        remainingChilren.Enqueue(currentChild);
                    }

                    counter++;

                    if (counter > 50) break;
                }

                //Console.WriteLine($"Toy distribution result: {result.ToString()}");

                //List<ToyDistribution> distributionResult = new List<ToyDistribution>();
                //foreach (var res in result.ToyDistribution)
                //{
                //    distributionResult.Add(new ToyDistribution() { ChildName = res.Key, ToyName = res.Value });
                //}

                HttpResponseMessage toyDistributionResponse = await httpClient.PostAsJsonAsync("/api/toydistribution", new { id = participateResponse.Id, toyDistribution = distributionResult });

                if (!toyDistributionResponse.IsSuccessStatusCode)
                {
                    string reason = await toyDistributionResponse.Content.ReadAsStringAsync();
                    throw new ChristmasException($"{toyDistributionResponse.StatusCode}: {(await toyDistributionResponse.Content.ReadAsStringAsync())}");
                }

                Console.WriteLine("##############################################################################");

                string apiResult = await toyDistributionResponse.Content.ReadAsStringAsync();
                Console.WriteLine(apiResult);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }    
        }
    }

    public class ToyDistribution
    {
        public string ChildName { get; set; }

        public string ToyName { get; set; }
    }
}
