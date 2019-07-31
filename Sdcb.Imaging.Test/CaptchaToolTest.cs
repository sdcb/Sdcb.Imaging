using System;
using System.IO;
using Xunit;

namespace Sdcb.Imaging.Test
{
    public class CaptchaToolTest
    {
        [Fact]
        public void Test1()
        {
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
        }
    }
}
