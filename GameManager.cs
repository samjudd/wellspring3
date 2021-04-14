using Godot;
using HexMapUtil;

public class GameManager : Node2D
{
  [Export]
  public int _char1StartX = 0;
  [Export]
  public int _char1StartY = 0;

  public Character _character1;
  public HexTileMap _map;

  private System.Collections.Generic.Dictionary<HexLocation, PathToHex> _hexPaths;
  private Sprite _selectedCharacter;

  public override void _Ready()
  {
    _character1 = GetNode<Character>("character");
    _map = GetNode<HexTileMap>("HexTileMap");

    // center character on (0,0) on grid
    Vector2 location = _map.OddQToWorld(new HexLocation(_char1StartX, _char1StartY));
    _character1.Position = location;
  }

  public override void _Process(float delta)
  {
    if (Input.IsActionJustPressed("select"))
    {
      _hexPaths = _map.SelectTile(_map.WorldToOddQ(GetGlobalMousePosition()), 8);
    }
    if (Input.IsActionJustPressed("pathfind"))
    {
      _hexPaths = _map.SelectTile(_map.WorldToOddQ(GetGlobalMousePosition()), 8);
    }
  }
}