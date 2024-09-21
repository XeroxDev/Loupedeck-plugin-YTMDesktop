namespace Loupedeck.YTMDesktopPlugin.Actions;

using Helpers;

using XeroxDev.YTMDesktop.Companion.Enums;
using XeroxDev.YTMDesktop.Companion.Exceptions;
using XeroxDev.YTMDesktop.Companion.Models.Output;

public class RepeatCommand : PluginMultistateDynamicCommand
{
    private enum RepeatType
    {
        // ReSharper disable once UnusedMember.Local
        None,

        // ReSharper disable once UnusedMember.Local
        All,

        // ReSharper disable once UnusedMember.Local
        One
    }

    private ERepeatMode _currentMode;

    public RepeatCommand() : base("Repeat", "Toggles repeat types", "Player")
    {
        foreach (var state in Enum.GetValues(typeof(RepeatType)))
        {
            this.AddState(state.ToString(), state.ToString(), $"If current repeat type is {state}");
        }
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

    private void OnStateChange(Object? _, StateOutput e)
    {
        if (e.Player?.Queue?.RepeatMode is null or ERepeatMode.Unknown)
        {
            return;
        }
        
        this._currentMode = e.Player.Queue.RepeatMode;

        this.SetCurrentState((Int32)this._currentMode);
        this.ActionImageChanged();
    }

    protected override async void RunCommand(String actionParameter)
    {
        ERepeatMode newMode;
        switch (this._currentMode)
        {
            case ERepeatMode.None:
                newMode = ERepeatMode.All;
                break;
            case ERepeatMode.All:
                newMode = ERepeatMode.One;
                break;
            case ERepeatMode.One:
            case ERepeatMode.Unknown:
            default:
                newMode = ERepeatMode.None;
                break;
        }

        try
        {
            await (Connector.RestClient?.SetRepeatMode(newMode) ?? Task.CompletedTask);
            this.Plugin.OnPluginStatusChanged(PluginStatus.Normal, "");
            this.SetCurrentState((Int32)newMode);
            this._currentMode = newMode;
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
        DrawingHelper.LoadBitmapImage("repeat", Enum.GetName(typeof(RepeatType), state));
}