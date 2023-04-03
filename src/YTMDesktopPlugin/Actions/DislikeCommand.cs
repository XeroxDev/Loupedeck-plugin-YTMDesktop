namespace Loupedeck.YTMDesktopPlugin.Actions
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Services;

    using Utils;

    public class DislikeCommand : PluginMultistateDynamicCommand
    {
        private SocketService SocketService { get; }
        private Subject<Boolean> OnDestroy { get; } = new Subject<Boolean>();

        private Boolean Disliked { get; set; }

        public DislikeCommand() : base("Dislike", "Dislikes track", "Track")
        {
            this.AddState("Neutral", "If current song is not disliked");
            this.AddState("Disliked", "If current song is disliked");
            
            this.SocketService = SocketService.Instance;
        }

        protected override Boolean OnLoad()
        {
            this.SocketService.OnTick
                .Select(response => response.Player.LikeStatus == "DISLIKE")
                .DistinctUntilChanged(b => b == this.Disliked)
                .TakeUntil(this.OnDestroy)
                .Subscribe(disliked =>
                {
                    this.Disliked = disliked;
                    this.SetCurrentState(disliked ? 1 : 0);
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

        protected override BitmapImage GetCommandImage(String actionParameter, Int32 state, PluginImageSize imageSize) =>
            DrawingHelper.LoadBitmapImage($"dislike-{(state == 1 ? "on" : "off")}");
    }
}