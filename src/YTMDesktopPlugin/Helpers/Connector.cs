// This file is part of the YTMDesktopPlugin project.
// 
// Copyright (c) 2024 Dominic Ris
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Loupedeck.YTMDesktopPlugin.Helpers;

using XeroxDev.YTMDesktop.Companion;
using XeroxDev.YTMDesktop.Companion.Clients;
using XeroxDev.YTMDesktop.Companion.Enums;
using XeroxDev.YTMDesktop.Companion.Exceptions;
using XeroxDev.YTMDesktop.Companion.Models.Output;
using XeroxDev.YTMDesktop.Companion.Settings;

internal static class Connector
{
    private static CompanionConnector? CompanionConnector { get; set; }
    public static RestClient? RestClient => CompanionConnector?.RestClient;
    private static SocketClient? SocketClient => CompanionConnector?.SocketClient;

    public static ESocketState ConnectionState = ESocketState.Disconnected;

    public static event EventHandler<Exception> OnError = delegate { };

    /// <summary>
    ///     The event that is raised when the socket connection is changed.
    /// </summary>
    public static event EventHandler<ESocketState> OnConnectionChange = delegate { };

    /// <summary>
    ///     The event that is raised when the YTMDesktop State has changed.
    /// </summary>
    public static event EventHandler<StateOutput> OnStateChange = delegate { };

    /// <summary>
    ///     The event that is raised when a playlist was created
    /// </summary>
    public static event EventHandler<PlaylistOutput> OnPlaylistCreated = delegate { };

    /// <summary>
    ///     The event that is raised when a playlist was deleted
    /// </summary>
    public static event EventHandler<String> OnPlaylistDeleted = delegate { };

    public static async Task Init(Plugin plugin, Boolean forceNewToken = false)
    {
        plugin.OnPluginStatusChanged(PluginStatus.Warning, "Connecting to YTMDesktop Companion...");
        var version = plugin.Assembly.GetName().Version?.ToString(3) ?? "0.0.0";
        plugin.TryGetPluginSetting("host", out var host);
        plugin.TryGetPluginSetting("port", out var portString);
        plugin.TryGetPluginSetting("token", out var token);

        if (host.IsNullOrEmpty() || host.Equals("localhost", StringComparison.InvariantCultureIgnoreCase))
        {
            host = "127.0.0.1";
        }

        var tryPortParse = Int32.TryParse(portString, out var port);

        if (!tryPortParse || port <= 0 || port > 65535)
        {
            port = 9863;
        }

        var settings = new ConnectorSettings(
            host,
            port,
            "xeroxdev-loupedeck",
            "Loupedeck Connector",
            version,
            token
        );

        if (CompanionConnector is null)
        {
            CompanionConnector = new CompanionConnector(settings);
            SocketClient!.OnError += (sender, e) => OnError(sender, e);
            SocketClient.OnConnectionChange += (sender, e) =>
            {
                OnConnectionChange(sender, e);
                ConnectionState = e;
            };
            SocketClient.OnStateChange += (sender, e) => OnStateChange(sender, e);
            SocketClient.OnPlaylistCreated += (sender, e) => OnPlaylistCreated(sender, e);
            SocketClient.OnPlaylistDeleted += (sender, e) => OnPlaylistDeleted(sender, e);
        }
        
        MetadataOutput? metadata = null;

        try
        {
            metadata = await RestClient!.GetMetadata();
        } catch (ApiException e)
        {
            plugin.OnPluginStatusChanged(PluginStatus.Error, e.Error.ToString());
        } catch (Exception e)
        {
            plugin.OnPluginStatusChanged(PluginStatus.Error, e.Message);
        }

        if (metadata is null)
        {
            if (!settings.Token.IsNullOrEmpty())
            {
                plugin.OnPluginStatusChanged(PluginStatus.Warning, "YTMDesktop is not started or can't be found. Due to last successful connection, Auto-Retry every 5 seconds is enabled", "https://help.xeroxdev.de/en/loupedeck/ytmd/home", "Visit Help Desk");
                await Task.Delay(5000);
                _ = Init(plugin, forceNewToken); // not awaited and discarded just so we don't recursively call the method and all waiting for each other
            }
            else
            {
                plugin.OnPluginStatusChanged(PluginStatus.Warning, "YTMDesktop is not started or can't be found.", "https://help.xeroxdev.de/en/loupedeck/ytmd/home", "Visit Help Desk");
            }
            return;
        }

        if (forceNewToken)
        {
            try
            {
                // If not, try to request one and show it to the user
                var code = await RestClient!.GetAuthCode();
                if (code is null)
                {
                    plugin.OnPluginStatusChanged(PluginStatus.Error, "Failed to get auth code. Probably the server is not running or the settings are wrong.");
                    return;
                }

                plugin.OnPluginStatusChanged(PluginStatus.Warning, $"Got new code, please compare it with the code from YTMDesktop: {code}");

                token = await RestClient.GetAuthToken(code);
                if (String.IsNullOrWhiteSpace(token))
                {
                    plugin.OnPluginStatusChanged(PluginStatus.Error, "Something went wrong...");
                    return;
                }

                // Save token to file
                plugin.SetPluginSetting("token", token);

                CompanionConnector.SetAuthToken(token);

                plugin.OnPluginStatusChanged(PluginStatus.Normal, "Token successfully saved.");
            }
            catch (ApiException e)
            {
                plugin.OnPluginStatusChanged(PluginStatus.Error, e.Error.ToString());
            }
            catch (Exception e)
            {
                plugin.OnPluginStatusChanged(PluginStatus.Error, e.Message);
            }
        }

        if (settings.Token.IsNullOrEmpty())
        {
            plugin.OnPluginStatusChanged(PluginStatus.Warning, "Plugin is not yet authenticated.", "https://help.xeroxdev.de/en/loupedeck/ytmd/home", "Visit Help Desk");
            return;
        }

        await SocketClient!.Connect();
        
        plugin.OnPluginStatusChanged(PluginStatus.Normal, "Connected to YTMDesktop Companion.");
    }
}