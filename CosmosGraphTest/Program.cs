using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosGraphTest
{
    class Program
    {
        private static DocumentClient _client;
        private static ResourceResponse<DocumentCollection> _graph;

        static Program()
        {
            var endpoint = "https://pandamonium.documents.azure.com:443/";
            var authKey = "2PHv7s0ZAy01Hr8sTGmJA7gCAapFeexKgOViYHwDrgvQs0QLXl6qXiCrjFBmJUgHQ1IjidN8HWVRavJ18i7SIw==";

            _client = new DocumentClient(
                new Uri(endpoint),
                authKey,
                new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp });


            _graph = _client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri("test"),
                new DocumentCollection { Id = "test" },
                new RequestOptions { OfferThroughput = 400 }).Result;
            
        }


        static async Task Run()
        {
            string queryString = "";
            while (queryString != "q")
            {
                Console.WriteLine("Submit gremlin:");
                try
                {
                    queryString = Console.ReadLine();
                    if (queryString == "q") break;

                    var query = _client.CreateGremlinQuery<dynamic>(_graph, queryString);

                    while (query.HasMoreResults)
                    {
                        foreach (dynamic result in await query.ExecuteNextAsync())
                        {
                            Console.WriteLine($"\t {JsonConvert.SerializeObject(result)}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\tException executing gremlin: {e}");
                }
            }

            //var query = client.CreateGremlinQuery<dynamic>(graph, "g.addV('Person', 'Jesse Carter').property('Age', 27).property('partitionKey', 'Person')");
            //IDocumentQuery<DocumentResponse<dynamic>> query = client.CreateGremlinQuery<DocumentResponse<dynamic>>(graph, "g.addV('label', 'Person', 'id', 'Alex Drenea', 'Age', 31, 'partitionKey', 'Person')");

            //var query = client.CreateGremlinQuery<dynamic>(graph, "g.V().valueMap()");

            //var query = client.CreateGremlinQuery<dynamic>(graph, @"g.V().and(has('partitionKey', eq('Person')), has('id', eq('Jesse Carter'))).as('Jesse').addE('knows', g.V().has('id', 'Alex Drenea'))");

            //var query = client.CreateGremlinQuery<dynamic>(graph, "g.V().has('Id', eq('Jesse Carter')))");

            //var query = client.CreateGremlinQuery<dynamic>(graph,
            //    @"g.V().match(
            //        __.as('jesse').and(has('partitionKey', eq('Person')), has('id', eq('Jesse Carter'))),
            //        __.as('alex').and(has('partitionKey', eq('Person')), has('id', eq('Alex Drenea'))))
            //        .addE('knows').from('jesse').to('alex')");

            //var query = client.CreateGremlinQuery<dynamic>(graph, "g.V().and(has('partitionKey', eq('Person')), has('id', eq('Jesse Carter'))).addE('knows').to(g.V().and(has('partitionKey', eq('Person')), has('id', eq('Alex Drenea'))))");

            //var query = client.CreateGremlinQuery<dynamic>(graph, "g.V().hasLabel('Person').has('id', 'Jesse Carter').outE()");

            //var 



            //"".ToString();
            //}
        }

        static void Main(string[] args)
        {
            Run().Wait();
            Console.WriteLine("Done");
        }
    }
}
