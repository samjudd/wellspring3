using Godot;
using System.Collections.Generic;
using HexMapUtil;

public class GameManager : Node2D
{
  [Export]
  public int _char1StartX = 0;
  [Export]
  public int _char1StartY = 0;

  private List<Character> _characters = new List<Character>();
  private HexTileMap _map;

  private Character _selectedCharacter = null;

  public override void _Ready()
  {
    _characters.Add(GetNode<Character>("character"));
    _map = GetNode<HexTileMap>("HexTileMap");

    // center character on (0,0) on grid
    Vector2 location = _map.OddQToWorld(new HexLocation(_char1StartX, _char1StartY));
    _characters[0].Position = location;
  }

  public override void _Process(float delta)
  {
    if (Input.IsActionJustPressed("select"))
    {
      int characterID = _map.GetCharacterID(GetGlobalMousePosition());
      if (characterID != Constants.NOCHARACTER)
      {
        _selectedCharacter = _characters[characterID];
        _selectedCharacter.Select();
      }
      else if (_selectedCharacter != null)
      {
        _selectedCharacter.Deselect();
        _selectedCharacter = null;
      }
    }
    else if (Input.IsActionJustPressed("pathfind") && _selectedCharacter != null)
    {
      // move character to selected spot if possible
      HexLocation selectedHex = _map.WorldToOddQ(GetGlobalMousePosition());
      _selectedCharacter.Move(selectedHex);
    }
    else if (Input.IsActionJustPressed("end_turn"))
    {
      _selectedCharacter.OnTurnStart();
    }
  }
}