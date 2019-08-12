using System.IO;
using WIC = SharpDX.WIC;
using D2D = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;
using SharpDX;
using System;
using System.Collections.Generic;

namespace Sdcb.Imaging
{
    public static class WatermarkTool
    {
        public const int TransparentWhite = 0x7FFFFFFF;

        public static void WatermarkText(Stream imageStream, Stream outputStream, string watermark,
            ImageFormats srcImageFormat, ImageFormats? destImageFormat = null, string font = "Times New Roman", float fontSize = 30.0f, int colorARGB = TransparentWhite)
        {
            Guid srcImageFormatGuid = ImageFormatGuidMapping[srcImageFormat];
            Guid destImageFormatGuid = ImageFormatGuidMapping[destImageFormat ?? srcImageFormat];
            using (var wic = new WIC.ImagingFactory2())
            using (var d2d = new D2D.Factory())
            using (var image = CreateWicImage(wic, imageStream, srcImageFormatGuid))
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

                SaveD2DBitmap(wic, wicBitmap, outputStream, destImageFormatGuid);
            }
        }

        public static byte[] WatermarkText(Stream imageStream, string watermark, ImageFormats srcImageFormat, ImageFormats? destImageFormat = null,
            string font = "Times New Roman", float fontSize = 30.0f, int colorARGB = TransparentWhite)
        {
            var ms = new MemoryStream();
            WatermarkText(imageStream, ms, watermark, srcImageFormat, destImageFormat, font, fontSize, colorARGB);
            return ms.ToArray();
        }

        private static void SaveD2DBitmap(WIC.ImagingFactory2 wic, WIC.Bitmap wicBitmap, Stream outputStream, Guid imageFormatGuid)
        {
            using (var encoder = new WIC.BitmapEncoder(wic, imageFormatGuid))
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

        private static WIC.FormatConverter CreateWicImage(WIC.ImagingFactory2 wic, Stream imageStream, Guid imageFormatGuid)
        {
            using (var decoder = new WIC.BitmapDecoder(wic, imageFormatGuid))
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

        public static Dictionary<ImageFormats, Guid> ImageFormatGuidMapping = new Dictionary<ImageFormats, Guid>
        {
            [ImageFormats.Bmp] = WIC.ContainerFormatGuids.Bmp,
            [ImageFormats.Png] = WIC.ContainerFormatGuids.Png,
            [ImageFormats.Ico] = WIC.ContainerFormatGuids.Ico,
            [ImageFormats.Jpeg] = WIC.ContainerFormatGuids.Jpeg,
            [ImageFormats.Tiff] = WIC.ContainerFormatGuids.Tiff,
            [ImageFormats.Gif] = WIC.ContainerFormatGuids.Gif,
            [ImageFormats.Wmp] = WIC.ContainerFormatGuids.Wmp,
            [ImageFormats.Dds] = WIC.ContainerFormatGuids.Dds,
            [ImageFormats.Adng] = WIC.ContainerFormatGuids.Adng,
        };
    }

    public enum ImageFormats
    {
        Bmp,
        Png,
        Ico,
        Jpeg,
        Tiff,
        Gif,
        Wmp,
        Dds,
        Adng,
    }
}
