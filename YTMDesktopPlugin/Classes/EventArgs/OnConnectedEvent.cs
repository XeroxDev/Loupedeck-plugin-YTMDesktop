namespace Loupedeck.YTMDesktopPlugin.Classes.EventArgs
{
    using System;

    using H.Socket.IO.EventsArgs;

    public sealed class OnConnectedEvent
    {
        public Object Sender { get; }
        public SocketIoEventEventArgs EventArgs { get; }

        public OnConnectedEvent(Object sender, SocketIoEventEventArgs eventArgs)
        {
            this.Sender = sender;
            this.EventArgs = eventArgs;
        }
    }
}