namespace Loupedeck.YTMDesktopPlugin.Commands
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Runtime.InteropServices;

    using Extensions;

    using Helper;

    using Services;

    using static Helper.DrawingHelper;


    public class VolumeAdjustment : PluginDynamicAdjustment
    {
        public static BehaviorSubject<Int32> OnVolume { get; } = new(50);
        public static Int32 LastVolume { get; set; } = 50;
        private Boolean IgnoreServer { get; set; }
        private Subject<Boolean> OnDestroy { get; } = new();
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

        public VolumeAdjustment() : base("Change volume", "Changes player volume", "Player", false) =>
            this.SocketService = SocketService.Instance;

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => "Volume";

        protected override async void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            var vol = OnVolume.Value;
            vol += diff;

            vol = vol switch
            {
                < 0 => -1,
                > 100 => 100,
                _ => vol
            };

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
            var bitmap = new Bitmap(70, 20);
            var g = Graphics.FromImage(bitmap);

            var percentage = OnVolume.Value;
            var bgColor = Color.FromArgb(156, 156, 156);
            var textColor = Color.White;
            var rect = new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1);
            var font = new Font("Arial", 20, FontStyle.Bold);
            var brush = new SolidBrush(Color.White);
            var width = (Int32)(rect.Width * percentage / 100.0);
            var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            g.DrawRectangle(new Pen(bgColor), rect);
            g.FillRectangle(new SolidBrush(bgColor), 0, 0, width, rect.Height);
            g.FillRectangle(new SolidBrush(Color.FromArgb(150, 0, 0, 0)), 0, 0, bitmap.Width, bitmap.Height);
            g.DrawAutoAdjustedFont($"{percentage}%", font, brush, rect, sf, 12);

            bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);

            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            return new BitmapImage(ms.ToArray());
        }
    }
}