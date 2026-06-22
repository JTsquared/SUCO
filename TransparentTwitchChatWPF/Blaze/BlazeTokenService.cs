using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TransparentTwitchChatWPF.Blaze;

/// <summary>
/// Acquires and caches Blaze App Access Tokens using the Client Credentials flow.
/// Tokens are cached in a DPAPI-encrypted file and refreshed automatically when expired.
/// </summary>
internal class BlazeTokenService
{
    private const string TokenEndpoint = "https://blaze.stream/bapi/oauth2/token";

    private static readonly string TokenCachePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TransparentTwitchChatWPF",
        "Blaze",
        "token.dat");

    private static readonly HttpClient HttpClient = new();

    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    /// <summary>
    /// Gets a valid App Access Token, fetching a new one if needed.
    /// Returns null if credentials are not configured or the request fails.
    /// </summary>
    public async Task<string?> GetAccessTokenAsync()
    {
        // Return cached token if still valid (with 5-minute buffer)
        if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
            return _cachedToken;

        // Try loading from disk cache
        var cached = LoadCachedToken();
        if (cached != null && DateTime.UtcNow < cached.Value.Expiry.AddMinutes(-5))
        {
            _cachedToken = cached.Value.Token;
            _tokenExpiry = cached.Value.Expiry;
            return _cachedToken;
        }

        // Fetch a fresh token
        return await FetchNewTokenAsync();
    }

    private async Task<string?> FetchNewTokenAsync()
    {
        string? clientSecret = BlazeCredentialManager.LoadSecret();
        if (string.IsNullOrEmpty(clientSecret))
        {
            Debug.WriteLine("Blaze Client Secret not configured.");
            return null;
        }

        try
        {
            var requestBody = new
            {
                clientId = BlazeCredentialManager.ClientId,
                clientSecret = clientSecret,
                grantType = "client_credentials"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await HttpClient.PostAsync(TokenEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Blaze token request failed ({response.StatusCode}): {errorBody}");
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<BlazeTokenResponse>(json);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                Debug.WriteLine("Blaze token response was empty or missing access token.");
                return null;
            }

            _cachedToken = tokenResponse.AccessToken;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

            // Persist to disk
            SaveCachedToken(_cachedToken, _tokenExpiry);

            Debug.WriteLine($"Blaze App Access Token acquired, expires in {tokenResponse.ExpiresIn}s");
            return _cachedToken;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to fetch Blaze token: {ex.Message}");
            return null;
        }
    }

    private void SaveCachedToken(string token, DateTime expiry)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(TokenCachePath)!);

            var cacheData = JsonSerializer.Serialize(new TokenCache
            {
                Token = token,
                ExpiryUtc = expiry.ToString("O")
            });

            byte[] encrypted = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(cacheData),
                null,
                DataProtectionScope.CurrentUser);

            File.WriteAllBytes(TokenCachePath, encrypted);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to cache Blaze token: {ex.Message}");
        }
    }

    private (string Token, DateTime Expiry)? LoadCachedToken()
    {
        if (!File.Exists(TokenCachePath))
            return null;

        try
        {
            byte[] encrypted = File.ReadAllBytes(TokenCachePath);
            byte[] decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            string json = Encoding.UTF8.GetString(decrypted);

            var cache = JsonSerializer.Deserialize<TokenCache>(json);
            if (cache == null || string.IsNullOrEmpty(cache.Token) || string.IsNullOrEmpty(cache.ExpiryUtc))
                return null;

            return (cache.Token, DateTime.Parse(cache.ExpiryUtc).ToUniversalTime());
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load cached Blaze token: {ex.Message}");
            return null;
        }
    }

    private class TokenCache
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = "";

        [JsonPropertyName("expiryUtc")]
        public string ExpiryUtc { get; set; } = "";
    }
}

internal class BlazeTokenResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; } = "";

    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = "";

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }
}
