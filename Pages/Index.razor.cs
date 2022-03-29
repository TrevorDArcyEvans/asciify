namespace asciify.Pages;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StaticDust;

public sealed partial class Index
{
  private readonly List<AsciiPageSize> _sizes = new List<AsciiPageSize>
  {
    new AsciiPageSize("micro", 320, 240),
    new AsciiPageSize("tiny", 480, 360),
    new AsciiPageSize("small", 640, 480),
    new AsciiPageSize("medium", 800, 600),
    new AsciiPageSize("large", 1024, 768),
    new AsciiPageSize("extra large", 1280, 1024),
    new AsciiPageSize("super large", 1600, 1200)
  };
  private string _rendered { get; set; }

  private void CreateAsciiArt(Image<Rgba32> img, AsciiPageSize selectedPageSize)
  {
    // calculate block size
    const int FudgeFactor = 5;
    var blockWidth = img.Width / selectedPageSize.Width;
    var blockHeight = img.Height / selectedPageSize.Height;
    var blockSize = FudgeFactor * Math.Max(blockWidth, blockHeight);
    blockSize = Math.Max(1, blockSize);
    
    // convert to ascii
    var filename = Path.GetTempPath();
    using (var fs = new StreamWriter(filename))
    {
      AsciiArt.ConvertImage(img, fs, blockSize, 5, false, true);
    }

    // save to file
    //File.WriteAllText(filename, browserStr);
  }

  private async Task LoadFile(InputFileChangeEventArgs e)
  {
    var data = e.File.OpenReadStream();
    var ms = new MemoryStream();
    await data.CopyToAsync(ms);
    ms.Seek(0, SeekOrigin.Begin);

    var img = Image.Load<Rgba32>(ms);
    _rendered = $"<b>{e.File.Name}</b> --> {img.Width} x {img.Height}";
  }
}
