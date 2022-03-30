namespace asciify.ui.cli;

using CommandLine;

internal sealed class Options
{
  [Value(index: 0, Required = true, HelpText = "Path to image file")]
  public string ImageFilePath { get; set; }

  [Option('s', "Size", Required = false, Default = "medium", HelpText = "Size of output image")]
  public string Size { get; set; }

  [Option('o', "Output", Required = false, Default = "index.html", HelpText = "Path to output file")]
  public string OutputFilePath { get; set; }
}
