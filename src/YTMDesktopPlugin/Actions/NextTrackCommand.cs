namespace Loupedeck.YTMDesktopPlugin.Actions;

using Helpers;

using XeroxDev.YTMDesktop.Companion.Exceptions;

public class NextTrackCommand() : PluginDynamicCommand("Next track", "Goes to next track", "Track")
{
    protected override async void RunCommand(String actionParameter)
    {
        try
        {
            await (Connector.RestClient?.Next() ?? Task.CompletedTask);
            this.Plugin.OnPluginStatusChanged(PluginStatus.Normal, "");
        }
        catch (ApiException e)
        {
            this.Plugin.OnPluginStatusChanged(PluginStatus.Error, e.Error.ToString());
        }
        catch (Exception e)
        {
            this.Plugin.OnPluginStatusChanged(PluginStatus.Error, e.Message);
        }
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        DrawingHelper.LoadBitmapImage("music-next");
}