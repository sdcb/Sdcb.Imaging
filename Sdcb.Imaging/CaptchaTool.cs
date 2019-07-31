using System.IO;
using WIC = SharpDX.WIC;
using D2D = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;
using SharpDX;
using System;

namespace Sdcb.Imaging
{
    public static class CaptchaTool
    {
        public static byte[] CreatePngImage(int width, int height, string text, 
            float fontSize = 30.0f, 
            string font = "Times New Roman", 
            int lineCount = 5, 
            bool rotation = false, 
            float turbulenceAmount = 60.0f)
        {
            using (var wic = new WIC.ImagingFactory2())
            using (var d2d = new D2D.Factory())
            using (var wicBitmap = new WIC.Bitmap(wic, width, height, WIC.PixelFormat.Format32bppPBGRA, WIC.BitmapCreateCacheOption.CacheOnDemand))
            using (var target = new D2D.WicRenderTarget(d2d, wicBitmap, new D2D.RenderTargetProperties()))
            using (var dwriteFactory = new DWrite.Factory())
            using (var brush = new D2D.SolidColorBrush(target, Color.Yellow))
            using (var encoder = new WIC.PngBitmapEncoder(wic))

            using (var ms = new MemoryStream())
            using (var dc = target.QueryInterface<D2D.DeviceContext>())
            using (var bmpLayer = new D2D.Bitmap1(dc, target.PixelSize,
                new D2D.BitmapProperties1(new D2D.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, D2D.AlphaMode.Premultiplied),
                d2d.DesktopDpi.Width, d2d.DesktopDpi.Height,
                D2D.BitmapOptions.Target)))
            {
                var r = new Random();
                encoder.Initialize(ms);

                D2D.Image oldTarget = dc.Target;

                {
                    dc.Target = bmpLayer;
                    dc.BeginDraw();
                    var textFormat = new DWrite.TextFormat(dwriteFactory, font, fontSize);
                    for (int charIndex = 0; charIndex < text.Length; ++charIndex)
                    {
                        using (var layout = new DWrite.TextLayout(dwriteFactory, text[charIndex].ToString(), textFormat, float.MaxValue, float.MaxValue))
                        {
                            var layoutSize = new Vector2(layout.Metrics.Width, layout.Metrics.Height);
                            using (var b2 = new D2D.LinearGradientBrush(dc, new D2D.LinearGradientBrushProperties
                            {
                                StartPoint = Vector2.Zero,
                                EndPoint = layoutSize,
                            }, new D2D.GradientStopCollection(dc, new[]
                            {
                                new D2D.GradientStop{ Position = 0.0f, Color = ColorFromHsl(r.NextFloat(0, 1), 1.0f, 0.8f) },
                                new D2D.GradientStop{ Position = 1.0f, Color = ColorFromHsl(r.NextFloat(0, 1), 1.0f, 0.8f) },
                            })))
                            {
                                var position = new Vector2(charIndex * width / text.Length, r.NextFloat(0, height - layout.Metrics.Height));
                                dc.Transform =
                                    Matrix3x2.Translation(-layoutSize / 2) *
                                    Matrix3x2.Skew(r.NextFloat(0, 0.5f), r.NextFloat(0, 0.5f)) *
                                    (rotation ? Matrix3x2.Rotation(r.NextFloat(0, (float)(Math.PI * 2))) : Matrix3x2.Identity) *
                                    Matrix3x2.Translation(position + layoutSize / 2);
                                dc.DrawTextLayout(Vector2.Zero, layout, b2);
                            }
                        }
                    }
                    for (var i = 0; i < lineCount; ++i)
                    {
                        target.Transform = Matrix3x2.Identity;
                        brush.Color = ColorFromHsl(r.NextFloat(0, 1), 1.0f, 0.3f);
                        target.DrawLine(
                            r.NextVector2(Vector2.Zero, new Vector2(width, height)),
                            r.NextVector2(Vector2.Zero, new Vector2(width, height)),
                            brush, 3.0f);
                    }
                    target.EndDraw();
                }

                Color background = ColorFromHsl(r.NextFloat(0, 1), 1.0f, 0.3f);
                {
                    dc.Target = null;
                    using (var displacement = new D2D.Effects.DisplacementMap(dc))
                    {
                        displacement.SetInput(0, bmpLayer, true);
                        displacement.Scale = turbulenceAmount;

                        var turbulence = new D2D.Effects.Turbulence(dc);
                        displacement.SetInputEffect(1, turbulence);

                        dc.Target = oldTarget;
                        dc.BeginDraw();
                        dc.Clear(background);
                        dc.DrawImage(displacement);
                        dc.EndDraw();

                        using (var frame = new WIC.BitmapFrameEncode(encoder))
                        {
                            frame.Initialize();
                            frame.SetSize(wicBitmap.Size.Width, wicBitmap.Size.Height);

                            var pixelFormat = wicBitmap.PixelFormat;
                            frame.SetPixelFormat(ref pixelFormat);
                            frame.WriteSource(wicBitmap);

                            frame.Commit();
                        }
                    }
                }

                encoder.Commit();
                return ms.ToArray();
            }
        }

        private static Color ColorFromHsl(float h, float s, float l)
        {
            if (h > 1.0f)
            {
                h = h / 360.0f;
            }
            double r = 0, g = 0, b = 0;
            if (l == 0)
            {
                r = g = b = 0;
            }
            else
            {
                if (s == 0)
                {
                    r = g = b = l;
                }
                else
                {
                    var temp2 = ((l <= 0.5) ? l * (1.0 + s) : l + s - (l * s));
                    var temp1 = (2.0 * l) - temp2;
                    var t3 = new double[] { h + (1.0 / 3.0), h, h - (1.0 / 3.0) };
                    var clr = new double[] { 0, 0, 0 };
                    for (var i = 0; i < 3; i++)
                    {
                        if (t3[i] < 0)
                        {
                            t3[i] += 1.0;
                        }
                        if (t3[i] > 1)
                        {
                            t3[i] -= 1.0;
                        }
                        clr[i] = 6.0 * t3[i] < 1.0
                            ? temp1 + ((temp2 - temp1) * t3[i] * 6.0)
                            : 2.0 * t3[i] < 1.0 ? temp2 : 3.0 * t3[i] < 2.0
                            ? temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0) : temp1;
                    }
                    r = clr[0];
                    g = clr[1];
                    b = clr[2];
                }
            }
            return new Color((byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
        }
    }
}
