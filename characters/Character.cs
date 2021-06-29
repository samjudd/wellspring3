using Godot;
using System.Collections.Generic;
using HexMapUtil;

public class Character : Sprite
{
  [Export]
  public string _characterName = "baseChar";
  [Export]
  public string _abilityName = "baseAblty";
  [Export]
  public int _baseAP = 10;
  [Export]
  public int _baseHP = 100;
  [Export]
  public int _baseAttackCost = 4;
  [Export]
  public int _baseAttackDamage = 10;
  [Export]
  public int[] _baseAttackDistance = new int[] { 1 };
  [Export]
  public int _baseArmor = 0;
  [Export]
  public int _baseSpellPower = 0;
  [Export]
  public int _baseSpellResist = 0;
  [Export]
  public int _baseAbilityCooldown = 1;
  [Export]
  public int _baseAbilityCost = 6;

  [Export]
  public float _moveSpeedTilesPerSecond = 3.0f;

  public PointAttribute HP { get; private set; }
  public PointAttribute AP { get; private set; }

  public StatAttribute AttackDamage { get; private set; }
  public StatAttribute AttackCost { get; private set; }
  public StatAttribute Armor { get; private set; }
  public StatAttribute SpellPower { get; private set; }
  public StatAttribute SpellResist { get; private set; }
  public StatAttribute AbilityCost { get; private set; }

  public Cooldown AbilityCD { get; private set; }

  public int ID
  {
    get { return _characterID; }
  }

  protected int _characterID = 0;
  public HexLocation _location { get; private set; }
  public GameUtil.Faction _faction = GameUtil.Faction.TEAMA;

  public GameManager _gameManager;
  protected HexTileMap _hexMap;
  public System.Collections.Generic.Dictionary<HexLocation, PathToHex> _movementRange = null;
  public List<HexLocation> _attackRange = null;
  private float _moveSpeedPxPerSecond;

  private Queue<HexLocation> _moveQueue = new Queue<HexLocation>();
  private Vector2 _moveStart;
  private Vector2 _moveEnd;
  private float _moveDuration = 1;
  private float _moveTime = 0;
  private bool _moving = false;

  private HexLocation _previewHex;
  private bool _selected = false;

  public Character() { }

  public override void _Ready()
  {
    _hexMap = GetNode<HexTileMap>("../HexTileMap");
    _gameManager = GetNode<GameManager>("..");
    // multiply by flat-to-flat distance to get move speed in pixels
    _moveSpeedPxPerSecond = _moveSpeedTilesPerSecond * _hexMap._hexDimensionsPx.y;

    // set map to know location of this character
    _hexMap._map.GetHexTile(_location).AddCharacter(_characterID);
    // move character to map location
    Position = _hexMap.OddQToWorld(_location);

    _previewHex = _location;

    // initialize all stats
    HP = new PointAttribute(_baseHP);
    AP = new PointAttribute(_baseAP);
    AttackDamage = new StatAttribute(_baseAttackDamage);
    AttackCost = new StatAttribute(_baseAttackCost);
    Armor = new StatAttribute(_baseArmor);
    SpellPower = new StatAttribute(_baseSpellPower);
    SpellResist = new StatAttribute(_baseSpellResist);
    AbilityCD = new Cooldown(_baseAbilityCooldown);
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

  public void Init(int characterID, GameUtil.Faction faction, HexLocation startLocation)
  {
    _characterID = characterID;
    _faction = faction;
    _location = startLocation;
  }

  public bool Move(HexLocation newLocation)
  {
    List<HexLocation> path = Util.HexPathfind(_location, newLocation, _movementRange);
    if (path == null || _moveQueue.Count != 0)
      return false;
    else
    {
      foreach (HexLocation hex in path)
        AP.value -= _hexMap._map.GetHexTile(hex).movement;
      _moveQueue = new Queue<HexLocation>(path);
      MoveSingleStep(_moveQueue.Dequeue());
      return true;
    }
  }

  public bool Attack(int target)
  {
    if (AP.value >= AttackCost.value)
    {
      _gameManager._characters[target].OnAttack(AttackDamage.value);
      AP.value -= AttackCost.value;
      _hexMap.ClearMapLines();
      Deselect();
      Select();
      return true;
    }
    else
      return false;
  }

  public virtual void CastAbility() { }

  public void OnAttack(int damage)
  {
    HP.value -= damage;
    if (HP.value == 0)
      Die();
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

  private void Die()
  {
    _hexMap._map.GetHexTile(_location).RemoveCharacter();
    QueueFree();
  }

  public void Select()
  {
    _selected = true;
    _movementRange = _hexMap.ShowMovementRange(this);
    if (AP.value >= AttackCost.value)
      _attackRange = _hexMap.ShowAttackRange(_location, new List<int>(_baseAttackDistance));
    else
      _attackRange = new List<HexLocation>();
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
    AP.value = AP.maxValue;
    AbilityCD.OnTurnStart();
  }

  public void OnTurnEnd()
  {
    Deselect();
  }
}
