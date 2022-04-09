using asciify.core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace asciify.ui.web.Pages;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StaticDust;

public sealed partial class Index
{
  [Inject]
  private IJSRuntime JSRuntime { get; set; }

  private string selectedSize { get; set; }
  private string _rendered { get; set; }
  private string _imgUrl { get; set; } = GetDefaultImageString();
  private bool IsFinishedEmbed { get; set; }

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
    IsFinishedEmbed = false;
    _imgUrl = await GetImageString(e.File);
    StateHasChanged();

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

    IsFinishedEmbed = true;
    StateHasChanged();
  }

  private static async Task<string> GetImageString(IBrowserFile file)
  {
    var buffers = new byte[file.Size];
    await file.OpenReadStream().ReadAsync(buffers);
    return $"data:{file.ContentType};base64,{Convert.ToBase64String(buffers)}";
  }

  private static string GetDefaultImageString(int width = 64, int height = 64)
  {
    var img = new Image<Rgba32>(Configuration.Default, width, height);
    using var ms = new MemoryStream();
    img.SaveAsPng(ms);
    var bytes = ms.ToArray();
    return $"data:img/png;base64,{Convert.ToBase64String(bytes)}";
  }

  private async Task DownloadRender()
  {
    // Generate a text file
    var data = System.Text.Encoding.UTF8.GetBytes(_rendered);
    await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", "index.html", "text/plain", data);
  }
}
