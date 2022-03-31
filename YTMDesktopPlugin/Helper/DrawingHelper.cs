namespace Loupedeck.YTMDesktopPlugin.Helper
{
    using System;

    public static class DrawingHelper
    {
        private static String RESOURCE_PATH = "Loupedeck.YTMDesktopPlugin.Resources";

        public static BitmapImage ReadImage(String imageName, String ext = "png", String addPath = "Images")
            => EmbeddedResources.ReadImage($"{RESOURCE_PATH}.{addPath}.{imageName}.{ext}");

        public static BitmapBuilder LoadBitmapBuilder
        (String imageName = "clear", String text = null, BitmapColor? textColor = null, String ext = "png",
            String addPath = "Images")
            => LoadBitmapBuilder(ReadImage(imageName, ext, addPath), text, textColor);

        public static BitmapBuilder LoadBitmapBuilder
            (BitmapImage image, String text = null, BitmapColor? textColor = null)
        {
            var builder = new BitmapBuilder(90, 90);

            builder.Clear(BitmapColor.Black);
            builder.DrawImage(image);
            builder.FillRectangle(0, 0, 90, 90, new BitmapColor(0, 0, 0, 150));

            return text is null ? builder : builder.AddTextOutlined(text, textColor: textColor);
        }

        public static BitmapImage LoadBitmapImage
        (String imageName = "clear", String text = null, BitmapColor? textColor = null, String ext = "png",
            String addPath = "Images")
            => LoadBitmapBuilder(imageName, text, textColor, ext, addPath).ToImage();

        public static BitmapImage LoadBitmapImage(BitmapImage image, String text = null, BitmapColor? textColor = null)
            => LoadBitmapBuilder(image, text, textColor).ToImage();

        public static BitmapBuilder AddTextOutlined(this BitmapBuilder builder, String text,
            BitmapColor? outlineColor = null,
            BitmapColor? textColor = null, Int32 fontSize = 15)
        {
            // Make it outline
            // builder.DrawText(text, outlineColor ?? BitmapColor.Black, fontSize + 2);
            builder.DrawText(text, textColor, fontSize);
            return builder;
        }

        public static BitmapImage DrawVolumeBar(PluginImageSize imageSize, BitmapColor backgroundColor, BitmapColor foregroundColor, Int32 percentage)
        {
            var dim = imageSize.GetDimension();
            var width = (Int32)(dim * percentage / 100.0);

            var builder = new BitmapBuilder(dim, dim);
            builder.Clear(BitmapColor.Black);

            builder.Translate(dim / 4, 0);
            builder.DrawRectangle(0, 0, dim / 2, dim - 1, backgroundColor);
            builder.FillRectangle(0, dim, dim / 2, -width, backgroundColor);
            builder.FillRectangle(0, 0, dim / 2, dim - 1, new BitmapColor(0, 0, 0, 150));
            builder.ResetMatrix();
            builder.DrawText($"{percentage}%", foregroundColor);
            return builder.ToImage();
        }

        public static Int32 GetDimension(this PluginImageSize size) => size == PluginImageSize.Width60 ? 50 : 80;
    }
}