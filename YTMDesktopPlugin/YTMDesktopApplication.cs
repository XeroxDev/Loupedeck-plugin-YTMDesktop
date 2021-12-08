namespace Loupedeck.YTMDesktopPlugin
{
    using System;

    using Services;

    public class YTMDesktopApplication : ClientApplication
    {
        public YTMDesktopApplication() => SocketService.Instance.StartConnection().ConfigureAwait(true);

        protected override Boolean IsProcessNameSupported(String processName) =>
            processName.ContainsNoCase("YouTube Music Desktop App");

        protected override String GetBundleName() => "";
    }
}