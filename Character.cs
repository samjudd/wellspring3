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

  public static int _characterID = 0;
  public static int _XStart = 0;
  public static int _YStart = 0;
  public HexLocation _location { get; private set; }
  public List<int> _attackRange = new List<int>();

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
  }

  public override void _Process(float delta)
  {
    if (_moving)
    {
      _moveTime += delta;
      Position = _moveStart.LinearInterpolate(_moveEnd, _moveTime / _moveDuration);
      if (Position.DistanceSquaredTo(_moveEnd) < 5)
      {
        Position = _moveEnd;
        _moving = false;
        _moveTime = 0f;
        // reselect character to recompute dijkstra showing movement
        if (_moveQueue.Count == 0)
          Select();
      }
    }
    else if (_moveQueue.Count != 0)
    {
      MoveSingleStep(_moveQueue.Dequeue());
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
    }
    return true;
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
    _movementRange = _hexMap.ShowMovementRange(_location, _currentAP);
  }
  public void Deselect()
  {
    _hexMap.ClearMoveHighlights();
  }

  public void OnTurnStart()
  {
    _currentAP = _maxAP;
  }
}
