namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;

    using Services;

    public class ToggleVolumeCommand : PluginDynamicCommand
    {
        private SocketService SocketService { get; }

        public ToggleVolumeCommand() : base("Toggle volume", "Toggles the volume", "Player")
            => this.SocketService = SocketService.Instance;


        protected override async void RunCommand(String actionParameter)
        {
            var vol = VolumeAdjustment.OnVolume.Value == 0
                ? VolumeAdjustment.LastVolume
                : -1;
            VolumeAdjustment.OnVolume.OnNext(vol);
            await this.SocketService.PlayerSetVolume(vol);
        }
    }
}