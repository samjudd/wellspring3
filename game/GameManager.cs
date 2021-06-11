using Godot;
using System.Collections.Generic;
using HexMapUtil;

public class GameManager : Node2D
{
  public Dictionary<int, Character> _characters = new Dictionary<int, Character>();
  private HexTileMap _map;
  private HUD _HUD;
  private int _turn = 1;
  private string _turnLetter = "a";

  private Character _selectedCharacter = null;

  public override void _Ready()
  {
    _map = GetNode<HexTileMap>("HexTileMap");
    _HUD = GetNode<HUD>("HUD");
    _characters.Add(0, GetNode<Character>("character"));
    _characters.Add(1, GetNode<TestEnemy>("testEnemy"));

    // center friendly on (0,0) on grid
    Vector2 location = _map.OddQToWorld(new HexLocation(_characters[0]._XStart, _characters[0]._YStart));
    _characters[0].Position = location;

    // center enemy on 3,0
    location = _map.OddQToWorld(new HexLocation(_characters[1]._XStart, _characters[1]._YStart));
    _characters[1].Position = location;

    _HUD.UpdateTurn(_turn, _turnLetter);
  }

  public override void _Process(float delta)
  {
    if (Input.IsActionJustPressed("select"))
    {
      int characterID = _map.GetCharacterID(GetGlobalMousePosition());
      HexLocation location = _map.WorldToOddQ(GetGlobalMousePosition());
      if (_characters.ContainsKey(characterID))
      {
        // if you click selected character deselect it
        if (_selectedCharacter == null)
          Select(characterID);
        else if (_selectedCharacter.ID == characterID)
          Deselect();
        // if you clicked another friendly character select that one instead
        else if (_characters[characterID]._faction == GameUtil.Faction.FRIENDLY)
          Select(characterID);
        // if you clicked an enemy character attack if in range, select otherwise
        else if (_characters[characterID]._faction == GameUtil.Faction.ENEMY)
        {
          if (_selectedCharacter == null)
            Select(characterID);
          else
          {
            if (_selectedCharacter._attackRange.Contains(location))
            {
              _selectedCharacter.Attack(characterID);
              _HUD.UpdateAP(_selectedCharacter.AP);
            }
            else
              Select(characterID);
          }
        }
      }
      // if you click at a spot on the map with nothing to select, move if within range otherwise deselect
      else if (_selectedCharacter != null)
      {
        if (_selectedCharacter._movementRange.ContainsKey(location))
        {
          _selectedCharacter.Move(location);
          _HUD.UpdateAP(_selectedCharacter.AP);
        }
        else
          Deselect();
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
      foreach (Character character in _characters.Values)
      {
        if (character._faction == GameUtil.Faction.FRIENDLY)
        {
          character.OnTurnEnd();
          character.OnTurnStart();
        }
      }
    }
  }

  private void Select(int newCharacterID)
  {
    if (_selectedCharacter != null)
      _selectedCharacter.Deselect();
    _selectedCharacter = _characters[newCharacterID];
    // only allow movement if character is friendly
    if (_characters[newCharacterID]._faction == GameUtil.Faction.FRIENDLY)
      _selectedCharacter.Select();
    _HUD.OnSelectCharacter(_selectedCharacter);
  }

  private void Deselect()
  {
    if (_selectedCharacter != null)
      _selectedCharacter.Deselect();
    _selectedCharacter = null;
    // change HUD to have nothing selected
  }
}