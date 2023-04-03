namespace Loupedeck.YTMDesktopPlugin
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Actions;

    using Services;

    using Utils.Enums;
    using Utils.Extensions;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class YTMDesktopPlugin : Plugin
    {
        public static YTMDesktopPlugin Instance { get; private set; }

        // Gets a value indicating whether this is an Universal plugin or an Application plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean HasNoApplication => true;

        private Subject<Boolean> OnDestroy { get; } = new Subject<Boolean>();

        // This method is called when the plugin is loaded during the Loupedeck service start-up.
        public override void Load()
        {
            Instance = this;
            this.FetchSettings();
            SocketService.Instance.IsConnected
                .DistinctUntilChanged()
                .TakeUntil(this.OnDestroy)
                .Subscribe(connected =>
                {
                    if (connected)
                    {
                        this.ResetStatus();
                    }
                    else
                    {
                        this.SetStatus(Loupedeck.PluginStatus.Error, ErrorCode.CouldNotConnect);
                    }
                });
        }

        // This method is called when the plugin is unloaded during the Loupedeck service shutdown.
        public override void Unload()
        {
            this.OnDestroy.OnNext(true);
            SocketService.Instance.Disconnect().ConfigureAwait(true);
        }

        private void FetchSettings()
        {
            this.TryGetSetting("host", out var host);
            this.TryGetSetting("port", out var port);
            this.TryGetSetting("password", out var password);
            this.TryGetSetting("playPauseFormat", out var playPauseFormat);

            SocketService.Instance.SetSettings(host ?? "127.0.0.1", port ?? "9863", password ?? "").Wait();
            PlayPauseCommand.Format = playPauseFormat ?? "{current}";
        }

        public Boolean TryGetSetting(String settingName, out String settingValue) =>
            this.TryGetPluginSetting(settingName, out settingValue);

        public void SetSetting(String settingName, String settingValue, Boolean backupOnline = false) =>
            this.SetPluginSetting(settingName, settingValue, backupOnline);

        public void DeleteSetting(String settingName) =>
            this.DeletePluginSetting(settingName);
    }
}