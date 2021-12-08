namespace Loupedeck.YTMDesktopPlugin.Classes.EventArgs
{
    using System;
    using System.Text.Json.Serialization;

    public class TrackAndPlayer
    {
        [JsonPropertyName("player")] public PlayerInfo Player { get; set; }
        [JsonPropertyName("track")] public TrackInfo Track { get; set; }
    }

    public class PlayerInfo
    {
        [JsonPropertyName("hasSong")] public Boolean HasSong { get; set; }
        [JsonPropertyName("isPaused")] public Boolean IsPaused { get; set; }
        [JsonPropertyName("volumePercent")] public Double VolumePercent { get; set; }

        [JsonPropertyName("seekbarCurrentPosition")]
        public Double SeekbarCurrentPosition { get; set; }

        [JsonPropertyName("seekbarCurrentPositionHuman")]
        public String SeekbarCurrentPositionHuman { get; set; }

        [JsonPropertyName("statePercent")] public Double StatePercent { get; set; }
        [JsonPropertyName("likeStatus")] public String LikeStatus { get; set; }
        [JsonPropertyName("repeatType")] public String RepeatType { get; set; }
    }

    public class TrackInfo
    {
        [JsonPropertyName("author")] public String Author { get; set; }
        [JsonPropertyName("title")] public String Title { get; set; }
        [JsonPropertyName("album")] public String Album { get; set; }
        [JsonPropertyName("cover")] public String Cover { get; set; }
        [JsonPropertyName("duration")] public Double Duration { get; set; }
        [JsonPropertyName("durationHuman")] public String DurationHuman { get; set; }
        [JsonPropertyName("url")] public String Url { get; set; }
        [JsonPropertyName("id")] public String Id { get; set; }
        [JsonPropertyName("isVideo")] public Boolean IsVideo { get; set; }
        [JsonPropertyName("isAdvertisement")] public Boolean IsAdvertisement { get; set; }
        [JsonPropertyName("inLibrary")] public Boolean InLibrary { get; set; }

        public void Deconstruct(out String author, out String title, out String album, out String cover, out Double duration, out String durationHuman, out String url, out String id, out Boolean isVideo, out Boolean isAdvertisement, out Boolean inLibrary)
        {
            author = this.Author;
            title = this.Title;
            album = this.Album;
            cover = this.Cover;
            duration = this.Duration;
            durationHuman = this.DurationHuman;
            url = this.Url;
            id = this.Id;
            isVideo = this.IsVideo;
            isAdvertisement = this.IsAdvertisement;
            inLibrary = this.InLibrary;
        }
    }
}