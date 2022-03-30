namespace asciify.core;

using System.Collections.ObjectModel;

public sealed class AsciiPageSize
{
  public string Name { get; }
  public int Width { get; }
  public int Height { get; }

  public AsciiPageSize(string name, int width, int height)
  {
    Name = name;
    Width = width;
    Height = height;
  }

  private static List<AsciiPageSize> _defaultSizes = new()
  {
    new AsciiPageSize("micro", 320, 240),
    new AsciiPageSize("tiny", 480, 360),
    new AsciiPageSize("small", 640, 480),
    new AsciiPageSize("medium", 800, 600),
    new AsciiPageSize("large", 1024, 768),
    new AsciiPageSize("extra large", 1280, 1024),
    new AsciiPageSize("super large", 1600, 1200)
  };

  public static ReadOnlyCollection<AsciiPageSize> DefaultSizes => _defaultSizes.AsReadOnly();
}
