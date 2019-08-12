using System;
using System.IO;
using Xunit;

namespace Sdcb.Imaging.Test
{
    public class WatermarkToolTest
    {
        [Fact]
        public void Test1()
        {
            using (var imageStream = File.OpenRead(@"..\..\..\..\sample-images\src.png"))
            using (var outputStream = File.OpenWrite(@"..\..\..\..\sample-images\watermark-test1.png"))
            {
                WatermarkTool.WatermarkText(
                    imageStream, outputStream,
                    watermark: "Watermark here",
                    ImageFormats.Png, 
                    font: "Times New Roman",
                    fontSize: 30,
                    colorARGB: 0x7f_FF_FF_FF); // transparent white
            }
        }

        [Fact]
        public void Test2()
        {
            var bytes = WatermarkTool.WatermarkText(
                imageStream: File.OpenRead(@"..\..\..\..\sample-images\src.png"), 
                watermark:  "Watermark here",
                srcImageFormat: ImageFormats.Png,
                font:       "Times New Roman",
                fontSize:    30,
                colorARGB:   0x7f_FF_FF_FF); // transparent white
            File.WriteAllBytes(@"..\..\..\..\sample-images\watermark-test2.png", bytes);
        }

        [Fact]
        public void JpegTest()
        {
            var bytes = WatermarkTool.WatermarkText(
                imageStream: File.OpenRead(@"..\..\..\..\sample-images\src.jpg"),
                watermark: "Watermark here",
                srcImageFormat: ImageFormats.Jpeg,
                font: "Times New Roman",
                fontSize: 30,
                colorARGB: 0x7f_FF_FF_FF); // transparent white
            File.WriteAllBytes(@"..\..\..\..\sample-images\watermark-dest.jpg", bytes);
        }
    }
}
