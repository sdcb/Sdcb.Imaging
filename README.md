# Sdcb.Imaging
Direct2D based watermark/captcha tool.

# Watermark usage
```csharp
var ms = WatermarkTool.WatermarkText(File.OpenRead(@"D:\_\WatermarkDemo.png"), "水印在此",
    font: "微软雅黑",
    fontSize: 30,
    colorARGB: 0x7f_FF_FF_FF);
using (var file = File.OpenWrite(@"D:\_\Demo2.png"))
{
    ms.CopyTo(file);
}
```
