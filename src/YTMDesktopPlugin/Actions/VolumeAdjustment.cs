namespace Loupedeck.YTMDesktopPlugin.Actions
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Services;

    using Utils;


    public class VolumeAdjustment : PluginDynamicAdjustment
    {
        public static BehaviorSubject<Int32> OnVolume { get; } = new BehaviorSubject<Int32>(50);
        public static Int32 LastVolume { get; set; } = 50;
        private Boolean IgnoreServer { get; set; }
        private Subject<Boolean> OnDestroy { get; } = new Subject<Boolean>();
        private SocketService SocketService { get; }


        protected override Boolean OnLoad()
        {
            this.SocketService
                .OnTick
                .DistinctUntilChanged()
                .TakeUntil(this.OnDestroy)
                .Subscribe(response =>
                {
                    if (this.IgnoreServer)
                    {
                        this.IgnoreServer = false;
                        return;
                    }

                    OnVolume.OnNext(Convert.ToInt32(response.Player.VolumePercent));
                });

            OnVolume
                .DistinctUntilChanged()
                .TakeUntil(this.OnDestroy)
                .Subscribe(vol =>
                {
                    LastVolume = vol <= 0 ? LastVolume : vol;
                    this.AdjustmentValueChanged();
                });
            return base.OnLoad();
        }

        protected override Boolean OnUnload()
        {
            this.OnDestroy.OnNext(true);
            return base.OnUnload();
        }

        public VolumeAdjustment() : base("Change volume", "Changes player volume", "Player", true) =>
            this.SocketService = SocketService.Instance;

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => "Toggle volume";

        protected override async void RunCommand(String actionParameter)
        {
            var vol = OnVolume.Value == 0
                ? LastVolume
                : -1;
            OnVolume.OnNext(vol);
            await this.SocketService.PlayerSetVolume(vol);
        }

        protected override async void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            var vol = OnVolume.Value;
            vol += diff;
            
            vol = vol <= 0 ? -1 : vol >= 100 ? 100 : vol;

            if (vol == LastVolume)
            {
                return;
            }

            LastVolume = vol;
            OnVolume.OnNext(vol);
            this.IgnoreServer = true;
            await this.SocketService.PlayerSetVolume(vol);
        }

        protected override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize)
        {
            try
            {
                return DrawingHelper.DrawVolumeBar(imageSize, new BitmapColor(156, 156, 156), BitmapColor.White, OnVolume.Value);
            }
            catch (Exception)
            {
                return base.GetAdjustmentImage(actionParameter, imageSize);
            }
        }
    }
}