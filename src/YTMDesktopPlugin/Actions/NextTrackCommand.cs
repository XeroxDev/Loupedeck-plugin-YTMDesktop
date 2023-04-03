namespace Loupedeck.YTMDesktopPlugin.Actions
{
    using System;

    using Services;

    using Utils;

    public class NextTrackCommand : PluginDynamicCommand
    {
        private SocketService SocketService { get; }

        public NextTrackCommand() : base("Next track", "Goes to next track", "Track") =>
            this.SocketService = SocketService.Instance;

        protected override async void RunCommand(String actionParameter) => await this.SocketService.TrackNext();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            DrawingHelper.LoadBitmapImage("music-next");
    }
}