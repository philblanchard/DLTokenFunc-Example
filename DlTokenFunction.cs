using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DirectLineTokenFuncProj;

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