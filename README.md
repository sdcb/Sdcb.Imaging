# Sdcb.Imaging
Direct2D based watermark/captcha tool.

# Watermark usage
```csharp
using (var imageStream = File.OpenRead(@"..\..\..\..\sample-images\src.png"))
using (var outputStream = File.OpenWrite(@"..\..\..\..\sample-images\watermark-test1.png"))
{
    WatermarkTool.WatermarkText(
        imageStream, outputStream,
        watermark: "Watermark here",
        font: "Times New Roman",
        fontSize: 30,
        colorARGB: 0x7f_FF_FF_FF); // transparent white
}
```

Origin image: 

![src.png](./sample-images/src.png)

Watermarked image:

![watermark-test1.png](./sample-images/watermark-test1.png)

# Captcha usage
```csharp
byte[] pngBytes = CaptchaTool.CreatePngImage(
    width: 200, height: 100, 
    fontSize: 50.0f, 
    text: "CAPTCHA", 
    font: "Times New Roman", 
    lineCount: 5, 
    rotation: false, 
    turbulenceAmount: 60.0f);
File.WriteAllBytes(
    @"..\..\..\..\sample-images\captcha-test1.png", pngBytes);
```

Generated example:

![captcha-test1.png](./sample-images/captcha-test1.png)
