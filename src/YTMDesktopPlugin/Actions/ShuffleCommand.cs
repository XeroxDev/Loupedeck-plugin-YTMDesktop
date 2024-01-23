namespace Loupedeck.YTMDesktopPlugin.Actions
{
    using System;

    using Services;

    using Utils;


    public class ShuffleCommand : PluginDynamicCommand
    {
        private SocketService SocketService { get; }

        public ShuffleCommand() : base("Shuffle", "Toggles shuffle", "Player") =>
            this.SocketService = SocketService.Instance;

        protected override async void RunCommand(String actionParameter) => await this.SocketService.PlayerShuffle();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            DrawingHelper.LoadBitmapImage($"shuffle");
    }
}