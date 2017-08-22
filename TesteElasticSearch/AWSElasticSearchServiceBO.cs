using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TesteElasticSearch
{
    public class AWSElasticSearchServiceBO
    {
        public string _endpointES = "";
        public string _defaultIndex = "isca-last-position";
        public string _accessKey = "";
        public string _secretKey = "";
        public string _awsRegion = "";

        public void BulkInsertDocuments()
        {
            CreateMapping();
            var client = GetElasticClient();
            var descriptor = new BulkDescriptor();
            foreach (var i in Enumerable.Range(0, 10))
            {
                int clientId = (i % 2 == 0) ? 1 : 2;
                string clientName = clientId == 1 ? "Anderson" : "Fulano 2";
                DateTime dateTime = clientId == 1 ? Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss")) : Convert.ToDateTime(DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd hh:mm:ss"));
                //DateTime dateTime = clientId == 1 ? DateTime.UtcNow : DateTime.UtcNow.AddDays(-1);
                var isca = new Isca { SerialNumber = "0000" + i, LastPosition = dateTime, Client = clientName, ClientId = clientId };
                descriptor.Index<Isca>(op => op.Document(isca).Id(new Id(isca.SerialNumber)));
            }
            var result = client.Bulk(descriptor);
        }

        public void BulkUpdateDocuments()
        {
            var client = GetElasticClient();
            var descriptor = new BulkDescriptor();
            foreach (var i in Enumerable.Range(0, 10))
            {
                var isca = new IscaPartial { SerialNumber = "0000" + i, LastPosition = DateTime.UtcNow };
                descriptor.Update<Isca, IscaPartial>(op => op.Doc(isca).RetriesOnConflict(3).Id(isca.SerialNumber));
            }
            descriptor.Refresh(Refresh.True);
            var result = client.Bulk(descriptor);

            //var client = new ElasticClient(config);
            //var isca = new Isca { NumeroSerie = "00001", Date = DateTime.UtcNow.ToString() };
            //var response = client.Update<Isca, Isca>(new DocumentPath<Isca>("00001"), u => u.Doc(isca));
        }

        public void DeleteIndexDefault()
        {
            var client = GetElasticClient();
            client.DeleteIndex(_defaultIndex);
        }

        public void CreateMapping()
        {
            var client = GetElasticClient();
            var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };
            var indexConfig = new IndexState
            {
                Settings = settings
            };

            client.CreateIndex(_defaultIndex, c => c
                    .InitializeUsing(indexConfig)
                    .Mappings(m => m.Map<Isca>(mp => mp.AutoMap())));
        }

        public ISearchResponse<Isca> GetFullTextSearch(string search, string clientId)
        {
            var client = GetElasticClient();
            var searchRequest = new SearchRequest
            {
                From = 0,
                Size = 10,
            };

            if (!string.IsNullOrEmpty(clientId))
            {
                var searchResults = client.Search<Isca>(l =>
                    l.Query(q =>
                        q.Term(p => p.ClientId, clientId)
                        && q.QueryString(qs => qs.Query("*" + search + "*"))
                    ).Sort(s=> s.Descending(f=> f.LastPosition))
                );
                return searchResults;
            }
            else
            {
                var searchResults = client.Search<Isca>(l => 
                    l.Query(q => 
                        q.QueryString(qs => qs.Query("*" + search+ "*")))
                        .Sort(s => s.Descending(f => f.LastPosition))
                );
                return searchResults;
            }
        }

        public ElasticClient GetElasticClient()
        {
            var httpConnection = new AwsHttpConnection(_awsRegion, new StaticCredentialsProvider(new AwsCredentials()
            {
                AccessKey = _accessKey,
                SecretKey = _secretKey,
            }));

            var pool = new SingleNodeConnectionPool(new Uri(_endpointES));
            var config = new ConnectionSettings(pool, httpConnection);
            config.DefaultIndex(_defaultIndex);
            return new ElasticClient(config);
        }
    }

    [ElasticsearchType(Name = "isca")]
    public class Isca
    {
        public string SerialNumber { get; set; }

        public DateTime LastPosition { get; set; }
        public string Client { get; set; }
        public int ClientId { get; set; }
    }

    public class IscaPartial
    {
        public string SerialNumber { get; set; }
        public DateTime LastPosition { get; set; }
    }
}
