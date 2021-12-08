namespace Loupedeck.YTMDesktopPlugin.Services
{
    using System;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;

    using Classes.EventArgs;

    using H.Socket.IO;

    using Newtonsoft.Json;

    public sealed class SocketService
    {
        public static SocketService Instance => Lazy.Value;
        public Subject<TrackAndPlayer> OnTick { get; } = new();
        public Subject<String> OnError { get; } = new();
        public Subject<OnConnectedEvent> OnConnected { get; } = new();

        public BehaviorSubject<Boolean> IsConnected { get; } = new(false);

        private String _host = "127.0.0.1";
        private String _port = "9863";
        private String _password = "";

        private static readonly Lazy<SocketService> Lazy = new(() => new SocketService());
        private SocketIoClient Client { get; set; }
        private SocketService() => this.SetEvents();

        private void SetEvents()
        {
            this.Client = new SocketIoClient();
            this.OnError.Subscribe(_ => this.IsConnected.OnNext(false));
            this.Client.ErrorReceived += (_, args) => this.OnError.OnNext(args.Value);
            this.Client.ExceptionOccurred += (sender, args) => this.OnError.OnNext(args.Value.Message);
            this.Client.Disconnected += (sender, args) => this.OnError.OnNext(args.Reason);
            this.Client.Connected += (sender, e) =>
            {
                this.IsConnected.OnNext(true);
                this.OnConnected.OnNext(new OnConnectedEvent(sender, e));
            };
            this.Client.On<TrackAndPlayer>("tick", response =>
            {
                this.IsConnected.OnNext(true);
                this.OnTick.OnNext(response);
            });
        }

        public async Task SetSettings(String host = "127.0.0.1", String port = "9863", String password = "")
        {
            this._host = host;
            this._port = port;
            this._password = password;
            if (this.IsConnected.Value)
            {
                await this.Disconnect();
                await this.StartConnection();
            }
        }

        public async Task StartConnection()
        {
            try
            {
                await this.Client.ConnectAsync(new Uri($"http://{this._host}:{this._port}/?password={this._password}"));
            }
            catch (Exception e)
            {
                this.OnError.OnNext(e.Message);
                await Task.Delay(1000);
                await this.StartConnection();
            }
        }

        #region Track functions

        public async Task TrackPlayPause() => await this.Emit(cmd: "track-play-pause");
        public async Task TrackPlay() => await this.Emit(cmd: "track-play");
        public async Task TrackPause() => await this.Emit(cmd: "track-pause");
        public async Task TrackNext() => await this.Emit(cmd: "track-next");
        public async Task TrackPrevious() => await this.Emit(cmd: "track-previous");
        public async Task TrackThumbsUp() => await this.Emit(cmd: "track-thumbs-up");
        public async Task TrackThumbsDown() => await this.Emit(cmd: "track-thumbs-down");

        #endregion

        #region Player functions

        public async Task PlayerVolumeUp() => await this.Emit(cmd: "player-volume-up");
        public async Task PlayerVolumeDown() => await this.Emit(cmd: "player-volume-down");
        public async Task PlayerForward() => await this.Emit(cmd: "player-forward");
        public async Task PlayerRewind() => await this.Emit(cmd: "player-rewind");

        public async Task PlayerSetSeekbar(Double seconds) =>
            await this.Emit(cmd: "player-set-seekbar", value: seconds);

        public async Task PlayerSetVolume(Int32 percentage) =>
            await this.Emit(cmd: "player-set-volume", value: percentage);

        public async Task PlayerSetQueue(Int32 index) =>
            await this.Emit(cmd: "player-set-queue", value: index);

        public async Task PlayerRepeat() => await this.Emit(cmd: "player-repeat");
        public async Task PlayerShuffle() => await this.Emit(cmd: "player-shuffle");
        public async Task PlayerAddLibrary() => await this.Emit(cmd: "player-add-library");
        public async Task PlayerAddPlaylist(Int32 index) => await this.Emit(cmd: "player-add-playlist", value: index);

        #endregion

        #region Other / request functions

        public async Task ShowLyricsHidden() => await this.Emit(cmd: "show-lyrics-hidden");
        public async Task RequestPlayer() => await this.Emit("query-player");
        public async Task RequestTrack() => await this.Emit("query-track");
        public async Task RequestQueue() => await this.Emit("query-queue");
        public async Task RequestPlaylist() => await this.Emit("query-playlist");
        public async Task RequestLyrics() => await this.Emit("query-lyrics");

        #endregion


        public async Task Disconnect() => await this.Client.DisconnectAsync();

        private async Task Emit(String evt = "media-commands", String cmd = "player-rewind", Object value = null) =>
            await this.Client
                .SendEventAsync(
                    $"[{String.Join(",", $"\"{evt}\"", $"\"{cmd}\"", JsonConvert.SerializeObject(value ?? true))}]")
                .ConfigureAwait(false);
    }
}