using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TransparentTwitchChatWPF.Blaze;

/// <summary>
/// Fetches Blaze App Access Tokens from the token proxy server.
/// The proxy holds the Client Secret — desktop clients never see it.
/// Tokens are cached in memory and refreshed automatically when expired.
/// </summary>
internal class BlazeTokenService
{
    private const string TokenProxyUrl = "https://blazegames.store/suco/token";

    private static readonly HttpClient HttpClient = new();

    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    /// <summary>
    /// Gets a valid App Access Token from the proxy, using cache when possible.
    /// Returns null if the proxy is unreachable or returns an error.
    /// </summary>
    public async Task<string?> GetAccessTokenAsync()
    {
        // Return cached token if still valid (with 5-minute buffer)
        if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
            return _cachedToken;

        return await FetchTokenFromProxy();
    }

    /// <summary>
    /// Returns the Client ID (public, safe to embed).
    /// </summary>
    public string GetClientId() => BlazeCredentialManager.ClientId;

    private async Task<string?> FetchTokenFromProxy()
    {
        try
        {
            var response = await HttpClient.GetAsync(TokenProxyUrl);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Blaze token proxy error ({response.StatusCode}): {errorBody}");
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<ProxyTokenResponse>(json);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                Debug.WriteLine("Token proxy returned empty response.");
                return null;
            }

            _cachedToken = tokenResponse.AccessToken;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

            Debug.WriteLine($"Blaze token acquired via proxy, expires in {tokenResponse.ExpiresIn}s");
            return _cachedToken;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to fetch Blaze token from proxy: {ex.Message}");
            return null;
        }
    }
}

internal class ProxyTokenResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = "";

    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = "";

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }
}
