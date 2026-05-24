using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransparentTwitchChatWPF;

public class WindowSettings
{
    public int ChatType { get; set; }
    public string Title { get; set; }
    public string Username { get; set; }
    public string URL { get; set; }
    public bool ChatFade { get; set; }
    public string FadeTime { get; set; }
    public bool ShowBotActivity { get; set; }
    public string ChatNotificationSound { get; set; }
    public int Theme { get; set; }
    public string CustomCSS { get; set; }
    public string TwitchPopoutCSS { get; set; }
    public bool AutoHideBorders { get; set; }
    public bool EnableTrayIcon { get; set; }
    public bool ConfirmClose { get; set; }
    public bool HideTaskbarIcon { get; set; }
    public bool AllowInteraction { get; set; }
    public bool RedemptionsEnabled { get; set; }
    public string ChannelID { get; set; }
    public bool BetterTtv { get; set; }
    public bool FrankerFaceZ { get; set; }
    public string jChatURL { get; set; }
}

public static class CustomCSS_Defaults
{
    public static string TwitchPopoutChat = @":root {
  --overlay-bg: rgba(0,0,0,0);
  --text-white: rgba(255,255,255,0.88);
  --text-muted: rgba(255,255,255,0.62);
  --text-shadow:
    -1px -1px 0 rgba(0,0,0,0.85),
     1px -1px 0 rgba(0,0,0,0.85),
    -1px  1px 0 rgba(0,0,0,0.85),
     1px  1px 0 rgba(0,0,0,0.85);
}

/* Transparent Twitch shell/backgrounds */
body,
.tw-flex,
.tw-root,
.tw-root--theme-dark,
.stream-chat,
.chat-room,
.chat-list,
.scrollable-area {
  background: var(--overlay-bg) !important;
  background-color: var(--overlay-bg) !important;
}

.scrollable-area {
  color: white !important;
}

/* Hide header / extra UI */
.chat-room__notifcations,
.tw-z-default,
.marquee-animation,
.stream-chat .stream-chat-header {
  display: none !important;
}

.stream-chat .stream-chat-header {
  background: var(--overlay-bg) !important;
  color: white !important;
}

#chat-room-header-label {
  color: #cacaca !important;
}

/* Readable chat message text */
.chat-line__message,
.chat-line__message *,
.chat-author__display-name,
.chat-author__display-name *,
.chat-line__username,
.chat-line__username *,
.text-fragment,
.mention-fragment,
[data-a-target=""chat-message-text""],
[data-a-target=""chat-message-text""] *,
[data-a-target=""chat-line-message""],
[data-a-target=""chat-line-message""] * {
  text-shadow: var(--text-shadow) !important;
}

/* Timestamps */
.chat-line__timestamp {
  color: var(--text-muted) !important;
  text-shadow: var(--text-shadow) !important;
}

/* Twitch system/status/notice/redeem text */
.chat-line__status,
.chat-line__status *,
.user-notice-line,
.user-notice-line *,
[class*=""user-notice-line""],
[class*=""user-notice-line""] *,
[class*=""UserNotice""],
[class*=""UserNotice""] *,
[class*=""reward""],
[class*=""reward""] *,
[class*=""Reward""],
[class*=""Reward""] *,
[class*=""redemption""],
[class*=""redemption""] *,
[class*=""Redemption""],
[class*=""Redemption""] *,
[class*=""redeem""],
[class*=""redeem""] *,
[class*=""Redeem""],
[class*=""Redeem""] *,
[data-a-target=""chat-welcome-message""],
[data-a-target=""chat-welcome-message""] *,
[data-a-target*=""user-notice-line""],
[data-a-target*=""user-notice-line""] *,
[data-a-target*=""channel-points""],
[data-a-target*=""channel-points""] *,
[data-a-target*=""reward""],
[data-a-target*=""reward""] *,
[data-a-target*=""redemption""],
[data-a-target*=""redemption""] *,
[data-test-selector=""user-notice-line""],
[data-test-selector=""user-notice-line""] *,
[data-test-selector*=""user-notice-line""],
[data-test-selector*=""user-notice-line""] *,
.channel-points-reward-line__icon,
.channel-points-reward-line__icon *,
div:has(> [data-test-selector=""user-notice-line""]) {
  color: var(--text-white) !important;
  text-shadow: var(--text-shadow) !important;
}

/* Channel point / redeem icon */
[data-test-selector=""user-notice-line""] svg,
.channel-points-reward-line__icon svg {
  fill: var(--text-white) !important;
}

/* Keep emotes/badges/images clean */
.chat-badge,
.chat-badge *,
img,
svg {
  text-shadow: none !important;
}

/* Chat input parent area */
.chat-input-tray__open,
.chat-input-container__open,
.chat-input__textarea,
.chat-input__textarea > div,
.chat-input__textarea .font-scale--default,
.chat-input__textarea .font-scale--bigger,
.chat-input__textarea [class*=""font-scale--""],
[class*=""font-scale--""]:has([data-a-target=""chat-input""]),
.chat-input__textarea:has([data-a-target=""chat-input""]) {
  background: transparent !important;
  background-color: transparent !important;
  color: white !important;
  box-shadow: none !important;
}

/* Bordered chat input box */
.chat-wysiwyg-input-box,
.chat-wysiwyg-input-box--allow-border-style,
.chat-wysiwyg-input-box--allow-focus-style,
.chat-wysiwyg-input-box:has([data-a-target=""chat-input""]) {
  background: rgba(0,0,0,0.18) !important;
  background-color: rgba(0,0,0,0.18) !important;
  border: 1px solid rgba(255,255,255,0.35) !important;
  border-radius: 8px !important;
  box-shadow: 0 0 0 1px rgba(0,0,0,0.35) !important;
  color: white !important;
}

.chat-wysiwyg-input-box:focus-within {
  border-color: rgba(145,70,255,0.85) !important;
  box-shadow: 0 0 0 1px rgba(145,70,255,0.55) !important;
}

/* Actual editable chat textbox */
[data-a-target=""chat-input""],
[data-test-selector=""chat-input""],
.chat-wysiwyg-input__editor {
  background: transparent !important;
  background-color: transparent !important;
  color: white !important;
  caret-color: white !important;
  outline: none !important;
}

[data-a-target=""chat-input""] *,
[data-test-selector=""chat-input""] *,
.chat-wysiwyg-input__editor * {
  color: white !important;
  text-shadow: none !important;
}

/* Placeholder text */
.chat-wysiwyg-input__placeholder {
  background: transparent !important;
  background-color: transparent !important;
  color: #a9a9a9 !important;
  text-shadow: none !important;
}

/* Highlights / announcements */
.community-highlight {
  background-color: rgba(0,0,0,0.75) !important;
}

.announcement-line {
  background-color: rgba(0,0,0,0.2) !important;
}

/* Hide only the rotating leaderboard banner area */
div:has(> div > div[aria-label=""Expand Top Gifters Leaderboard""]),
div:has(> div > div[aria-label=""Expand Top Clips Leaderboard""]),
div:has(> div > div[aria-label=""Expand Top Cheerers Leaderboard""]) {
  display: none !important;
}

/* Some icon-row background cleanup */
div:has(> div.tw-svg + div) {
  background-color: transparent !important;
}

/* Light theme base fixes */
.tw-root--theme-light {
  background: var(--overlay-bg) !important;
  background-color: var(--overlay-bg) !important;
  color: white !important;
}

.tw-root--theme-light svg {
  fill: white !important;
}

.tw-root--theme-light input,
.tw-root--theme-light textarea {
  color: white !important;
}

.tw-root--theme-light input::placeholder,
.tw-root--theme-light textarea::placeholder {
  color: #cacaca !important;
}

.tw-root--theme-light [class*=""tw-border-""] {
  border-color: rgba(255,255,255,0.25) !important;
}

/* Do not outline Twitch menus/settings/popups */
[role=""dialog""],
[role=""dialog""] *,
[role=""menu""],
[role=""menu""] *,
[role=""listbox""],
[role=""listbox""] *,
.tw-balloon,
.tw-balloon *,
[data-a-target*=""settings""],
[data-a-target*=""settings""] * {
  text-shadow: none !important;
}

/* Keep light-mode menus/settings readable */
.tw-root--theme-light [role=""dialog""],
.tw-root--theme-light [role=""dialog""] *,
.tw-root--theme-light [role=""menu""],
.tw-root--theme-light [role=""menu""] *,
.tw-root--theme-light [role=""listbox""],
.tw-root--theme-light [role=""listbox""] *,
.tw-root--theme-light .tw-balloon,
.tw-root--theme-light .tw-balloon * {
  color: #111 !important;
  text-shadow: none !important;
}

/* Don't force SVG icons inside dialogs to black/white weirdly */
.tw-root--theme-light [role=""dialog""] svg,
.tw-root--theme-light [role=""menu""] svg,
.tw-root--theme-light [role=""listbox""] svg,
.tw-root--theme-light .tw-balloon svg {
  fill: currentColor !important;
}
";

    public static string WebCaptioner = @"body { background-color: rgba(0,0,0,0.1) !important; }
.transcript { background-color: rgba(0,0,0,0) !important; margin-bottom: -1em !important; }
.bg-dark { background-color: rgba(0,0,0,0) !important; }";

    public static string NoneTheme_CustomCSS = @"#chat_box {
 text-shadow: 2px 2px 0 #000, 2px 2px 4px #000;
 letter-spacing: 1px;
}

.chat_line {
 color: #fff;
 font-size: 16px!important;
 font-weight: bold;
}

.chat_line .nick {

}

.message { display: inline !important; }
.highlight { background-color: rgba(255,255,0,0.5) !important; }";
}

public static class CustomJS_Defaults
{
    public static string VIP_Check = @"
var vip = false;
if (tags2.badges)
{
    tags2.badges.forEach(function(badge2) {
        if (badge2.type.toLowerCase() == 'vip')
        {
            vip = true;
            return;
        }
    });
}
allowOther = vip;";

    public static string Mod_Check = @"
var mod = false;

if (tags2.badges)
{
    tags2.badges.forEach(function(badge2) {
        if (badge2.type.toLowerCase() == 'moderator')
        {
            mod = true;
            return;
        }
    });
}
if (mod) { allowOther = true; }";

    public static string Callback_PlaySound = @"
            (async function() {
                await CefSharp.BindObjectAsync('jsCallback');
                jsCallback.playSound();
            })();";

    public static string jChat_VIP_Check = @"
var vip = false;
if (typeof(info.badges) === 'string')
{
    info.badges.split(',').forEach(badge => {
        badge = badge.split('/');
        if (badge[0].toLowerCase() == 'vip')
        {
            highlightSuffix = 'VIP';
            vip = true;
        }
    });
}
allowOther = vip;";

    public static string jChat_Mod_Check = @"
var mod = false;

if (typeof(info.badges) === 'string')
{
    info.badges.split(',').forEach(badge => {
        badge = badge.split('/');
        if (badge[0].toLowerCase() == 'moderator')
        {
            highlightSuffix = 'Mod';
            mod = true;
        }
    });
}
if (mod) { allowOther = true; }";
    
    public static string jCyan_VIP_Check = @"
var vip = false;
if (tags && typeof(tags.badges) === 'string')
{
    tags.badges.split(',').forEach(badge => {
        badge = badge.split('/');
        if (badge[0].toLowerCase() == 'vip')
        {
            highlightSuffix = 'VIP';
            vip = true;
        }
    });
}
allowOtherBasedOnTags = vip;";

    public static string jCyan_Mod_Check = @"
var mod = false;

if (tags && typeof(tags.badges) === 'string')
{
    tags.badges.split(',').forEach(badge => {
        badge = badge.split('/');
        if (badge[0].toLowerCase() == 'moderator')
        {
            highlightSuffix = 'Mod';
            mod = true;
        }
    });
}
if (mod) { allowOtherBasedOnTags = true; }";
}
