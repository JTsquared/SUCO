using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using TransparentTwitchChatWPF.ChatProviders;
using TransparentTwitchChatWPF.Twitch;
using TwitchLib.Api.Helix.Models.Chat.ChatSettings;

namespace TransparentTwitchChatWPF.Utils;

public class WebViewConfigurator
{
    private readonly ILogger<WebViewConfigurator> _logger;
    private readonly TwitchService _twitchService;

    public WebViewConfigurator(ILogger<WebViewConfigurator> logger, TwitchService twitchService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _twitchService = twitchService ?? throw new ArgumentNullException(nameof(twitchService));
    }

    public async Task ConfigureAsync(CoreWebView2 coreWebView2)
    {
        var chatType = (ChatTypes)App.Settings.GeneralSettings.ChatType;

        IChatProvider chatProvider;
        try
        {
            // Get the correct chat provider using the Factory
            chatProvider = ChatProviderFactory.Create(chatType);
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex, "The selected chat type is not currently supported");

            MessageBox.Show(
                "The selected chat type is not currently supported. Please choose a different one in the settings.",
                "Unsupported Chat Type",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            // default to a known-good provider or simply return.
            chatProvider = null; // or new DefaultChatProvider();
        }

        if (chatProvider != null)
        {
            await chatProvider.ConfigureAsync(coreWebView2);

            // Inject Scripts and Styles
            await ExecuteScriptAsync(coreWebView2, chatProvider.GetJavascriptToExecute());
            await InjectCssAsync(coreWebView2, chatProvider.GetCssToInject());

            //chatProvider.PushNewMessage(chatType.ToString() + " Loaded.");

            // Initialize other services
            if (App.Settings.GeneralSettings.RedemptionsEnabled)
            {
                _ = _twitchService.InitializeAsync();
            }
        }
    }

    // --- Helper methods --------------------------
    private async Task InjectCssAsync(CoreWebView2 coreWebView2, string css)
    {
        if (string.IsNullOrEmpty(css)) return;
        string cssJson = JsonSerializer.Serialize(css);
        string script = $$"""
        (() => {
            const css = {{cssJson}};
            const styleId = 'ttco-injected-css';

            function ensureStyle() {
                const head = document.head || document.documentElement;
                if (!head) return;

                let style = document.getElementById(styleId);
                if (!style) {
                    style = document.createElement('style');
                    style.id = styleId;
                    style.type = 'text/css';
                }

                if (style.textContent !== css) {
                    style.textContent = css;
                }

                if (style.parentNode !== head || head.lastElementChild !== style) {
                    head.appendChild(style);
                }
            }

            ensureStyle();

            if (window.__ttcoCssObserver) {
                window.__ttcoCssObserver.disconnect();
            }

            if (document.head) {
                window.__ttcoCssObserver = new MutationObserver(ensureStyle);
                window.__ttcoCssObserver.observe(document.head, { childList: true });
            }
        })();
        """;
        await coreWebView2.ExecuteScriptAsync(script);
    }

    private async Task ExecuteScriptAsync(CoreWebView2 coreWebView2, string script)
    {
        if (string.IsNullOrEmpty(script)) return;
        await coreWebView2.ExecuteScriptAsync(script);
    }

    /*private string InsertCustomCSS2(string CSS)
    {
        string uriEncodedCSS = Uri.EscapeDataString(CSS);
        string script = "const ttcCSS = document.createElement('style');";
        script += "ttcCSS.innerHTML = decodeURIComponent(\"" + uriEncodedCSS + "\");";
        script += "document.querySelector('head').appendChild(ttcCSS);";
        return script;
    }

    private string InsertCustomJS2(string js)
    {
        string uriEncodedJS = Uri.EscapeDataString(js);
        string script = "const ttcJS = document.createElement('script');";
        script += "ttcJS.innerHTML = decodeURIComponent(\"" + uriEncodedJS + "\");";
        script += "document.querySelector('head').appendChild(ttcJS);";
        return script;
    }*/
}
