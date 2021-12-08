namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;

    using Services;

    using static Helper.DrawingHelper;

    public class PreviousTrackCommand : PluginDynamicCommand
    {
        private SocketService SocketService { get; }

        public PreviousTrackCommand() : base("Previous track", "Goes to previous track", "Track") =>
            this.SocketService = SocketService.Instance;

        protected override async void RunCommand(String actionParameter) => await this.SocketService.TrackPrevious();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            LoadBitmapImage("music-prev");
    }
}