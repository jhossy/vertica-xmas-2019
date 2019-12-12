using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Cosmos;
using System.Globalization;
using System.Collections.Generic;
using Xmas2019_3.Library.Infrastructure.Geo;
using Xmas2019_3.Library.Infrastructure;

namespace Xmas2019_3.Library.Infrastructure.Search
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

        public async Task<ReindeerPosition> GetDocumentByZoneAsync(Zone zone)
        {
            string sqlText = "SELECT o.id, o.countryCode, o.name, o.location " +
                                "FROM Objects o " +
                                "WHERE ST_DISTANCE(o.location, { 'type': 'Point', 'coordinates':[@lon, @lat]}) < @radius " +
                                "AND o.name = @name";

            QueryDefinition query = new QueryDefinition(sqlText)
                .WithParameter("@lon", zone.Center.lon)
                .WithParameter("@lat", zone.Center.lat)
                .WithParameter("@radius", zone.Radius.Value)
                .WithParameter("@name", zone.Reindeer);

            FeedIterator<ReindeerPosition> queryResultSetIterator = _container.GetItemQueryIterator<ReindeerPosition>(query, null, new QueryRequestOptions() { PartitionKey = new PartitionKey(zone.CountryCode) });

            List<ReindeerPosition> result = new List<ReindeerPosition>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<ReindeerPosition> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (ReindeerPosition pos in currentResultSet)
                {
                    result.Add(pos);
                    System.Console.WriteLine("Found:" + pos.ToString());
                }
                System.Console.WriteLine("Next document...");
            }

            if (result.Count != 1) throw new System.Exception("Too many reindeer positions found");


            return result.First();
        }
    }
}
