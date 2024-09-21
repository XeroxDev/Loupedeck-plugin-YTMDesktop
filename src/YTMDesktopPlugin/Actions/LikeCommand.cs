namespace Loupedeck.YTMDesktopPlugin.Actions;

using Helpers;

using XeroxDev.YTMDesktop.Companion.Enums;
using XeroxDev.YTMDesktop.Companion.Exceptions;
using XeroxDev.YTMDesktop.Companion.Models.Output;

public class LikeCommand : PluginMultistateDynamicCommand
{
    private Boolean _liked;

    public LikeCommand() : base("Like", "Likes track", "Track")
    {
        this.AddState("Neutral", "If current song is not liked");
        this.AddState("Liked", "If current song is liked");
    }

    protected override Boolean OnLoad()
    {
        Connector.OnStateChange += this.OnStateChange;
        return base.OnUnload();
    }

    protected override Boolean OnUnload()
    {
        Connector.OnStateChange -= this.OnStateChange;
        return base.OnUnload();
    }

    private void OnStateChange(Object? _, StateOutput e)
    {
        if (e.Video?.LikeStatus is null or ELikeStatus.Unknown)
        {
            return;
        }

        this._liked = e.Video.LikeStatus == ELikeStatus.Like;
        this.SetCurrentState(this._liked ? 1 : 0);
        this.ActionImageChanged();
    }

    protected override async void RunCommand(String actionParameter)
    {
        try
        {
            await (Connector.RestClient?.ToggleLike() ?? Task.CompletedTask);
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

    protected override BitmapImage GetCommandImage(String actionParameter, Int32 state, PluginImageSize imageSize) =>
        DrawingHelper.LoadBitmapImage($"like-{(state == 1 ? "on" : "off")}");
}