namespace Loupedeck.YTMDesktopPlugin
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Commands;

    using Enums;

    using Extensions;

    using Helper;

    using Services;

    public class YTMDesktopPlugin : Plugin
    {
        public static YTMDesktopPlugin Instance { get; private set; }
        public override Boolean UsesApplicationApiOnly => true;

        private Subject<Boolean> OnDestroy { get; } = new();

        public override void Load()
        {
            Instance = this;
            this.Info.Icon16x16 = DrawingHelper.ReadImage("icon-16", addPath: "Icon");
            this.Info.Icon32x32 = DrawingHelper.ReadImage("icon-32", addPath: "Icon");
            this.Info.Icon48x48 = DrawingHelper.ReadImage("icon-48", addPath: "Icon");
            this.Info.Icon256x256 = DrawingHelper.ReadImage("icon-256", addPath: "Icon");
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

        private void FetchSettings()
        {
            this.TryGetSetting("host", out var host);
            this.TryGetSetting("port", out var port);
            this.TryGetSetting("password", out var password);
            this.TryGetSetting("playPauseFormat", out var playPauseFormat);

            var socketInstance = SocketService.Instance;
            socketInstance.SetSettings(host ?? "127.0.0.1", port ?? "9863", password ?? "").Wait();
            PlayPauseCommand.Format = playPauseFormat ?? "{current}";
        }

        public override void Unload()
        {
            this.OnDestroy.OnNext(true);
            SocketService.Instance.Disconnect().ConfigureAwait(true);
        }

        public override void RunCommand(String commandName, String parameter)
        {
        }

        public override void ApplyAdjustment(String adjustmentName, String parameter, Int32 diff)
        {
        }

        public Boolean TryGetSetting(String settingName, out String settingValue) =>
            this.TryGetPluginSetting(settingName, out settingValue);

        public void SetSetting(String settingName, String settingValue, Boolean backupOnline = false) =>
            this.SetPluginSetting(settingName, settingValue, backupOnline);

        public void DeleteSetting(String settingName) =>
            this.DeletePluginSetting(settingName);
    }
}