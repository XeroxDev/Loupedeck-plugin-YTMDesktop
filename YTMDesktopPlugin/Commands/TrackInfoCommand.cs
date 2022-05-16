namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Classes.EventArgs;

    using Helper;

    using Services;

    using static Helper.DrawingHelper;

    public class TrackInfoCommand : PluginDynamicCommand
    {
        private Subject<Boolean> OnDestroy { get; } = new();
        private SocketService SocketService { get; }

        private Int32 TitleIndex { get; set; }
        private String CurrentTitle { get; set; } = "";
        private Int32 AuthorIndex { get; set; }
        private String CurrentAuthor { get; set; } = "";
        private Int32 AlbumIndex { get; set; }
        private String CurrentAlbum { get; set; } = "";
        private String CurrentThumbnail { get; set; } = "";
        private String CurrentUrl { get; set; } = "";

        private String DisplayTitle { get; set; } = "Nothing";
        private BitmapImage ThumbnailBitmap { get; set; }


        public TrackInfoCommand() : base("Track Info",
            "Shows thumbnail, title, author and album. On push opens link to track", "Track") =>
            this.SocketService = SocketService.Instance;

        protected override Boolean OnLoad()
        {
            this.SocketService.OnTick
                .DistinctUntilChanged()
                .TakeUntil(this.OnDestroy)
                .Subscribe(response =>
                {
                    var (author, title, album, cover, _, _, url, _, _, _, _) = this.GetSongData(response);

                    if (this.CurrentTitle != title)
                    {
                        this.TitleIndex = 0;
                    }

                    if (this.CurrentAlbum != album)
                    {
                        this.AlbumIndex = 0;
                    }

                    if (this.CurrentAuthor != author)
                    {
                        this.AuthorIndex = 0;
                    }

                    if (this.CurrentThumbnail != cover)
                    {
                        this.CurrentThumbnail = cover;
                        if (!cover.StartsWith("http"))
                        {
                            this.ThumbnailBitmap = LoadBitmapImage(text: cover);
                        }
                        else
                        {
                            try
                            {
                                using var ms = new MemoryStream();
                                GetImageAsStream(cover).CopyTo(ms);
                                this.ThumbnailBitmap = BitmapImage.FromArray(ms.ToArray());
                            }
                            catch
                            {
                                this.ThumbnailBitmap = LoadBitmapImage(text: "Not found");
                            }
                        }

                        this.ThumbnailBitmap.Resize(90, 90);
                    }

                    this.CurrentTitle = title;
                    this.CurrentAuthor = author;
                    this.CurrentAlbum = album;
                    this.CurrentThumbnail = cover;
                    this.CurrentUrl = url;

                    var displayTitle = this.CurrentTitle;
                    var displayAlbum = this.CurrentAlbum == displayTitle ? "" : this.CurrentAlbum;
                    var displayAuthor = this.CurrentAuthor == this.CurrentTitle ? "" : this.CurrentAuthor;

                    var max = 7;
                    if (String.IsNullOrEmpty(displayTitle)
                        && String.IsNullOrEmpty(displayAlbum)
                        && String.IsNullOrEmpty(displayAuthor))
                    {
                        return;
                    }

                    if (this.CurrentTitle.Length > max && !String.IsNullOrEmpty(displayTitle))
                    {
                        displayTitle =
                            this.GetScrollingText("".PadLeft(max, '.') + displayTitle + "".PadLeft(max, '.'),
                                this.TitleIndex, max);
                        this.TitleIndex++;
                        if (this.CurrentTitle.Length - this.TitleIndex + max * 2 < max)
                        {
                            this.TitleIndex = 0;
                        }
                    }

                    if (this.CurrentAuthor.Length > max && !String.IsNullOrEmpty(displayAuthor))
                    {
                        displayAuthor =
                            this.GetScrollingText("".PadLeft(max, '.') + displayAuthor + "".PadLeft(max, '.'),
                                this.AuthorIndex, max);
                        this.AuthorIndex++;
                        if (this.CurrentAuthor.Length - this.AuthorIndex + max * 2 < max)
                        {
                            this.AuthorIndex = 0;
                        }
                    }

                    if (this.CurrentAlbum.Length > max && !String.IsNullOrEmpty(displayAlbum))
                    {
                        displayAlbum =
                            this.GetScrollingText("".PadLeft(max, '.') + displayAlbum + "".PadLeft(max, '.'),
                                this.AlbumIndex, max);
                        this.AlbumIndex++;
                        if (this.CurrentAlbum.Length - this.AlbumIndex + max * 2 < max)
                        {
                            this.AlbumIndex = 0;
                        }
                    }

                    this.DisplayTitle = $"{displayTitle}\n{displayAlbum}\n{displayAuthor}";
                    this.ActionImageChanged();
                });
            return base.OnLoad();
        }

        private TrackInfo GetSongData(TrackAndPlayer data)
        {
            var hasSong = data.Player.HasSong;
            var isPaused = data.Player.IsPaused;

            if (isPaused)
            {
                data.Track.Title = "Paused";
                data.Track.Author = "Paused";
                data.Track.Album = "Paused";
            }
            else if (!hasSong)
            {
                data.Track.Cover = "N/A";
                data.Track.Title = "N/A";
                data.Track.Author = "N/A";
                data.Track.Album = "N/A";
            }

            return data.Track;
        }

        private String GetScrollingText(String text, Int32 index, Int32 max) => text.Substring(index, max);


        protected override Boolean OnUnload()
        {
            this.OnDestroy.OnNext(true);
            return base.OnUnload();
        }

        protected override void RunCommand(String actionParameter) => Process.Start(this.CurrentUrl);

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            LoadBitmapImage(this.ThumbnailBitmap,
                this.SocketService.IsConnected.Value ? this.DisplayTitle : "Not connected");

        private static Stream GetImageAsStream(String urlImage)
        {
            var request = WebRequest.Create(urlImage);
            var response = request.GetResponse();
            return response.GetResponseStream();
        }
    }
}