namespace Loupedeck.YTMDesktopPlugin.Actions;

using Helpers;

using XeroxDev.YTMDesktop.Companion.Exceptions;
using XeroxDev.YTMDesktop.Companion.Models.Output;

public class VolumeAdjustment : PluginDynamicAdjustment
{
    private Int32 _currentVolume;
    private Int32 _lastVolume;
    private readonly Debouncer _debouncer;
    private Boolean _changingVolume;

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
        if (this._changingVolume)
        {
            return;
        }

        this._currentVolume = Convert.ToInt32(Math.Floor(e.Player?.Volume ?? 0));
        this.AdjustmentValueChanged();
    }

    public VolumeAdjustment() : base("Change volume", "Changes player volume", "Player", true)
        => this._debouncer = new Debouncer(TimeSpan.FromMilliseconds(250), this.SetVolume);

    protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => "Toggle volume";

    protected override async void RunCommand(String actionParameter)
    {
        if (this._currentVolume <= 0)
        {
            this._currentVolume = this._lastVolume;

            try
            {
                await (Connector.RestClient?.SetVolume(this._currentVolume) ?? Task.CompletedTask);
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

            return;
        }

        this._lastVolume = this._currentVolume;
        this._currentVolume = 0;

        try
        {
            await (Connector.RestClient?.SetVolume(this._currentVolume) ?? Task.CompletedTask);
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

    protected override void ApplyAdjustment(String actionParameter, Int32 diff)
    {
        this._changingVolume = true;
        var vol = this._currentVolume;
        vol += diff;

        vol = vol <= 0 ? 0 : vol >= 100 ? 100 : vol;

        if (vol == this._currentVolume)
        {
            return;
        }

        this._currentVolume = vol;
        this._lastVolume = vol;

        this._debouncer.Invoke();
    }

    private void SetVolume()
    {
        try
        {
            Connector.RestClient?.SetVolume(this._currentVolume).Wait();
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

        this._changingVolume = false;
    }

    protected override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize)
    {
        try
        {
            return DrawingHelper.DrawVolumeBar(imageSize, new BitmapColor(156, 156, 156), this._changingVolume ? new BitmapColor(255, 0, 0) : new BitmapColor(0, 255, 0), this._currentVolume);
        }
        catch (Exception)
        {
            return base.GetAdjustmentImage(actionParameter, imageSize);
        }
    }
}