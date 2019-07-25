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
            WatermarkTool.WatermarkText(File.OpenRead(@"D:\_\WatermarkDemo.png"), File.OpenWrite(@"D:\_\Demo2.png"), "水印在此");
        }

        [Fact]
        public void Test2()
        {
            var ms = WatermarkTool.WatermarkText(File.OpenRead(@"D:\_\WatermarkDemo.png"), "水印在此");
            using (var file = File.OpenWrite(@"D:\_\Demo2.png"))
            {
                ms.CopyTo(file);
            }
        }
    }
}
