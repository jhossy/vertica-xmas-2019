using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xmas2019_3.TestConsole.Infrastructure;

namespace Xmas2019_3.TestConsole
{
    class Program
    {
        private static FileReader _fileReader = new FileReader();

        private static readonly string _endpointUri = "https://xmas2019.documents.azure.com/";
        private static readonly string _authKey = "OUn2BREa6Iqmi2hgVMQIQiBHh2ANSmVw1ygtySs56tRnuIrMQjHiS5mQw4dBMxlGEYy5tqGZecaqmCQwXqWU7Q==";
        private static readonly string _databaseId = "World";
        private static readonly string _collectionId = "Objects";

        private static readonly string _id = "1c8e4cef-2d2c-47af-9a6b-c760575b55aa";


        static async Task Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            string filePath = "Data/vertica-input.json";

            Console.WriteLine("Reading file...");

            string rawInput = _fileReader.Read(filePath);

            Console.WriteLine("File successfully read, parsing file...");
            
            InputData data = JsonConvert.DeserializeObject<InputData>(rawInput);

            List<Zone> normalizedZones = new List<Zone>();

            CosmosConnector cosmosConnector = new CosmosConnector(_endpointUri, _authKey, _databaseId, _collectionId);

            List<ReindeerPosition> positions = new List<ReindeerPosition>();

            foreach (Zone zone in data.Zones)
            {
                Zone normalized = zone.NormalizeRadius();
                normalizedZones.Add(normalized);

                //Console.WriteLine($"Zone - {zone.ToString()}");
                Console.WriteLine($"Normalized - {normalized.ToString()}");

                Console.WriteLine("Connecting to CosmosDb...");
                positions.Add(await cosmosConnector.GetDocumentByZoneAsync(normalized));                
                Console.WriteLine("-----------------------------------------------------------");
            }

            try
            {
                await SendResult(_id, positions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.ReadLine();
        }

        private static async Task SendResult(string id, List<ReindeerPosition> positions)
        {
            Console.WriteLine();

            string tmp = JsonConvert.SerializeObject(new ReindeerAnswer(id, positions));

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://vertica-xmas2019.azurewebsites.net");
                
                //HttpResponseMessage apiResponse = await httpClient.PostAsJsonAsync("/api/reindeerrescue", JsonConvert.SerializeObject(new ReindeerAnswer(id, positions)));

                //if (!apiResponse.IsSuccessStatusCode)
                //    throw new InvalidOperationException($"{apiResponse.StatusCode}: {(await apiResponse.Content.ReadAsStringAsync())}");

                //Console.WriteLine($"Result: {await apiResponse.Content.ReadAsStringAsync()}");
            }
            
        }
    }
}
