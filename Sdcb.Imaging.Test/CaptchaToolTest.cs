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
            File.WriteAllBytes(
                @"D:\_\Captcha.png", 
                CaptchaTool.CreateTextCaptchaPngImage(200, 100, "Hello"));
        }
    }
}
