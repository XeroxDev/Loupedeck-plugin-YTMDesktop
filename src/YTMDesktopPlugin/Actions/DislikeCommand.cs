namespace Loupedeck.YTMDesktopPlugin.Actions;

using Helpers;

using XeroxDev.YTMDesktop.Companion.Enums;
using XeroxDev.YTMDesktop.Companion.Exceptions;
using XeroxDev.YTMDesktop.Companion.Models.Output;

public class DislikeCommand : PluginMultistateDynamicCommand
{
    private Boolean _disliked;

    public DislikeCommand() : base("Dislike", "Dislikes track", "Track")
    {
        this.AddState("Neutral", "If current song is not disliked");
        this.AddState("Disliked", "If current song is disliked");
    }

    protected override Boolean OnLoad()
    {
        Connector.OnStateChange += this.OnStateChange;
        return base.OnLoad();
    }

    protected override Boolean OnUnload()
    {
        Connector.OnStateChange -= this.OnStateChange;
        return base.OnUnload();
    }

    private void OnStateChange(Object? sender, StateOutput e)
    {
        if (e.Video?.LikeStatus is null or ELikeStatus.Unknown)
        {
            return;
        }

        this._disliked = e.Video.LikeStatus == ELikeStatus.Dislike;
        this.SetCurrentState(this._disliked ? 1 : 0);
        this.ActionImageChanged();
    }

    protected override async void RunCommand(String actionParameter)
    {
        try
        {
            await (Connector.RestClient?.ToggleDislike() ?? Task.CompletedTask);
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
        DrawingHelper.LoadBitmapImage($"dislike-{(state == 1 ? "on" : "off")}");
}