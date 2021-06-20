using Godot;
using System.Collections.Generic;
using HexMapUtil;

public class GameManager : Node2D
{
  [Export]
  Vector2[] _AStartLocations = new Vector2[5];
  [Export]
  Vector2[] _BStartLocations = new Vector2[5];

  public Dictionary<int, Character> _characters = new Dictionary<int, Character>();
  private HexTileMap _map;
  private HUD _HUD;
  private int _turn = 1;
  private string _turnLetter = "A";
  private GameUtil.Faction _playerFaction = GameUtil.Faction.TEAMA;
  private GameUtil.Faction _otherFaction = GameUtil.Faction.TEAMB;

  private Character _selectedCharacter = null;

  private PackedScene _char1 = ResourceLoader.Load<PackedScene>("res://characters/character.tscn");
  private PackedScene _enemy1 = ResourceLoader.Load<PackedScene>("res://enemies/testEnemy.tscn");

  public override void _Ready()
  {
    _map = GetNode<HexTileMap>("HexTileMap");
    _HUD = GetNode<HUD>("HUD");

    // create all the characters to populate the world
    int ID = 0;
    for (int i = 0; i < _AStartLocations.Length; i++)
    {
      _characters.Add(ID, (Character)_char1.Instance());
      _characters[ID].Init(ID, GameUtil.Faction.TEAMA, new HexLocation(_AStartLocations[i]));
      AddChild(_characters[ID]);
      ID += 1;
    }

    for (int i = 0; i < _BStartLocations.Length; i++)
    {
      _characters.Add(ID, (TestEnemy)_enemy1.Instance());
      _characters[ID].Init(ID, GameUtil.Faction.TEAMB, new HexLocation(_BStartLocations[i]));
      AddChild(_characters[ID]);
      ID += 1;
    }

    _HUD.UpdateTurn(_turn, _turnLetter);
  }

  public override void _Process(float delta)
  {
    if (Input.IsActionJustPressed("select"))
    {
      int characterID = _map.GetCharacterID(GetGlobalMousePosition());
      GD.Print(characterID);
      HexLocation location = _map.WorldToOddQ(GetGlobalMousePosition());
      if (_characters.ContainsKey(characterID))
      {
        // if you click selected character deselect it
        if (_selectedCharacter == null)
          Select(characterID);
        else if (_selectedCharacter.ID == characterID)
          Deselect();
        // if you clicked another friendly character select that one instead
        else if (_characters[characterID]._faction == _playerFaction)
          Select(characterID);
        // if you clicked an enemy character attack if in range, select otherwise
        else if (_characters[characterID]._faction == _otherFaction)
        {
          if (_selectedCharacter == null || _selectedCharacter._faction == _otherFaction)
            Select(characterID);
          else // selected character is player faction
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
    else if (Input.IsActionJustPressed("end_turn"))
    {
      // Update HUD
      _turn += 1;
      if (_turnLetter == "A")
        _turnLetter = "B";
      else
        _turnLetter = "A";
      _HUD.UpdateTurn(_turn, _turnLetter);

      // run turn start and end functions for all characters
      foreach (Character character in _characters.Values)
      {
        if (character._faction == _playerFaction)
          character.OnTurnEnd();
        if (character._faction == _otherFaction)
          character.OnTurnStart();
      }
      // swap factions so player 2 can control
      GameUtil.Faction swap = _playerFaction;
      _playerFaction = _otherFaction;
      _otherFaction = swap;
    }
  }

  private void Select(int newCharacterID)
  {
    if (_selectedCharacter != null)
      _selectedCharacter.Deselect();
    _selectedCharacter = _characters[newCharacterID];
    // only allow movement if character is friendly
    if (_characters[newCharacterID]._faction == _playerFaction)
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