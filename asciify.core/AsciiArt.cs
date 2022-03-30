/*======================================================================/

Copyright (C) 2004 Daniel Fisher(lennybacon).  All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

For more information email: info@lennybacon.com

12/27/04 Enhancements by Steven Fowler (steven_m_fowler@yahoo.com)
=======================================================================*/

namespace StaticDust;

using System.Text;
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

  private static string BrightnessToChar(float avgBrt)
  {
    var asciiChar = avgBrt switch
    {
      < 10 => "#",
      < 17 => "@",
      < 24 => "&",
      < 31 => "$",
      < 38 => "%",
      < 45 => "|",
      < 52 => "!",
      < 59 => ";",
      < 66 => ":",
      < 73 => "'",
      < 80 => "`",
      < 87 => ".",
      _ => " "
    };

    return asciiChar;
  }

  /// <summary>
  /// converts an image to an ascii web page
  /// </summary>
  /// <param name="img">image to convert</param>
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
  /// <returns>HTML string containing ascii representation of image</returns>
  public static string ConvertImage(
    Image<Rgba32> img,
    int imgBlockSize,
    int fontSize,
    bool quick,
    bool colour)
  {
    var sb = new StringBuilder();
    var WebPage1 =
      "<html>" +
      "<body>" +
      "<pre>" +
      $"<span style=\"font-size: {fontSize}px;font-family: monospace;\">";
    sb.Append(WebPage1);

    var bufPtr = new byte[img.Width * img.Height * 4];

    var pixWidth = imgBlockSize;
    var pixHeight = pixWidth * 2;
    var pixSeg = pixWidth * pixHeight;
    var numHeightIter = img.Height / pixHeight;
    var numWidthIter = img.Width / pixWidth;

    img.ProcessPixelRows(acc =>
    {
      for (var y = 0; y < acc.Height; y++)
      {
        var pxRow = acc.GetRowSpan(y);
        for (var x = 0; x < pxRow.Length - 1; x++)
        {
          ref var px = ref pxRow[x];
        }
      }
    });


    for (var h = 0; h < numHeightIter; h++)
    {
      // segment height
      var startY = h * pixHeight;

      // segment width
      for (var w = 0; w < numWidthIter; w++)
      {
        var startX = w * pixWidth;
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
                var offset = 4 * (img.Width * cY + cX);
                var alpha = bufPtr[offset] / 255f;
                var red = bufPtr[offset + 1] / 255f;
                var green = bufPtr[offset + 2] / 255f;
                var blue = bufPtr[offset + 3] / 255f;

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
                  var offset = 4 * (img.Width * cY + cX);
                  var alpha = bufPtr[offset] / 255f;
                  var red = bufPtr[offset + 1] / 255f;
                  var green = bufPtr[offset + 2] / 255f;
                  var blue = bufPtr[offset + 3] / 255f;

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

        var avgBrt = allBrightness / pixSeg * 100f;
        var asciiChar = BrightnessToChar(avgBrt);

        if (colour)
        {
          const string ColourCharElement = "<code style=\"color:{0}\">{1}</code>";

          var avgRed = allRed / pixSeg;
          var avgGreen = allGreen / pixSeg;
          var avgBlue = allBlue / pixSeg;
          var clrHex = $"#{(int)(avgRed * 255):x}{(int)(avgGreen * 255):x}{(int)(avgBlue * 255):x}";
          var asciiStr = string.Format(ColourCharElement, clrHex, asciiChar);

          sb.Append(asciiStr);
        }
        else
        {
          sb.Append(asciiChar);
        }
      }

      sb.Append(Environment.NewLine);
    }

    const string WebPage3 =
      "</span>" +
      "</pre>" +
      "</body>" +
      "</html>";

    sb.Append(WebPage3);

    return sb.ToString();
  }
}
