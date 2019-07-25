using System.IO;
using WIC = SharpDX.WIC;
using D2D = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;
using SharpDX;

namespace Sdcb.Imaging
{
    public static class WatermarkTool
    {
        public const int TransparentWhite = 0x7FFFFFFF;

        public static void WatermarkText(Stream imageStream, Stream outputStream, string watermark, string font = "微软雅黑", float fontSize = 30.0f, int colorARGB = TransparentWhite)
        {
            using (var wic = new WIC.ImagingFactory2())
            using (var d2d = new D2D.Factory())
            using (var image = CreateWicImage(wic, imageStream))
            using (var wicBitmap = new WIC.Bitmap(wic, image.Size.Width, image.Size.Height, WIC.PixelFormat.Format32bppPBGRA, WIC.BitmapCreateCacheOption.CacheOnDemand))
            using (var target = new D2D.WicRenderTarget(d2d, wicBitmap, new D2D.RenderTargetProperties()))
            using (var bmpPicture = D2D.Bitmap.FromWicBitmap(target, image))
            using (var dwriteFactory = new DWrite.Factory())
            using (var brush = new D2D.SolidColorBrush(target, new Color(colorARGB)))
            {
                target.BeginDraw();
                {
                    target.DrawBitmap(bmpPicture, new RectangleF(0, 0, target.Size.Width, target.Size.Height), 1.0f, D2D.BitmapInterpolationMode.Linear);
                    target.DrawRectangle(new RectangleF(0, 0, target.Size.Width, target.Size.Height), brush);
                    var textFormat = new DWrite.TextFormat(dwriteFactory, font, DWrite.FontWeight.Bold, DWrite.FontStyle.Normal, fontSize)
                    {
                        ParagraphAlignment = DWrite.ParagraphAlignment.Far,
                        TextAlignment = DWrite.TextAlignment.Trailing,
                    };
                    target.DrawText(watermark, textFormat, new RectangleF(0, 0, target.Size.Width, target.Size.Height), brush);
                }
                target.EndDraw();

                SaveD2DBitmap(wic, wicBitmap, outputStream);
            }
        }

        public static MemoryStream WatermarkText(Stream imageStream, string watermark, string font = "微软雅黑", float fontSize = 30.0f, int colorARGB = TransparentWhite)
        {
            var ms = new MemoryStream();
            WatermarkText(imageStream, ms, watermark, font, fontSize, colorARGB);
            return ms;
        }

        private static void SaveD2DBitmap(WIC.ImagingFactory2 wic, WIC.Bitmap wicBitmap, Stream outputStream)
        {
            using (var encoder = new WIC.BitmapEncoder(wic, WIC.ContainerFormatGuids.Png))
            {
                encoder.Initialize(outputStream);
                using (var frame = new WIC.BitmapFrameEncode(encoder))
                {
                    frame.Initialize();
                    frame.SetSize(wicBitmap.Size.Width, wicBitmap.Size.Height);

                    var pixelFormat = wicBitmap.PixelFormat;
                    frame.SetPixelFormat(ref pixelFormat);
                    frame.WriteSource(wicBitmap);

                    frame.Commit();
                    encoder.Commit();
                }
            }
        }

        private static WIC.FormatConverter CreateWicImage(WIC.ImagingFactory2 wic, Stream imageStream)
        {
            using (var decoder = new WIC.PngBitmapDecoder(wic))
            {
                var decodeStream = new WIC.WICStream(wic, imageStream);
                decoder.Initialize(decodeStream, WIC.DecodeOptions.CacheOnLoad);
                using (var decodeFrame = decoder.GetFrame(0))
                {
                    var converter = new WIC.FormatConverter(wic);
                    converter.Initialize(decodeFrame, WIC.PixelFormat.Format32bppPBGRA);
                    return converter;
                }
            }
        }
    }
}
