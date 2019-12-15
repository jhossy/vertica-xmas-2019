using Elasticsearch.Net;
using Nest;
using System;

namespace Xmas2019.Library.Infrastructure.Search
{
    public class SearchClient
    {
        private readonly ElasticClient _client;

        public SearchClient(string userName, string password, string cloudId)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            var credentials = new BasicAuthenticationCredentials(userName, password);
            var pool = new CloudConnectionPool(cloudId, credentials);

            _client = new ElasticClient(new ConnectionSettings(pool));
        }

        public SantaInformation GetDocument(string index, string documentId)
        {
            var request = new GetRequest(index, documentId);

            try
            {
                GetResponse<SantaInformation> result = _client.Get<SantaInformation>(request);
                if (result.IsValid) return result.Source;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return SantaInformation.Empty();
        }
    }
}
