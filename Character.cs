using Godot;
using HexMapUtil;

public class Character : Sprite
{
  public int characterID = 1;
  public int XStart = 0;
  public int YStart = 0;
  public HexLocation location;

  public override void _Ready()
  {
    location = new HexLocation(XStart, YStart);
  }
}
