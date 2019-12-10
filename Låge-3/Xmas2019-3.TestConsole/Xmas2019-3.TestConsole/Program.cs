using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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


        static async Task Main(string[] args)
        {
            string filePath = "Data/vertica-input.json";

            Console.WriteLine("Reading file...");

            string rawInput = _fileReader.Read(filePath);

            Console.WriteLine("File successfully read, parsing file...");
            
            InputData data = JsonConvert.DeserializeObject<InputData>(rawInput);

            List<Zone> normalizedZones = new List<Zone>();

            CosmosConnector cosmosConnector = new CosmosConnector(_endpointUri, _authKey, _databaseId, _collectionId);

            foreach (Zone zone in data.Zones)
            {
                Zone normalized = zone.NormalizeRadius();
                normalizedZones.Add(normalized);

                Console.WriteLine($"Zone - {zone.ToString()}");
                Console.WriteLine($"Normalized - {normalized.ToString()}");

                Console.WriteLine("Connecting to CosmosDb...");
                await cosmosConnector.GetDocumentByZoneAsync(normalized);                
                Console.WriteLine("-----------------------------------------------------------");
            }
            
            Console.ReadLine();
        }
    }
}
