using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TransparentTwitchChatWPF.Blaze;

/// <summary>
/// Manages Blaze API credentials. The Client ID is public and hardcoded.
/// The Client Secret is stored in a DPAPI-encrypted file local to the
/// current Windows user so it never needs to be in source control.
/// </summary>
internal static class BlazeCredentialManager
{
    // Public Client ID — safe to embed (like a Twitch Client ID).
    public const string ClientId = "awNAaTGBxGQhP5YpTtyFj1y63jx80BPK";

    private static readonly string CredentialFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TransparentTwitchChatWPF",
        "Blaze");

    private static readonly string SecretFilePath = Path.Combine(CredentialFolder, "client.dat");

    /// <summary>
    /// Returns true if the Client Secret has been configured on this machine.
    /// </summary>
    public static bool HasSecret => File.Exists(SecretFilePath);

    /// <summary>
    /// Encrypts and stores the Client Secret using DPAPI (current-user scope).
    /// </summary>
    public static void StoreSecret(string clientSecret)
    {
        Directory.CreateDirectory(CredentialFolder);

        byte[] plainBytes = Encoding.UTF8.GetBytes(clientSecret);
        byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(SecretFilePath, encryptedBytes);
    }

    /// <summary>
    /// Decrypts and returns the stored Client Secret.
    /// Returns null if no secret is stored or decryption fails.
    /// </summary>
    public static string? LoadSecret()
    {
        if (!File.Exists(SecretFilePath))
            return null;

        try
        {
            byte[] encryptedBytes = File.ReadAllBytes(SecretFilePath);
            byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (CryptographicException ex)
        {
            Debug.WriteLine($"Failed to decrypt Blaze secret: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Removes the stored secret file.
    /// </summary>
    public static void ClearSecret()
    {
        if (File.Exists(SecretFilePath))
            File.Delete(SecretFilePath);
    }
}
