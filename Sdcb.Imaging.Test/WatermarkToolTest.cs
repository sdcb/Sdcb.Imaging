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
            WatermarkTool.AddWatermark(File.OpenRead(@"D:\_\WatermarkDemo.png"), File.OpenWrite(@"D:\_\Demo2.png"), "Ë®Ó¡ÔÚ´Ë");
        }
    }
}
