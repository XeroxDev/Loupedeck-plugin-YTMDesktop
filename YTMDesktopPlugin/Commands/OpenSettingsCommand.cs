namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;

    using GUI;

    using Services;

    using static Helper.DrawingHelper;

    public class OpenSettingsCommand : PluginDynamicCommand
    {
        public OpenSettingsCommand() : base("Open Settings", "Opens the settings", "General") {}

        protected override void RunCommand(String actionParameter) => Settings.Open();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            LoadBitmapImage(text: "Open settings");
    }
}