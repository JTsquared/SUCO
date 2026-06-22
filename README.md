# SUCO - Stream Unified Chat Overlay

[![GitHub](https://img.shields.io/github/license/JTsquared/SUCO)](https://raw.githubusercontent.com/JTsquared/SUCO/master/LICENSE)
[![GitHub release](https://img.shields.io/github/v/release/JTsquared/SUCO)](https://github.com/JTsquared/SUCO/releases)

A Windows desktop app and browser overlay that displays live chat from multiple streaming platforms in a single, unified view. Designed for streamers who broadcast to more than one platform and want all their chat in one place.

## Supported Platforms

| Platform | Status | Connection Method |
|----------|--------|-------------------|
| **Blaze** | Supported | Socket.IO EventSub (real-time) |
| **Twitch** | Supported | Anonymous IRC WebSocket (real-time) |
| **Kick** | Supported | Public Pusher WebSocket (real-time) |
| **Arena** | Supported | Server-side polling via SSE proxy |
| **YouTube** | Coming soon | — |

## Features

- **Unified chat** — Messages from all platforms appear in one view with colored platform badges
- **Transparent overlay** — Sits on top of your game window so you can read chat while playing
- **Browser source** — Use as an OBS Browser Source without installing anything
- **Web settings** — Configure your overlay at [blazegames.store/suco/settings](https://blazegames.store/suco/settings)
- **Customizable appearance** — Text size, font, color, background opacity, fade timeout
- **Settings sync** — Sync appearance between the desktop app and browser overlay
- **Auto-updates** — Desktop app checks for updates automatically via GitHub Releases
- **No API keys needed** — Twitch and Kick connect anonymously; Blaze and Arena tokens are handled by the server

## Quick Start

### Option 1: Browser Source (no install required)

1. Go to [blazegames.store/suco/settings](https://blazegames.store/suco/settings)
2. Login with Blaze
3. Enter your channel names for each platform
4. Customize appearance
5. Save, then copy the OBS URL
6. Add it as a **Browser Source** in OBS

### Option 2: Desktop App

1. Download the latest `SUCO-Setup.exe` from [Releases](https://github.com/JTsquared/SUCO/releases)
2. Run the installer (no admin required)
3. Select **"Blaze Chat"** from the chat type dropdown in Settings
4. Enter your channel names for each platform
5. Adjust text size, font, background, etc.
6. Click Save — chat appears in the overlay

### Desktop App Usage

- **Move** the overlay by dragging the border at the top
- **Resize** by dragging the grip at the bottom-right
- **Hide borders** by clicking the button in the top-right (or use the hotkey)
- **Settings** — right-click the title bar or the system tray icon
- **Toggle interaction** — `Ctrl+Alt+F7` (click-through mode for gaming)
- **Toggle borders** — `Ctrl+Alt+F9`

## Building from Source

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 22+](https://nodejs.org/)

### Build

```bash
# 1. Clone the repo
git clone https://github.com/JTsquared/SUCO.git
cd SUCO

# 2. Build the NativeChatClient
cd NativeChatClient
npm install
npm run build
cd ..

# 3. Build and run the WPF app
cd TransparentTwitchChatWPF
dotnet run
```

## Architecture

- **Desktop app** — C#/.NET 8 WPF with WebView2 for rendering chat
- **Browser overlay** — Standalone HTML/JS served from the SUCO proxy server
- **SUCO proxy** — Node.js/Express server that handles Blaze OAuth tokens, Arena chat polling, and settings storage
- **Settings sync** — Desktop app and browser overlay can share settings via the proxy API

## Credits

This project is adapted from [Transparent Twitch Chat Overlay](https://github.com/baffler/Transparent-Twitch-Chat-Overlay) by [Baffler](https://github.com/baffler). The original project provided the WPF overlay framework, WebView2 integration, and Twitch chat support that SUCO builds upon.

SUCO extends the original with multi-platform chat support (Blaze, Kick, Arena), a web-based browser source overlay, server-side settings management, and OAuth-protected configuration.

## License

This project is licensed under the [GNU General Public License v3.0](LICENSE), the same license as the original project.
