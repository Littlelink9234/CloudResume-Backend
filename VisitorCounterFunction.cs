using AzureFuncHtmlResume.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Container = Microsoft.Azure.Cosmos.Container;

namespace AzureFuncHtmlResume
{
    public class VisitorCounterFunction
    {
        private readonly ILogger<VisitorCounterFunction> _logger;

        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("COSMOS_DB_ENDPOINT");
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("COSMOS_DB_KEY");
        private static readonly string DatabaseId = "HtmlResume";
        private static readonly string ContainerId = "VisitorCount";

        private static readonly CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        private static readonly Container container = cosmosClient.GetContainer(DatabaseId, ContainerId);

        public VisitorCounterFunction(ILogger<VisitorCounterFunction> logger)
        {
            _logger = logger;
        }

        [Function("visitor-counter")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var visitorId = "1";
                var response = await container.ReadItemAsync<Item>(visitorId, new PartitionKey(visitorId));
                var visitorData = response.Resource;

                // update the visitor count in cosmos db
                if (visitorData != null)
                {
                    visitorData.VisitorCount++;
                    await container.ReplaceItemAsync(visitorData, visitorId);
                }

                // Return results
                return new OkObjectResult(visitorData.VisitorCount);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in Cosmos DB: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
