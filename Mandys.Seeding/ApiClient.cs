using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Mandys.Application.DTOs;

namespace Mandys.Seeding;

public sealed class ApiException(string message) : Exception(message);

public sealed record PagedResponse<TItem>(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<TItem> Items
);

public sealed class ApiClient : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public ApiClient(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _httpClient = new HttpClient();
    }

    public async Task LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/login?useCookies=false";
        var body = new LoginRequest(email, password);
        using var response = await _httpClient.PostAsJsonAsync(url, body, JsonOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new ApiException($"Login failed ({(int)response.StatusCode}): {text}");
        }

        var auth = await response.Content.ReadFromJsonAsync<TokenAuthResponse>(JsonOptions, cancellationToken)
            ?? throw new ApiException("Login failed: empty response payload.");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}{path}";
        using var response = await _httpClient.PostAsJsonAsync(url, body, JsonOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new ApiException($"POST {path} -> {(int)response.StatusCode}: {text}");
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken)
            ?? throw new ApiException($"POST {path} returned an empty response body.");
    }

    public async Task PostAsync<TRequest>(
        string path,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}{path}";
        using var response = await _httpClient.PostAsJsonAsync(url, body, JsonOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new ApiException($"POST {path} -> {(int)response.StatusCode}: {text}");
        }
    }

    public async Task<IReadOnlyList<TItem>> ListAllAsync<TItem>(
        string path,
        CancellationToken cancellationToken = default)
    {
        var items = new List<TItem>();
        var page = 1;

        while (true)
        {
            var separator = path.Contains('?') ? '&' : '?';
            var requestPath = $"{path}{separator}page={page}&pageSize=100";
            var url = $"{_baseUrl}{requestPath}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var text = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new ApiException($"GET {path} -> {(int)response.StatusCode}: {text}");
            }

            var data = await response.Content.ReadFromJsonAsync<PagedResponse<TItem>>(JsonOptions, cancellationToken)
                ?? throw new ApiException($"GET {path} returned an empty response body.");

            items.AddRange(data.Items);
            if (page >= data.TotalPages)
            {
                return items;
            }

            page++;
        }
    }

    public static void Log(string message)
    {
        var writer = message.StartsWith("ERROR", StringComparison.Ordinal) ? Console.Error : Console.Out;
        writer.WriteLine(message);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
