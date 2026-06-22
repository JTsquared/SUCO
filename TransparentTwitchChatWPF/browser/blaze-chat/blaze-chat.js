// Blaze Chat Overlay Client
// Connects to Blaze Stream EventSub via Socket.IO for real-time chat messages.

(function () {
    'use strict';

    const MAX_MESSAGES = 50;
    const API_BASE = 'https://api.blaze.stream/v1';
    const SOCKET_URL = 'https://blaze.stream';
    const SOCKET_PATH = '/ws';

    let config = {
        channelId: '',
        clientId: '',
        accessToken: '',
        fadeTimeout: 0
    };

    let socket = null;
    let sessionId = null;
    const blazeChatHost = window.chrome && window.chrome.webview ? window.chrome.webview : null;

    // --- Status display ---

    function showStatus(text, duration) {
        const el = document.getElementById('status_text');
        el.textContent = text;
        el.classList.add('visible');
        if (duration > 0) {
            setTimeout(() => el.classList.remove('visible'), duration);
        }
    }

    function hideStatus() {
        document.getElementById('status_text').classList.remove('visible');
    }

    // --- Chat rendering ---

    function escapeHtml(str) {
        const div = document.createElement('div');
        div.appendChild(document.createTextNode(str));
        return div.innerHTML;
    }

    function getUserColor(username) {
        let hash = 0;
        for (let i = 0; i < username.length; i++) {
            hash = username.charCodeAt(i) + ((hash << 5) - hash);
        }
        const hue = Math.abs(hash) % 360;
        return 'hsl(' + hue + ', 70%, 65%)';
    }

    function renderBadges(roles) {
        let html = '';
        if (!roles || !Array.isArray(roles)) return html;

        if (roles.includes('moderator')) {
            html += '<span class="badge badge-mod" title="Moderator">M</span>';
        }
        if (roles.includes('vip')) {
            html += '<span class="badge badge-vip" title="VIP">V</span>';
        }
        if (roles.includes('og')) {
            html += '<span class="badge badge-og" title="OG">OG</span>';
        }
        if (roles.includes('subscriber')) {
            html += '<span class="badge badge-sub" title="Subscriber">S</span>';
        }
        return html;
    }

    function addChatMessage(data) {
        const container = document.getElementById('chat_container');

        // Extract sender info from the EventSub payload
        const sender = data.sender || {};
        const displayName = sender.displayName || sender.username || 'Anonymous';
        const color = getUserColor(displayName);
        const badges = renderBadges(sender.roles);
        const text = escapeHtml(data.message || '');

        const line = document.createElement('div');
        line.className = 'chat-line';
        line.dataset.messageId = data.messageId || '';
        line.innerHTML =
            badges +
            '<span class="username" style="color:' + color + '">' + escapeHtml(displayName) + '</span>' +
            '<span class="separator">: </span>' +
            '<span class="message-text">' + text + '</span>';

        container.appendChild(line);

        // Prune old messages
        while (container.children.length > MAX_MESSAGES) {
            container.removeChild(container.firstChild);
        }

        // Scroll to bottom
        container.scrollTop = container.scrollHeight;

        // Fade out after timeout
        if (config.fadeTimeout > 0) {
            setTimeout(function () {
                line.classList.add('fading');
                setTimeout(function () {
                    if (line.parentNode) {
                        line.parentNode.removeChild(line);
                    }
                }, 1000);
            }, config.fadeTimeout * 1000);
        }
    }

    function handleMessageDelete(data) {
        const messageId = data.messageId;
        if (!messageId) return;

        const el = document.querySelector('[data-message-id="' + messageId + '"]');
        if (el) {
            el.classList.add('fading');
            setTimeout(function () {
                if (el.parentNode) el.parentNode.removeChild(el);
            }, 500);
        }
    }

    function handleChatClear() {
        document.getElementById('chat_container').innerHTML = '';
    }

    // --- Resolve channel name to channel ID ---

    async function resolveChannelId(channelName) {
        if (!config.clientId || !config.accessToken) return null;

        try {
            // Blaze API uses slug[] parameter to look up channels by name
            const url = API_BASE + '/channels?slug[]=' + encodeURIComponent(channelName);
            console.log('[BlazeChat] Resolving channel via:', url);

            const response = await fetch(url, {
                headers: {
                    'Authorization': 'Bearer ' + config.accessToken,
                    'client-id': config.clientId,
                    'Accept': 'application/json'
                }
            });

            console.log('[BlazeChat] Channel lookup response:', response.status);

            if (!response.ok) {
                const errText = await response.text();
                console.error('[BlazeChat] Channel lookup failed:', response.status, errText);
                return null;
            }

            const data = await response.json();
            console.log('[BlazeChat] Channel lookup result:', JSON.stringify(data).substring(0, 300));

            // Response could be { channels: [...] } or just an array
            const channels = data.channels || data;
            if (Array.isArray(channels) && channels.length > 0) return channels[0].id;
            if (data && data.id) return data.id;
            return null;
        } catch (err) {
            console.error('[BlazeChat] Failed to resolve channel:', err);
            return null;
        }
    }

    // --- Socket.IO EventSub ---

    async function subscribeToEvent(type, channelId) {
        try {
            const response = await fetch(API_BASE + '/events/subscriptions', {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + config.accessToken,
                    'client-id': config.clientId,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    type: type,
                    version: '1',
                    sessionId: sessionId,
                    condition: {
                        channelId: channelId
                    }
                })
            });

            if (!response.ok) {
                const errBody = await response.text();
                console.error('EventSub subscribe failed for ' + type + ':', response.status, errBody);
                return false;
            }

            console.log('Subscribed to ' + type);
            return true;
        } catch (err) {
            console.error('EventSub subscribe error for ' + type + ':', err);
            return false;
        }
    }

    function connectSocket() {
        if (socket) {
            socket.disconnect();
            socket = null;
        }

        showStatus('Connecting to Blaze chat...', 0);

        socket = io(SOCKET_URL, {
            path: SOCKET_PATH,
            transports: ['websocket', 'polling'],
            reconnection: true,
            reconnectionDelay: 2000,
            reconnectionAttempts: 10
        });

        socket.on('connect', function () {
            console.log('Socket.IO connected');
        });

        socket.on('disconnect', function (reason) {
            console.log('Socket.IO disconnected:', reason);
            showStatus('Disconnected from Blaze chat. Reconnecting...', 5000);
            sessionId = null;
        });

        socket.on('connect_error', function (err) {
            console.error('Socket.IO connection error:', err.message);
            showStatus('Connection error: ' + err.message, 5000);
        });

        // Listen for EventSub messages
        socket.on('eventsub', async function (data) {
            if (!data || !data.metadata) return;

            const messageType = data.metadata.messageType;

            if (messageType === 'session_welcome') {
                // Extract sessionId and subscribe to chat events
                sessionId = data.payload && data.payload.session
                    ? data.payload.session.id
                    : (data.payload ? data.payload.sessionId : null);

                if (!sessionId) {
                    console.error('No sessionId in welcome message:', data);
                    showStatus('Failed to get session ID', 5000);
                    return;
                }

                console.log('Got sessionId:', sessionId);

                // Subscribe to chat events
                const chatOk = await subscribeToEvent('channel.chat.message', config.channelId);
                const deleteOk = await subscribeToEvent('channel.chat.message_delete', config.channelId);
                const clearOk = await subscribeToEvent('channel.chat.clear', config.channelId);

                if (chatOk) {
                    showStatus('Connected to Blaze chat', 3000);
                } else {
                    showStatus('Failed to subscribe to chat events', 5000);
                }
            }
            else if (messageType === 'notification') {
                const subType = data.metadata.subscriptionType;
                const payload = data.payload || {};

                if (subType === 'channel.chat.message') {
                    addChatMessage(payload);
                }
                else if (subType === 'channel.chat.message_delete') {
                    handleMessageDelete(payload);
                }
                else if (subType === 'channel.chat.clear') {
                    handleChatClear();
                }
            }
        });
    }

    // --- C# bridge communication ---

    console.log('[BlazeChat] Script loaded, host bridge:', blazeChatHost ? 'available' : 'unavailable');

    if (blazeChatHost) {
        blazeChatHost.addEventListener('message', async function (event) {
            const message = event.data;
            console.log('[BlazeChat] Received message from C#:', JSON.stringify(message).substring(0, 200));

            switch (message.type) {
                case 'blazeConfig':
                    config.clientId = message.payload.clientId || '';
                    config.accessToken = message.payload.accessToken || '';
                    config.fadeTimeout = message.payload.fadeTimeout || 0;

                    console.log('[BlazeChat] Config received - clientId:', config.clientId ? 'present' : 'MISSING',
                        'token:', config.accessToken ? 'present' : 'MISSING');

                    var channelInput = message.payload.channel || '';

                    // If it looks like a UUID, use it directly; otherwise resolve the name
                    if (/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(channelInput)) {
                        config.channelId = channelInput;
                    } else if (channelInput) {
                        showStatus('Resolving channel: ' + channelInput + '...', 0);
                        var resolved = await resolveChannelId(channelInput);
                        if (resolved) {
                            config.channelId = resolved;
                        } else {
                            showStatus('Could not find Blaze channel: ' + channelInput, 5000);
                            return;
                        }
                    }

                    console.log('[BlazeChat] Channel resolved:', config.channelId || 'NONE');

                    if (config.channelId && config.accessToken) {
                        console.log('[BlazeChat] Starting Socket.IO connection...');
                        connectSocket();
                    } else {
                        console.error('[BlazeChat] Missing -', !config.channelId ? 'channelId' : '', !config.accessToken ? 'accessToken' : '');
                        showStatus('Missing channel or credentials', 5000);
                    }
                    break;

                default:
                    console.warn('Unknown message type:', message.type);
            }
        });

        // Notify the C# host that we're ready
        blazeChatHost.postMessage({
            type: 'BlazeChatReady',
            protocolVersion: 1
        });
    } else {
        console.warn('Blaze Chat host bridge is unavailable.');
        showStatus('No host bridge - running standalone', 0);
    }
})();
