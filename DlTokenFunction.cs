using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DirectLineTokenFuncProj;

public class DlTokenFunction : IDlTokenFunction
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly Uri _remoteServiceBaseUrl = new("https://directline.botframework.com");
    private readonly string _secret = Environment.GetEnvironmentVariable("DL_SECRET");

    public DlTokenFunction(HttpClient httpClient, ILoggerFactory loggerFactory)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secret);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.BaseAddress = _remoteServiceBaseUrl;
        _logger = loggerFactory.CreateLogger<DlTokenFunction>();
    }

    public async Task<DirectLineToken> GetToken()
    {
        _logger.LogInformation("Starting DL Fetch");
        try
        {
            var dlTokenResponse =
                await _httpClient.PostAsJsonAsync<DirectLineRequest>("v3/directline/tokens/generate", null);
            dlTokenResponse.EnsureSuccessStatusCode();
            var response = await dlTokenResponse.Content.ReadFromJsonAsync<DirectLineToken>();
            return response;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode != null)
                _logger.LogError("Call To DirectLine Failed with Status Code: {StatusCode} \r\n Message: {Message}",
                    ex.StatusCode.ToString(), ex.Message);
            var error = new Error(message: ex.Message, statusCode: ex.StatusCode);
            var response = new DirectLineToken(error: error);
            return response;
        }
    }
}