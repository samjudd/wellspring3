using Godot;
using System.Collections.Generic;
using HexMapUtil;

public class Character : Sprite
{
  [Export]
  public int _maxAP = 10;
  [Export]
  public int _maxHP = 100;
  [Export]
  public float _moveSpeedTilesPerSecond = 3.0f;
  [Export]
  public int _attackCost = 4;
  [Export]
  public int[] _attackRange = new int[] { 1 };

  public int HP
  {
    get { return _currentHP; }
    set { _currentHP = Mathf.Clamp(value, 0, _maxHP); }
  }

  public int AP
  {
    get { return _currentAP; }
    set { _currentAP = Mathf.Clamp(value, 0, _maxAP); }
  }

  public int ID
  {
    get { return _characterID; }
  }

  private int _characterID = 0;
  public static int _XStart = 0;
  public static int _YStart = 0;
  public HexLocation _location { get; private set; }

  private HexTileMap _hexMap;
  private int _currentAP;
  private int _currentHP;
  private System.Collections.Generic.Dictionary<HexLocation, PathToHex> _movementRange = null;
  private float _moveSpeedPxPerSecond;

  private Queue<HexLocation> _moveQueue = new Queue<HexLocation>();
  private Vector2 _moveStart;
  private Vector2 _moveEnd;
  private float _moveDuration = 1;
  private float _moveTime = 0;
  private bool _moving = false;

  private HexLocation _previewHex;
  private bool _selected = false;

  // need empty constructor for godot engine to instantiate with
  public Character() { }

  public Character(int characterID)
  {
    _characterID = characterID;
  }

  public override void _Ready()
  {
    _location = new HexLocation(_XStart, _YStart);
    _hexMap = GetNode<HexTileMap>("../HexTileMap");
    _currentAP = _maxAP;
    _currentHP = _maxHP;
    // multiply by flat-to-flat distance to get move speed in pixels
    _moveSpeedPxPerSecond = _moveSpeedTilesPerSecond * _hexMap._hexDimensionsPx.y;

    // set map to know location of this character
    _hexMap._map.GetHexTile(_location).AddCharacter(_characterID);

    _previewHex = _location;
  }

  public override void _Process(float delta)
  {
    // ############################## MOVEMENT ##############################
    if (_moving)
    {
      _moveTime += delta;
      Position = _moveStart.LinearInterpolate(_moveEnd, _moveTime / _moveDuration);
      // single step of move is complete 
      if (Position.DistanceSquaredTo(_moveEnd) < 5)
      {
        Position = _moveEnd;
        _moveTime = 0f;
        _moving = false;
        // check if move is fully complete or not
        if (_moveQueue.Count == 0)
        {
          _hexMap.ClearMapLines();
          Deselect();
          Select();
        }
        else
          MoveSingleStep(_moveQueue.Dequeue());
      }
    }

    // ############################## MOVE PREVIEW ##############################
    HexLocation mouseLocation = _hexMap.WorldToOddQ(GetGlobalMousePosition());
    if (_selected && !_moving && _previewHex != mouseLocation && _movementRange.ContainsKey(mouseLocation))
    {
      _hexMap.ClearMapLines();
      _hexMap.DrawPath(_location, mouseLocation, _movementRange);
      _previewHex = mouseLocation;
    }
  }

  public bool Move(HexLocation newLocation)
  {
    List<HexLocation> path = Util.HexPathfind(_location, newLocation, _movementRange);
    if (path == null || _moveQueue.Count != 0)
      return false;
    else
    {
      foreach (HexLocation hex in path)
        _currentAP -= _hexMap._map.GetHexTile(hex).movement;
      _moveQueue = new Queue<HexLocation>(path);
      MoveSingleStep(_moveQueue.Dequeue());
      return true;
    }
  }

  private void MoveSingleStep(HexLocation newLocation)
  {
    if (!_moving)
    {
      _moveStart = _hexMap.OddQToWorld(_location);
      _moveEnd = _hexMap.OddQToWorld(newLocation);
      _moveDuration = _moveEnd.DistanceTo(_moveStart) / _moveSpeedPxPerSecond;
      ChangeLocation(newLocation);
      _moving = true;
    }
  }

  private void ChangeLocation(HexLocation newLocation)
  {
    _hexMap._map.GetHexTile(_location).RemoveCharacter();
    _location = newLocation;
    _hexMap._map.GetHexTile(_location).AddCharacter(_characterID);
  }

  public void Select()
  {
    _selected = true;
    _movementRange = _hexMap.ShowMovementRange(_location, _currentAP);
    if (_currentAP >= _attackCost)
      _hexMap.ShowAttackRange(_location, new List<int>(_attackRange));
  }
  public void Deselect()
  {
    _hexMap.ClearMoveHighlights();
    _hexMap.ClearAttackHighlights();
    _hexMap.ClearMapLines();
    _hexMap.HighlightHexes();
    _selected = false;
  }

  public void OnTurnStart()
  {
    _currentAP = _maxAP;
  }

  public void OnTurnEnd()
  {
    Deselect();
  }
}
