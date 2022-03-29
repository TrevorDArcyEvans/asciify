namespace asciify;

public class AsciiPageSize
{
  public string Name { get; init;}
  public int Width { get; init;}
  public int Height { get; init;}

  public AsciiPageSize(string name, int width, int height)
  {
    Name = name;
    Width = width;
    Height = height;
  }
}
