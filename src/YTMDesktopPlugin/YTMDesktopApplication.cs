namespace Loupedeck.YTMDesktopPlugin;

// This class can be used to connect the Loupedeck plugin to an application.

public class YTMDesktopApplication : ClientApplication
{
    protected override Boolean IsProcessNameSupported(String processName) => processName.ContainsNoCase("YouTube Music Desktop App");

    // This method can be used to link the plugin to a Windows application.
    protected override String GetProcessName() => "YouTube Music Desktop App";

    // This method can be used to check whether the application is installed or not.
    public override ClientApplicationStatus GetApplicationStatus()
    {
        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(localAppData, "youtube_music_desktop_app", "youtube-music-desktop-app.exe");
            return File.Exists(path) ? ClientApplicationStatus.Installed : ClientApplicationStatus.NotInstalled;
        }
        catch (Exception)
        {
            return ClientApplicationStatus.NotInstalled;
        }
    }
}