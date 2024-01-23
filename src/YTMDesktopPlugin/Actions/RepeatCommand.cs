namespace Loupedeck.YTMDesktopPlugin.Actions
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Services;

    using Utils;

    public class RepeatCommand : PluginMultistateDynamicCommand
    {
        private enum RepeatType
        {
            None,
            All,
            One
        }

        private SocketService SocketService { get; }
        private Subject<Boolean> OnDestroy { get; } = new Subject<Boolean>();

        public RepeatCommand() : base("Repeat", "Toggles repeat types", "Player")
        {
            foreach (var state in Enum.GetValues(typeof(RepeatType)))
            {
                this.AddState(state.ToString(), $"If current repeat type is {state}");
            }

            this.SocketService = SocketService.Instance;
        }

        protected override Boolean OnLoad()
        {
            this.SocketService.OnTick
                .Select(response => response.Player.RepeatType)
                .DistinctUntilChanged()
                .TakeUntil(this.OnDestroy)
                .Subscribe(repeatType =>
                {
                    this.SetCurrentState((Int32)Enum.Parse(typeof(RepeatType), repeatType, true));
                    this.ActionImageChanged();
                });

            return base.OnLoad();
        }

        protected override Boolean OnUnload()
        {
            this.OnDestroy.OnNext(true);
            return base.OnUnload();
        }

        protected override async void RunCommand(String actionParameter) => await this.SocketService.PlayerRepeat();

        protected override BitmapImage GetCommandImage(String actionParameter, Int32 state, PluginImageSize imageSize) =>
            DrawingHelper.LoadBitmapImage("repeat", Enum.GetName(typeof(RepeatType), state));
    }
}