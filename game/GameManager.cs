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
  private HUD _HUD;
  private int _turn = 1;

  private Character _selectedCharacter = new Character(Constants.NOCHARACTER);

  public override void _Ready()
  {
    _characters.Add(GetNode<Character>("character"));
    _map = GetNode<HexTileMap>("HexTileMap");
    _HUD = GetNode<HUD>("HUD");

    // center character on (0,0) on grid
    Vector2 location = _map.OddQToWorld(new HexLocation(_char1StartX, _char1StartY));
    _characters[0].Position = location;
  }

  public override void _Process(float delta)
  {
    if (Input.IsActionJustPressed("select"))
    {
      int characterID = _map.GetCharacterID(GetGlobalMousePosition());
      if (characterID != Constants.NOCHARACTER && characterID != _selectedCharacter.ID)
      {
        _selectedCharacter = _characters[characterID];
        _selectedCharacter.Select();
        _HUD.OnSelectCharacter(_selectedCharacter);
      }
      else if (_selectedCharacter.ID != Constants.NOCHARACTER)
      {
        _selectedCharacter.Deselect();
        _selectedCharacter = new Character(Constants.NOCHARACTER);
        // need to hide character UI altogether when nothing is selected
      }
    }
    else if (Input.IsActionJustPressed("pathfind") && _selectedCharacter != null)
    {
      // move character to selected spot if possible
      HexLocation selectedHex = _map.WorldToOddQ(GetGlobalMousePosition());
      _selectedCharacter.Move(selectedHex);
      _HUD.UpdateAP(_selectedCharacter.AP);
    }
    else if (Input.IsActionJustPressed("end_turn"))
    {
      _turn += 1;
      _HUD.UpdateTurn(_turn);
      // update all characters onturnstart 
      for (int i = 0; i < _characters.Count; i++)
        _characters[i].OnTurnStart();
    }
  }
}