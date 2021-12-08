namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Services;

    using static Helper.DrawingHelper;

    public class LikeCommand : PluginDynamicCommand
    {
        private SocketService SocketService { get; }
        private Subject<Boolean> OnDestroy { get; } = new();

        private Boolean Liked { get; set; }

        public LikeCommand() : base("Like", "Likes track", "Track") =>
            this.SocketService = SocketService.Instance;

        protected override Boolean OnLoad()
        {
            this.SocketService.OnTick
                .Select(response => response.Player.LikeStatus == "LIKE")
                .DistinctUntilChanged(b => b == this.Liked)
                .TakeUntil(this.OnDestroy)
                .Subscribe(liked =>
                {
                    this.Liked = liked;
                    this.ActionImageChanged();
                });
            return base.OnLoad();
        }

        protected override Boolean OnUnload()
        {
            this.OnDestroy.OnNext(true);
            return base.OnUnload();
        }

        protected override async void RunCommand(String actionParameter) => await this.SocketService.TrackThumbsUp();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            LoadBitmapImage($"like-{(this.Liked ? "on" : "off")}");
    }
}