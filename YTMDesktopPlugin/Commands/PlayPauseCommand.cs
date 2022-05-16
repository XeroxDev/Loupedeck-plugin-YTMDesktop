namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Enums;

    using Extensions;

    using Services;

    using static Helper.DrawingHelper;

    public class PlayPauseCommand : PluginDynamicCommand
    {
        private Boolean Playing { get; set; } = true;
        private Int32 Current { get; set; }
        private Int32 Duration { get; set; }
        private Int32 Remaining { get; set; }
        private Subject<Boolean> OnDestroy { get; } = new();
        private SocketService SocketService { get; }
        public static String Format = "{current}";


        public PlayPauseCommand() : base("Play/Pause", "Pauses or resumes music track", "Track") =>
            this.SocketService = SocketService.Instance;

        protected override Boolean OnLoad()
        {
            this.SocketService.OnTick
                .DistinctUntilChanged()
                .TakeUntil(this.OnDestroy)
                .Subscribe(response =>
                {
                    this.Current = Convert.ToInt32(response.Player.SeekbarCurrentPosition);
                    this.Duration = Convert.ToInt32(response.Track.Duration);
                    this.Remaining = this.Duration - this.Current;
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
                    text = FormatTitle(Format, this.Current, this.Duration, this.Remaining);
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


        public static String FormatTitle(String format, Int32 current = 0, Int32 duration = 0, Int32 remaining = 0)
        {
            IDictionary<String, String> varMapping = new Dictionary<String, String>
            {
                { "current", FormatTime(current) },
                { "current:S", current.ToString() },
                { "duration", FormatTime(duration) },
                { "duration:S", duration.ToString() },
                { "remaining", FormatTime(remaining) },
                { "remaining:S", remaining.ToString() }
            };

            return varMapping.Aggregate(format, (current1, kv) => current1.Replace($"{{{kv.Key}}}", kv.Value));
        }

        private static String FormatTime(Int32 seconds)
        {
            var minutes = seconds / 60;
            var secondsLeft = seconds % 60;
            var hours = minutes / 60;
            var minutesLeft = minutes % 60;

            var result = "";

            if (hours > 0)
            {
                result += $"{hours:D2}:";
            }

            result += $"{minutesLeft:D2}:{secondsLeft:D2}";

            return result;
        }
    }
}