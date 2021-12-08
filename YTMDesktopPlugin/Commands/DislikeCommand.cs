namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Services;

    using static Helper.DrawingHelper;

    public class DislikeCommand : PluginDynamicCommand
    {
        private SocketService SocketService { get; }
        private Subject<Boolean> OnDestroy { get; } = new();

        private Boolean Disliked { get; set; }

        public DislikeCommand() : base("Dislike", "Dislikes track", "Track") =>
            this.SocketService = SocketService.Instance;

        protected override Boolean OnLoad()
        {
            this.SocketService.OnTick
                .Select(response => response.Player.LikeStatus == "DISLIKE")
                .DistinctUntilChanged(b => b == this.Disliked)
                .TakeUntil(this.OnDestroy)
                .Subscribe(liked =>
                {
                    this.Disliked = liked;
                    this.ActionImageChanged();
                });
            return base.OnLoad();
        }

        protected override Boolean OnUnload()
        {
            this.OnDestroy.OnNext(true);
            return base.OnUnload();
        }

        protected override async void RunCommand(String actionParameter) => await this.SocketService.TrackThumbsDown();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            LoadBitmapImage($"dislike-{(this.Disliked ? "on" : "off")}");
    }
}