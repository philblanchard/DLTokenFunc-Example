using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DirectLineTokenFuncProj
{
    public class DirectLineTokenFetch
    {
        private readonly ILogger _logger;
        private readonly IDlTokenFunction _client;

        public DirectLineTokenFetch(ILoggerFactory loggerFactory, IDlTokenFunction client)
        {
            _logger = loggerFactory.CreateLogger<DirectLineTokenFetch>();
            _client = client;
        }

        [Function("DirectLineTokenFetch")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Request Processed");

            
            var token = await _client.GetToken();

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(token);
            
            return response;
        }
    }
}
