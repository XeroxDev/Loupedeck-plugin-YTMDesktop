namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Enums;

    using Extensions;

    using Services;

    using static Helper.DrawingHelper;

    public class PlayPauseCommand : PluginDynamicCommand
    {
        private Boolean Playing { get; set; } = true;
        private String Duration { get; set; } = "00:00";
        private Subject<Boolean> OnDestroy { get; } = new();
        private SocketService SocketService { get; }


        public PlayPauseCommand() : base("Play/Pause", "Pauses or resumes music track", "Track") =>
            this.SocketService = SocketService.Instance;

        protected override Boolean OnLoad()
        {
            this.SocketService.OnTick
                .DistinctUntilChanged()
                .TakeUntil(this.OnDestroy)
                .Subscribe(response =>
                {
                    this.Duration = response.Player.SeekbarCurrentPositionHuman;
                    this.Playing = !response.Player.IsPaused;
                    this.ActionImageChanged();
                });
            this.SocketService.IsConnected
                .DistinctUntilChanged()
                .TakeUntil(this.OnDestroy)
                .Subscribe(_ => this.ActionImageChanged());
            return base.OnLoad();
        }

        protected override Boolean OnUnload()
        {
            this.OnDestroy.OnNext(true);
            return base.OnUnload();
        }

        protected override async void RunCommand(String actionParameter)
        {
            this.Playing = !this.Playing;
            await this.SocketService.TrackPlayPause();
            this.ActionImageChanged(actionParameter);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            String text;
            String imageName;
            if (this.SocketService.IsConnected.Value)
            {
                if (this.Playing)
                {
                    text = this.Duration;
                    imageName = "music-pause";
                }
                else
                {
                    text = "Paused";
                    imageName = "music-play";
                }
            }
            else
            {
                text = "Not connected";
                imageName = "music-play";
            }

            return LoadBitmapImage(imageName, text);
        }
    }
}