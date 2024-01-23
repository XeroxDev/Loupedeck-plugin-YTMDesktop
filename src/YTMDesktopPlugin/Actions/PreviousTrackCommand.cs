namespace Loupedeck.YTMDesktopPlugin.Actions
{
    using System;

    using Services;

    using Utils;

    public class PreviousTrackCommand : PluginDynamicCommand
    {
        private SocketService SocketService { get; }

        public PreviousTrackCommand() : base("Previous track", "Goes to previous track", "Track") =>
            this.SocketService = SocketService.Instance;

        protected override async void RunCommand(String actionParameter) => await this.SocketService.TrackPrevious();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            DrawingHelper.LoadBitmapImage("music-prev");
    }
}