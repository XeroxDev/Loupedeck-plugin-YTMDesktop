namespace Loupedeck.YTMDesktopPlugin.Actions
{
    using System;

    using GUI;

    using Utils;

    public class OpenSettingsCommand : PluginDynamicCommand
    {
        public OpenSettingsCommand() : base("Open Settings", "Opens the settings", "General") {}

        protected override void RunCommand(String actionParameter) => Settings.Open();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            DrawingHelper.LoadBitmapImage(text: "Open settings");
    }
}