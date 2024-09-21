namespace Loupedeck.YTMDesktopPlugin.Actions;

using Helpers;

using XeroxDev.YTMDesktop.Companion.Enums;
using XeroxDev.YTMDesktop.Companion.Exceptions;
using XeroxDev.YTMDesktop.Companion.Models.Output;

public class PlayPauseCommand : ActionEditorCommand
{
    private Boolean _playing = true;
    private Int32 _current;
    private Int32 _duration;
    private Int32 _remaining;
    private const String TextName = "playPauseFormat";
    private const String TextLabel = "Format Line";
    private const String TextDescription = "Available variables: {current}, {current:S}, {duration}, {duration:S}, {remaining}, {remaining:S}";


    public PlayPauseCommand()
    {
        this.DisplayName = "Play/Pause";
        this.SuperGroupName = "Track";
        this.GroupName = "Track";
        
        this.Description = "Pauses or resumes music track";

        this.ActionEditor.AddControlEx(new ActionEditorTextbox($"{TextName}1", $"{TextLabel} 1", TextDescription));
        this.ActionEditor.AddControlEx(new ActionEditorTextbox($"{TextName}2", $"{TextLabel} 2", TextDescription));
        this.ActionEditor.AddControlEx(new ActionEditorTextbox($"{TextName}3", $"{TextLabel} 3", TextDescription));
    }

    protected override Boolean OnLoad()
    {
        Connector.OnStateChange += this.OnStateChange;
        return base.OnLoad();
    }

    private void OnStateChange(Object? _, StateOutput e)
    {
        this._current = Convert.ToInt32(Math.Floor(e.Player?.VideoProgress ?? 0.0));
        this._duration = Convert.ToInt32(Math.Floor(e.Video?.DurationSeconds ?? 0.0));
        this._remaining = this._duration - this._current;
        this._playing = (e.Player?.TrackState ?? ETrackState.Unknown) == ETrackState.Playing;
        this.ActionImageChanged();
    }

    protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
    {
        try
        {
            Connector.RestClient?.PlayPause()?.Wait();
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

        this.ActionImageChanged();
        return true;
    }

    protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth, Int32 imageHeight)
    {
        String? format = null;
        var line1Success = actionParameters.TryGetString($"{TextName}1", out var line1);
        var line2Success = actionParameters.TryGetString($"{TextName}2", out var line2);
        var line3Success = actionParameters.TryGetString($"{TextName}3", out var line3);

        if (line1Success && !String.IsNullOrWhiteSpace(line1))
        {
            format = line1;
        }
        
        if (line2Success && !String.IsNullOrWhiteSpace(line2))
        {
            format = String.IsNullOrWhiteSpace(format) ? line2 : $"{format}\n{line2}";
        }
        
        if (line3Success && !String.IsNullOrWhiteSpace(line3))
        {
            format = String.IsNullOrWhiteSpace(format) ? line3 : $"{format}\n{line3}";
        }
        
        
        String text;
        String imageName;
        if (Connector.ConnectionState == ESocketState.Connected)
        {
            if (this._playing)
            {
                text = FormatTitle(format ?? "{current}", this._current, this._duration, this._remaining);
                imageName = "music-pause";
            }
            else
            {
                text = "Paused";
                imageName = "music-play";
            }
        }
        else
        {
            text = "Not connected";
            imageName = "music-play";
        }

        return DrawingHelper.LoadBitmapImage(imageName, text);
    }


    public static String FormatTitle(String format, Int32 current = 0, Int32 duration = 0, Int32 remaining = 0)
    {
        IDictionary<String, String> varMapping = new Dictionary<String, String>
        {
            { "current", FormatTime(current) },
            { "current:S", current.ToString() },
            { "duration", FormatTime(duration) },
            { "duration:S", duration.ToString() },
            { "remaining", FormatTime(remaining) },
            { "remaining:S", remaining.ToString() }
        };

        return varMapping.Aggregate(format, (current1, kv) => current1.Replace($"{{{kv.Key}}}", kv.Value));
    }

    private static String FormatTime(Int32 seconds)
    {
        var minutes = seconds / 60;
        var secondsLeft = seconds % 60;
        var hours = minutes / 60;
        var minutesLeft = minutes % 60;

        var result = "";

        if (hours > 0)
        {
            result += $"{hours:D2}:";
        }

        result += $"{minutesLeft:D2}:{secondsLeft:D2}";

        return result;
    }
}