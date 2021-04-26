using Godot;
using System.Collections.Generic;
using HexMapUtil;

public class GameManager : Node2D
{
  private Dictionary<int, Character> _friendly = new Dictionary<int, Character>();
  private Dictionary<int, Character> _enemy = new Dictionary<int, Character>();
  private HexTileMap _map;
  private HUD _HUD;
  private int _turn = 1;
  private string _turnLetter = "a";

  private Character _selectedCharacter = null;

  public override void _Ready()
  {
    _map = GetNode<HexTileMap>("HexTileMap");
    _HUD = GetNode<HUD>("HUD");
    _friendly.Add(0, GetNode<Character>("character"));
    _enemy.Add(1, GetNode<TestEnemy>("testEnemy"));

    // center friendly on (0,0) on grid
    Vector2 location = _map.OddQToWorld(new HexLocation(_friendly[0]._XStart, _friendly[0]._YStart));
    _friendly[0].Position = location;

    // center enemy on 3,0
    location = _map.OddQToWorld(new HexLocation(_enemy[1]._XStart, _enemy[1]._YStart));
    _enemy[1].Position = location;

    _HUD.UpdateTurn(_turn, _turnLetter);
  }

  public override void _Process(float delta)
  {
    if (Input.IsActionJustPressed("select"))
    {
      int characterID = _map.GetCharacterID(GetGlobalMousePosition());
      if (_friendly.ContainsKey(characterID))
      {

        if (_selectedCharacter != null)
          _selectedCharacter.Deselect();

        if (_selectedCharacter != null && _selectedCharacter.ID == characterID)
          _selectedCharacter = null;
        else
        {
          _selectedCharacter = _friendly[characterID];
          _selectedCharacter.Select();
          _HUD.OnSelectCharacter(_selectedCharacter);
        }
      }
      else if (_enemy.ContainsKey(characterID))
      {
        if (_selectedCharacter != null)
          _selectedCharacter.Deselect();

        // show stats on UI
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
      _HUD.UpdateAP(_selectedCharacter.AP);
    }
    else if (Input.IsActionJustPressed("end_turn"))
    {
      _turn += 1;
      if (_turnLetter == "a")
        _turnLetter = "b";
      else
        _turnLetter = "a";
      _HUD.UpdateTurn(_turn, _turnLetter);
      // update all characters onturnstart 
      foreach (Character character in _friendly.Values)
      {
        character.OnTurnEnd();
        character.OnTurnStart();
      }
    }
  }
}