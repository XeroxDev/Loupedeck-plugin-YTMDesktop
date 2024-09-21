namespace Loupedeck.YTMDesktopPlugin.Actions;

using Helpers;

public class OpenSettingsCommand : ActionEditorCommand
{
    public OpenSettingsCommand()
    {
        this.DisplayName = "Settings Profile";
        this.GroupName = "General";
        this.Name = "Settings";

        this.ActionEditor.AddControlEx(new ActionEditorTextbox("host", "Host", "The host of the YTMDesktop Companion").SetRequired());
        this.ActionEditor.AddControlEx(new ActionEditorTextbox("port", "Port", "The port of the YTMDesktop Companion").SetRequired()
            .SetFormat(ActionEditorTextboxFormat.Integer));
        
    }

    protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
    {
        var hostSuccess = actionParameters.TryGetString("host", out var host);
        var portSuccess = actionParameters.TryGetInt32("port", out var port);

        if (hostSuccess)
        {
            this.Plugin.SetPluginSetting("host", host);
        }

        if (portSuccess)
        {
            this.Plugin.SetPluginSetting("port", port.ToString());
        }

        _ = Connector.Init(this.Plugin, true);

        return true;
    }
}