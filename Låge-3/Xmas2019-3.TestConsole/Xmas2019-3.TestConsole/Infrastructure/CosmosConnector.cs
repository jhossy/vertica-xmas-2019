using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Cosmos;
using System.Globalization;

namespace Xmas2019_3.TestConsole.Infrastructure
{
    public class CosmosConnector
    {
        private CosmosClient _client;
        private Container _container;

        public CosmosConnector(string endpointUri, string primaryKey, string databaseId, string container)
        {
            _client = new CosmosClient(endpointUri, primaryKey);
            _container = _client.GetContainer(databaseId, container);
        }

        public async Task GetDocumentByZoneAsync(Zone zone)
        {
            string sqlText = "SELECT o.id " +
                                "FROM Objects o " +
                                "WHERE ST_DISTANCE(o.location, { 'type': 'Point', 'coordinates':[@lat, @lon]}) = @radius " +
                                "AND o.name = @name";

            QueryDefinition query = new QueryDefinition(sqlText)
                .WithParameter("@lat", zone.Center.Lat.ToString(CultureInfo.GetCultureInfo("en-US")))
                .WithParameter("@lon", zone.Center.Lon.ToString(CultureInfo.GetCultureInfo("en-US")))
                .WithParameter("@radius", zone.Radius)
                .WithParameter("@name", zone.Reindeer);

            FeedIterator<ReindeerPosition> queryResultSetIterator = _container.GetItemQueryIterator<ReindeerPosition>(query, null, new QueryRequestOptions() { PartitionKey = new PartitionKey(zone.CountryCode) });

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<ReindeerPosition> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (ReindeerPosition pos in currentResultSet)
                {
                    System.Console.WriteLine(pos.ToString());
                }
                System.Console.WriteLine("Next document...");
            }
        }

        //private readonly DocumentClient _client;
        //private readonly string _databaseId;
        //public CosmosConnector(string endpointUri, string primaryKey, string databaseId)
        //{
        //    _client = new DocumentClient(new System.Uri(endpointUri), primaryKey);
        //    _databaseId = databaseId;
        //}

        //public async Task GetDocumentByZoneAsync(string collection, Zone zone)
        //{
        //    foreach(ReindeerPosition pos in _client.CreateDocumentQuery<ReindeerPosition>(UriFactory.CreateDocumentCollectionUri(_databaseId, collection), new FeedOptions() { PartitionKey = new Microsoft.Azure.Documents.PartitionKey(zone.CountryCode)})
        //        .Where(doc => doc.CountryCode == zone.CountryCode && doc.Location.Distance(new Microsoft.Azure.Documents.poi)
        //    {
        //        System.Console.WriteLine(pos.ToString());
        //    }
        //    //FeedIterator<ReindeerPosition> queryResultSetIterator = _container.GetItemQueryIterator<ReindeerPosition>(query);

        //}
    }
}
