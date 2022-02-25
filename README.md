#.Net Azure Function - DL Token Fetching

This guide is a higher-level walkthrough of the steps to create an Azure Function with .Net6. The objective of this function will be to exchange a DirectLine secret for a token to be used when with the BotFramework Web Chat.

From a high-level, the steps are as follows:
1. Ensure required tools are installed
    1. Azure CLI
    2. .Net 6
    3. Azure Core Tools v4
    4. An IDE
2. Create Function project with Core Tools
3. Create Function Trigger (Http in this case)
4. Create Typed HttpClient for Dependency Injection
5. Create Typed client interface
6. Register services within DI Container
7. Create constructor using Typed Http Client
8. Create Types
9. Set up local settings
10. Test
11. Publish


## Walkthrough
### Tool Installation
The tool installation process should be relatively easy to follow and is documented on MS’s site for the various tools.

### Create Function Project
We will create an isolated process runtime for this function. The following command accomplishes that:
func init DirectLineTokenFuncProj --worker-runtime dotnet-isolated

cd DirectLineTokenFuncProj

Open the directory in your IDE of choice and examine the project structure & files.

###Create Function Trigger
The following command, from within the project directory, will create the actual Class that registers a method as a function.

func new --name DirectLikeTokenFetch --template "HTTP trigger" --authlevel "anonymous"

This should create a new `.cs` file with the same name as supplied to the `--name` parameter.

###Create Typed HttpClient
Azure Functions support Dependency Injection and so we should follow this proper pattern for using HttpClients. This will help avoid socket exhaustion.

####Step 1: Config/Register your service
In `Program.cs` edit your `HostBuilder()` to match the following pattern:

public static void Main()
{
var host = new HostBuilder()
.ConfigureFunctionsWorkerDefaults()
.ConfigureServices(s =>
{
s.AddHttpClient<IDlTokenFunction, DlTokenFunction>();
})
.Build();

            host.Run();
        }
####Step 2: Create Client
In the above step we identified an `Interface` and a class that implements this `Interface` .

First, create the `Interface`. To start, we will keep this quite simple:
public interface IDlTokenFunction
{
Task<DirectLineToken> GetToken();
}

Next, we will create the actual Class that implements this - shown below:
public class DlTokenFunction : IDlTokenFunction
{
private readonly HttpClient _httpClient;
private readonly Uri _remoteServiceBaseUrl = new Uri("https://directline.botframework.com");
private readonly string _secret = System.Environment.GetEnvironmentVariable("DL_SECRET");

    public DlTokenFunction(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secret);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.BaseAddress = _remoteServiceBaseUrl;
    }

    public async Task<DirectLineToken> GetToken()
    {
        var dlTokenResponse = await _httpClient.PostAsJsonAsync<DirectLineRequest>("v3/directline/tokens/generate", null);
        dlTokenResponse.EnsureSuccessStatusCode();
        var response = await dlTokenResponse.Content.ReadFromJsonAsync<DirectLineToken>();
        return response;
    }
}

In the above snippet, the `SECRET` for this call is stored as `DL_SECRET` in the `local.settings.json` file.

### Configure your `Function` to use this Typed Client
In the `Function` you registered before, `DirectLineTokenFetch`, edit your class to look as below:

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DirectLineTokenFuncProj
{
public class DirectLikeTokenFetch
{
private readonly ILogger _logger;
private readonly IDlTokenFunction _client;

        public DirectLikeTokenFetch(ILoggerFactory loggerFactory, IDlTokenFunction client)
        {
            _logger = loggerFactory.CreateLogger<DirectLikeTokenFetch>();
            _client = client;
        }

        [Function("DirectLikeTokenFetch")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var token = await _client.GetToken();

            var response = req.CreateResponse(HttpStatusCode.OK);
           // response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            //response.WriteString("Welcome to Azure Functions!");

            await response.WriteAsJsonAsync(token);
            
            return response;
        }
    }
}


###Create your Types
The above samples use some types for marshalling data as json. They are below:

public class User
{
public string Id { get; set; }
public string Name { get; set; }
}

public class DirectLineRequest
{
public User User { get; set; }
public List<string> TrustedOrigins { get; set; }
}

public class DirectLineToken
{
public string ConversationId { get; set; }
public string Token { get; set; }
public int ExpiresIn { get; set; }

}

###Test your function
func start
This will build and run your function locally. Navigate to the localhost address to see if everything is working