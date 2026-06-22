namespace TransparentTwitchChatWPF.Blaze;

/// <summary>
/// Holds the public Blaze Client ID. The Client Secret is managed
/// server-side by the token proxy and never touches the desktop app.
/// </summary>
internal static class BlazeCredentialManager
{
    /// <summary>
    /// Public Client ID — safe to embed (like a Twitch Client ID).
    /// </summary>
    public const string ClientId = "awNAaTGBxGQhP5YpTtyFj1y63jx80BPK";
}
