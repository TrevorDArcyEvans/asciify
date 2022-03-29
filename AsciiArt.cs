/*======================================================================/

Copyright (C) 2004 Daniel Fisher(lennybacon).  All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

For more information email: info@lennybacon.com

12/27/04 Enhancements by Steven Fowler (steven_m_fowler@yahoo.com)
=======================================================================*/

namespace StaticDust
{
  using SixLabors.ImageSharp;
  using SixLabors.ImageSharp.PixelFormats;

  /// <summary>
  /// Summary description for image2html.
  /// </summary>
  public static class AsciiArt
  {
    // from:
    //    http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color
    private static float RGBtoBrightness(float red, float green, float blue)
    {
      return (red + red + blue + green + green + green) / 6;
    }

    /// <summary>
    /// converts an image to an ascii web page
    /// </summary>
    /// <param name="img">image to convert</param>
    /// <param name="fileStream">opened file to write HTML</param>
    /// <param name="imgBlockSize">
    /// used to determine the width in pixels of one ascii character
    /// which is then used to determine the block size in pixels
    /// of one ascii character
    /// </param>
    /// <param name="fontSize">
    /// fixed width font size in pixels
    /// <remarks>5 pixels seems to be a good value</remarks>
    /// </param>
    /// <param name="quick">
    /// <remarks>
    /// quick brightness calculation is only done on first row of pixels in a block
    /// </remarks>
    /// </param>
    /// <param name="colour">true to generate a colour ascii image</param>
    /// <param name="monitor">
    /// optional callback to monitor progress, <seealso cref="IAsciiArtProgress"/>
    /// <remarks>pass in null to not monitor progress</remarks>
    /// </param>
    /// <returns></returns>
    public static void ConvertImage(
      Image<Rgba32> img, 
      StreamWriter fileStream,
      int imgBlockSize,
      int fontSize,
      bool quick,
      bool colour)
    {
      const string WebPage1 =
        "<html>" +
          "<body>" +
          "<pre>" +
          "<span style=\"font-size: {0}px;font-family: monospace;\">";
      fileStream.Write(WebPage1, fontSize);

#if false
      var buffer = Marshal.AllocHGlobal((int)(img.Width * img.Height * 4));
      var colorSpace = CGColorSpace.CreateDeviceRGB();
      var context = new CGBitmapContext(buffer, (int)img.Width, (int)img.Height, 8, 4 * (int)img.Width, colorSpace, CGImageAlphaInfo.NoneSkipFirst);


      try
      {
        context.InterpolationQuality = CGInterpolationQuality.None;
        context.DrawImage(new Rectangle(0, 0, (int)img.Width, (int)img.Height), img.CGImage);

        unsafe
        {
          var bufPtr = (byte*)((void*)buffer);

          var pixWidth = imgBlockSize;
          var pixHeight = pixWidth * 2;
          var pixSeg = pixWidth * pixHeight;
          var numHeightIter = img.Height / pixHeight;
          var numWidthIter = img.Width / pixWidth;
          var percentIter = (int)(numHeightIter * numWidthIter / 100) + 1;
          var currIter = 0;


          for (var h = 0; h < numHeightIter; h++, currIter++)
          {
            // segment height
            var startY = (h * pixHeight);

            // segment width
            for (var w = 0; w < numWidthIter; w++, currIter++)
            {
              var startX = (w * pixWidth);
              var allBrightness = 0f;
              var allAlpha = 0f;
              var allRed = 0f;
              var allGreen = 0f;
              var allBlue = 0f;

              if (quick)
              {
                // each pix of this segment
                for (var y = 0; y < pixWidth; y++)
                {
                  var cY = y + startY;
                  var cX = 0 + startX;
                  try
                  {
                    if (cX < img.Width)
                    {
                      int offset = (int)(4 * (img.Width * cY + cX));
                      float alpha = (float)bufPtr[offset] / 255f;
                      float red = (float)bufPtr[offset + 1] / 255f;
                      float green = (float)bufPtr[offset + 2] / 255f;
                      float blue = (float)bufPtr[offset + 3] / 255f;

                      allAlpha += alpha;
                      allRed += red;
                      allGreen += green;
                      allBlue += blue;             
                      allBrightness += RGBtoBrightness(red, green, blue);
                    }
                  }
                  catch
                  {
                    allBrightness += 0.5f;
                  }
                }
              }
              else
              {
                // each pix of this segment
                for (var y = 0; y < pixWidth; y++)
                {
                  for (var x = 0; x < pixHeight; x++)
                  {
                    var cY = y + startY;
                    var cX = x + startX;
                    try
                    {
                      if (cX < img.Width)
                      {
                        int offset = (int)(4 * (img.Width * cY + cX));
                        float alpha = (float)bufPtr[offset] / 255f;
                        float red = (float)bufPtr[offset + 1] / 255f;
                        float green = (float)bufPtr[offset + 2] / 255f;
                        float blue = (float)bufPtr[offset + 3] / 255f;

                        allAlpha += alpha;
                        allRed += red;
                        allGreen += green;
                        allBlue += blue;
                        allBrightness += RGBtoBrightness(red, green, blue);
                      }
                    }
                    catch
                    {
                      allBrightness += 0.5f;
                    }
                  }
                }
              }

              var avgBrt = (allBrightness / pixSeg) * 100f;
              string asciiChar;
              if (avgBrt < 10)
              {
                asciiChar = "#";
              }
              else if (avgBrt < 17)
              {
                asciiChar = "@";
              }
              else if (avgBrt < 24)
              {
                asciiChar = "&";
              }
              else if (avgBrt < 31)
              {
                asciiChar = "$";
              }
              else if (avgBrt < 38)
              {
                asciiChar = "%";
              }
              else if (avgBrt < 45)
              {
                asciiChar = "|";
              }
              else if (avgBrt < 52)
              {
                asciiChar = "!";
              }
              else if (avgBrt < 59)
              {
                asciiChar = ";";
              }
              else if (avgBrt < 66)
              {
                asciiChar = ":";
              }
              else if (avgBrt < 73)
              {
                asciiChar = "'";
              }
              else if (avgBrt < 80)
              {
                asciiChar = "`";
              }
              else if (avgBrt < 87)
              {
                asciiChar = ".";
              }
              else
              {
                asciiChar = " ";
              }

              if (colour)
              {
                const string ColourCharElement = "<code style=\"color:{0}\">{1}</code>";
                
                var avgRed = (allRed / pixSeg);
                var avgGreen = (allGreen / pixSeg);
                var avgBlue = (allBlue / pixSeg);                
                var clrHex = string.Format("#{0:x}{1:x}{2:x}", (int)(avgRed * 255), (int)(avgGreen * 255), (int)(avgBlue * 255));
                var asciiStr = string.Format(ColourCharElement, clrHex, asciiChar);

                fileStream.Write(asciiStr);
              }
              else
              {
                fileStream.Write(asciiChar);
              }
            }
            fileStream.Write("\n");
          }
        }
      }
      finally
      {
        context.Dispose();
        colorSpace.Dispose();
      }
#endif

      const string WebPage3 =
            "</span>" +
          "</pre>" +
          "</body>" +
        "</html>";

      fileStream.Write(WebPage3);
    }
  }
}
