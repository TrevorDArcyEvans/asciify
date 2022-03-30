using asciify.core;

namespace asciify.ui.web.Pages;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StaticDust;

public sealed partial class Index
{
  private string selectedSize { get; set; }
  private string _rendered { get; set; }

  protected override void OnInitialized()
  {
    selectedSize = AsciiPageSize.DefaultSizes[0].Name;
    base.OnInitialized();
  }

  private static string CreateAsciiArt(Image<Rgba32> img, AsciiPageSize selectedPageSize)
  {
    // calculate block size
    const int FudgeFactor = 5;
    var blockWidth = img.Width / selectedPageSize.Width;
    var blockHeight = img.Height / selectedPageSize.Height;
    var blockSize = FudgeFactor * Math.Max(blockWidth, blockHeight);
    blockSize = Math.Max(1, blockSize);
    
    // convert to ascii
    return AsciiArt.ConvertImage(img, blockSize, 5, false, true);
  }

  private async Task LoadFile(InputFileChangeEventArgs e)
  {
    var data = e.File.OpenReadStream();
    var ms = new MemoryStream();
    await data.CopyToAsync(ms);
    ms.Seek(0, SeekOrigin.Begin);

    var info = await Image.IdentifyAsync(ms);
    if (info is null)
    {
      _rendered = $"<b>{e.File.Name}</b> --> unknown format";
      return;
    }

    ms.Seek(0, SeekOrigin.Begin);
    var img = Image.Load<Rgba32>(ms);
    var size = AsciiPageSize.DefaultSizes.Single(x => x.Name == selectedSize);
    var html = CreateAsciiArt(img, size);
    _rendered = html;
  }
}
