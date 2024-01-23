namespace Loupedeck.YTMDesktopPlugin.Actions
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Services;

    using Utils;

    public class LikeCommand : PluginMultistateDynamicCommand
    {
        private SocketService SocketService { get; }
        private Subject<Boolean> OnDestroy { get; } = new Subject<Boolean>();

        private Boolean Liked { get; set; }

        public LikeCommand() : base("Like", "Likes track", "Track")
        {
            this.AddState("Neutral", "If current song is not liked");
            this.AddState("Liked", "If current song is liked");
            this.SocketService = SocketService.Instance;
        }

        protected override Boolean OnLoad()
        {
            this.SocketService.OnTick
                .Select(response => response.Player.LikeStatus == "LIKE")
                .DistinctUntilChanged(b => b == this.Liked)
                .TakeUntil(this.OnDestroy)
                .Subscribe(liked =>
                {
                    this.Liked = liked;
                    this.SetCurrentState(liked ? 1 : 0);
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

        protected override BitmapImage GetCommandImage(String actionParameter, Int32 state, PluginImageSize imageSize) =>
            DrawingHelper.LoadBitmapImage($"like-{(state == 1 ? "on" : "off")}");
    }
}