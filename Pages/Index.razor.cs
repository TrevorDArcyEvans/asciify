namespace asciify;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StaticDust;

public sealed partial class Index
{
  List<AsciiPageSize> sizes = new List<AsciiPageSize>
  {
    new AsciiPageSize("micro", 320, 240),
    new AsciiPageSize("tiny", 480, 360),
    new AsciiPageSize("small", 640, 480),
    new AsciiPageSize("medium", 800, 600),
    new AsciiPageSize("large", 1024, 768),
    new AsciiPageSize("extra large", 1280, 1024),
    new AsciiPageSize("super large", 1600, 1200)
  };
  
  private void CreateAsciiArt(Image<Rgba32> mImageSelectorImage, AsciiPageSize selectedPageSize)
  {
    // calculate block size
    const int FudgeFactor = 5;
    var blockWidth = mImageSelectorImage.Width / selectedPageSize.Width;
    var blockHeight = mImageSelectorImage.Height / selectedPageSize.Height;
    var blockSize = FudgeFactor * Math.Max(blockWidth, blockHeight);
    blockSize = Math.Max(1, blockSize);
    
    // convert to ascii
    var filename = Path.GetTempPath();
    using (var fs = new StreamWriter(filename))
    {
      AsciiArt.ConvertImage(mImageSelectorImage, fs, (int)blockSize, 5, false, true);
    }

    // save to file
    //File.WriteAllText(filename, browserStr);
  }
}
