namespace asciify.ui.cli;

using CommandLine;
using core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StaticDust;

internal static class Program
{
  public static async Task Main(string[] args)
  {
    var result = await Parser.Default.ParseArguments<Options>(args)
      .WithParsedAsync(Run);
    await result.WithNotParsedAsync(HandleParseError);
  }

  private static async Task Run(Options opt)
  {
    var img = await Image.LoadAsync<Rgba32>(opt.ImageFilePath);
    var size = AsciiPageSize.DefaultSizes.Single(x => x.Name == opt.Size);
    var html = CreateAsciiArt(img, size);
    await File.WriteAllTextAsync(opt.OutputFilePath, html);
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

  private static Task HandleParseError(IEnumerable<Error> errs)
  {
    if (errs.IsVersion())
    {
      Console.WriteLine("Version Request");
      return Task.CompletedTask;
    }

    if (errs.IsHelp())
    {
      Console.WriteLine("Help Request");
      return Task.CompletedTask;
      ;
    }

    Console.WriteLine("Parser Fail");
    return Task.CompletedTask;
  }
}
