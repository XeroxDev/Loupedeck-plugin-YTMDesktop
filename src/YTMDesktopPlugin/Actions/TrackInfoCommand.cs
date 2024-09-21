namespace Loupedeck.YTMDesktopPlugin.Actions;

using Helpers;

using XeroxDev.YTMDesktop.Companion.Enums;
using XeroxDev.YTMDesktop.Companion.Models.Output;

using static Helpers.DrawingHelper;

public class TrackInfoCommand() : PluginDynamicCommand("Track Info", "Shows thumbnail, title, author and album. On push opens link to track", "Track")
{
    private Int32 _titleIndex;
    private String _currentTitle = "";
    private Int32 _authorIndex;
    private String _currentAuthor = "";
    private Int32 _albumIndex;
    private String _currentAlbum = "";
    private String _currentThumbnail = "";
    private String _displayTitle = "Nothing";
    private BitmapImage? _thumbnailBitmap;


    protected override Boolean OnLoad()
    {
        Connector.OnStateChange += this.OnStateChange;
        return base.OnLoad();
    }

    private void OnStateChange(Object? _, StateOutput e)
    {
        var data = GetSongData(e);
        var title = data.Video?.Title;
        var author = data.Video?.Author;
        var album = data.Video?.Album;
        var cover = data.Video?.Thumbnails?.Length > 0 ? data.Video?.Thumbnails[0].Url ?? null : null;

        if (this._currentTitle != title)
        {
            this._titleIndex = 0;
        }

        if (this._currentAlbum != album)
        {
            this._albumIndex = 0;
        }

        if (this._currentAuthor != author)
        {
            this._authorIndex = 0;
        }

        if (this._currentThumbnail != cover && !String.IsNullOrEmpty(cover))
        {
            this._currentThumbnail = cover;
            if (!cover.StartsWith("http"))
            {
                this._thumbnailBitmap = LoadBitmapImage(text: cover);
            }
            else
            {
                try
                {
                    using var ms = new MemoryStream();
                    GetImageAsStream(cover).CopyTo(ms);
                    this._thumbnailBitmap = BitmapImage.FromArray(ms.ToArray());
                }
                catch
                {
                    this._thumbnailBitmap = LoadBitmapImage(text: "Not found");
                }
            }

            this._thumbnailBitmap.Resize(90, 90);
        }

        this._currentTitle = title ?? this._currentTitle;
        this._currentAuthor = author ?? this._currentAuthor;
        this._currentAlbum = album ?? this._currentAlbum;
        this._currentThumbnail = cover ?? this._currentThumbnail;

        var displayTitle = this._currentTitle;
        var displayAlbum = this._currentAlbum == displayTitle ? "" : this._currentAlbum;
        var displayAuthor = this._currentAuthor == this._currentTitle ? "" : this._currentAuthor;

        const Int32 max = 7;
        if (String.IsNullOrEmpty(displayTitle)
            && String.IsNullOrEmpty(displayAlbum)
            && String.IsNullOrEmpty(displayAuthor))
        {
            return;
        }

        if (this._currentTitle.Length > max && !String.IsNullOrEmpty(displayTitle))
        {
            displayTitle =
                GetScrollingText("".PadLeft(max, '.') + displayTitle + "".PadLeft(max, '.'),
                    this._titleIndex, max);
            this._titleIndex++;
            if (this._currentTitle.Length - this._titleIndex + max * 2 < max)
            {
                this._titleIndex = 0;
            }
        }

        if (this._currentAuthor.Length > max && !String.IsNullOrEmpty(displayAuthor))
        {
            displayAuthor =
                GetScrollingText("".PadLeft(max, '.') + displayAuthor + "".PadLeft(max, '.'),
                    this._authorIndex, max);
            this._authorIndex++;
            if (this._currentAuthor.Length - this._authorIndex + max * 2 < max)
            {
                this._authorIndex = 0;
            }
        }
        
        if (this._currentAlbum.Length > max && !String.IsNullOrEmpty(displayAlbum))
        {
            displayAlbum =
                GetScrollingText("".PadLeft(max, '.') + displayAlbum + "".PadLeft(max, '.'),
                    this._albumIndex, max);
            this._albumIndex++;
            if (this._currentAlbum.Length - this._albumIndex + max * 2 < max)
            {
                this._albumIndex = 0;
            }
        }

        this._displayTitle = $"{displayTitle}\n{displayAlbum}\n{displayAuthor}";
        this.ActionImageChanged();
    }

    private static StateOutput GetSongData(StateOutput data)
    {
        var isPaused = (data.Player?.TrackState ?? ETrackState.Unknown) == ETrackState.Paused;

        if (!isPaused)
        {
            return data;
        }

        data.Video.Title = "Paused";
        data.Video.Author = "Paused";
        data.Video.Album = "Paused";

        return data;
    }

    private static String GetScrollingText(String text, Int32 index, Int32 max) => text.Substring(index, max);

    protected override void RunCommand(String actionParameter) => this.Plugin.OnPluginStatusChanged(PluginStatus.Normal, "");

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        this._thumbnailBitmap is null
            ? LoadBitmapImage(text: "Not found")
            : LoadBitmapImage(this._thumbnailBitmap, Connector.ConnectionState == ESocketState.Connected ? this._displayTitle : Enum.GetName(typeof(ESocketState), Connector.ConnectionState));

    private static Stream GetImageAsStream(String urlImage)
    {
        using var client = new HttpClient();
        return client.GetStreamAsync(urlImage).Result;
    }
}