namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Services;

    using static Helper.DrawingHelper;

    public class RepeatCommand : PluginDynamicCommand
    {
        private SocketService SocketService { get; }
        private Subject<Boolean> OnDestroy { get; } = new();
        private String _repeatType = "None";


        public RepeatCommand() : base("Repeat", "Toggles repeat types", "Player") =>
            this.SocketService = SocketService.Instance;

        protected override Boolean OnLoad()
        {
            this.SocketService.OnTick
                .Select(response => response.Player.RepeatType)
                .DistinctUntilChanged()
                .TakeUntil(this.OnDestroy)
                .Subscribe(repeatType =>
                {
                    this._repeatType = repeatType;
                    this.ActionImageChanged();
                });

            return base.OnLoad();
        }

        protected override Boolean OnUnload()
        {
            this.OnDestroy.OnNext(true);
            return base.OnUnload();
        }

        protected override async void RunCommand(String actionParameter) => await this.SocketService.PlayerRepeat();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            LoadBitmapImage("repeat", this._repeatType);
    }
}