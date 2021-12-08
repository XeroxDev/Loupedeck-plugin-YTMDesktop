namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;

    using Services;

    using static Helper.DrawingHelper;


    public class ShuffleCommand : PluginDynamicCommand
    {
        private SocketService SocketService { get; }

        public ShuffleCommand() : base("Shuffle", "Toggles shuffle", "Player") =>
            this.SocketService = SocketService.Instance;

        protected override async void RunCommand(String actionParameter) => await this.SocketService.PlayerShuffle();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            LoadBitmapImage($"shuffle");
    }
}