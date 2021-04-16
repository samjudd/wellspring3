using Godot;
using Godot.Collections;
using HexMapUtil;
using Priority_Queue;
using System.Collections.Generic;

public class HexTileMap : TileMap
{
  public HexLocation _hexDimensionsPx = new HexLocation(140, 120);
  // array of custom hextile struct that can store hex properties
  public HexMap _map;
  // distance from center of hex to any corner
  private float _hexSizePx;
  // lines on map
  private List<Line2D> _mapLines = new List<Line2D>();

  public override void _Ready()
  {
    _hexSizePx = _hexDimensionsPx.x / 2f;

    // get bounds of tilemap
    Array usedTiles = GetUsedCells();
    int xMax = 0, yMax = 0;
    foreach (Vector2 tile in usedTiles)
    {
      xMax = Mathf.Max(xMax, Mathf.RoundToInt(tile.x));
      yMax = Mathf.Max(yMax, Mathf.RoundToInt(tile.y));
    }

    // need to add 1 since 0 index makes the size calcs otherwise off by 1
    _map = new HexMap(xMax + 1, yMax + 1);
    for (int i = 0; i < usedTiles.Count; i++)
    {
      // cast as vector2 since godot doesn't store them in a typed array for some reason
      Vector2 tile = (Vector2)usedTiles[i];
      HexLocation tileHL = new HexLocation(tile);
      int tileID = GetCellv(tile);
      _map[tileHL.x][tileHL.y] = new HexTile(tileID, tileHL, Util.TileIDToMovement[tileID]);
      Vector2 locationPx = OddQToWorld(tileHL);
      PrintText(tile.ToString(), locationPx);
    }
  }

  public System.Collections.Generic.Dictionary<HexLocation, PathToHex> ShowMovementRange(HexLocation tile, int movement)
  {
    if (_map.Contains(tile))
    {
      // clear lines from map
      clearMapLines();
      // get which hexes can be reached
      System.Collections.Generic.Dictionary<HexLocation, PathToHex> reachableHexes = ReachableHexes(tile, movement); ;

      // draw lines for all reachable hexes
      foreach (HexLocation hex in reachableHexes.Keys)
        _mapLines.Add(ConnectHex(reachableHexes[hex].pathToHex, hex));

      // return dictionary of reachable hexes
      return reachableHexes;
    }
    return new System.Collections.Generic.Dictionary<HexLocation, PathToHex>();
  }

  public int GetCharacterID(Vector2 worldLocation)
  {
    HexLocation tile = WorldToOddQ(worldLocation);
    return GetCharacterID(tile);
  }

  public int GetCharacterID(HexLocation tile)
  {
    // 0 is the ID for no character on a tile 
    return _map.GetHexTile(tile).characterID;
  }

  private System.Collections.Generic.Dictionary<HexLocation, PathToHex> ReachableHexes(HexLocation hex, int range)
  {
    // initialize data storage for dijkstra
    SimplePriorityQueue<HexLocation> frontier = new SimplePriorityQueue<HexLocation>();
    // needed to write out whole namespace since Godot.Dictionary also exists
    System.Collections.Generic.Dictionary<HexLocation, PathToHex> hexPathInfo =
      new System.Collections.Generic.Dictionary<HexLocation, PathToHex>();

    // queue start hex
    frontier.Enqueue(hex, 0);

    HexLocation current;
    while (frontier.Count != 0)
    {
      current = frontier.Dequeue();
      foreach (HexLocation next in GetNeighbors(current))
      {
        int newCost = _map.GetHexTile(next).movement;
        if (hexPathInfo.ContainsKey(current))
          newCost += hexPathInfo[current].costToHex;

        // if the cost of going to the new tile is more than the range, end here for that path
        if (newCost > range)
          continue;
        else if (!hexPathInfo.ContainsKey(next) || newCost < hexPathInfo[next].costToHex)
        {
          // store path to next hex and cost to path 
          hexPathInfo[next] = new PathToHex(current, newCost);
          frontier.Enqueue(next, newCost);
        }
      }
    }
    return hexPathInfo;
  }

  private void HighlightTile(HexLocation tile)
  {
    SetCell(tile.x, tile.y, 3, true, false, true);
  }

  public void clearMapLines()
  {
    for (int i = 0; i < _mapLines.Count; i++)
    {
      _mapLines[i].QueueFree();
    }
    _mapLines.RemoveRange(0, _mapLines.Count);
  }

  public Vector2 OddQToWorld(HexLocation index)
  {
    return MapToWorld(Util.HexLocToVec2(index)) + Util.HexLocToVec2(_hexDimensionsPx) / 2f;
  }

  public HexLocation WorldToOddQ(Vector2 location)
  {
    return Util.CubeToOddQ(WorldToCube(location));
  }

  private CubeHexLocation WorldToCube(Vector2 location)
  {
    // need to shift the origin from the screen origin to the tilemap origin
    // this is the center of the [0,0] tile
    location.x = location.x - _hexDimensionsPx.x / 2f;
    location.y = location.y - _hexDimensionsPx.y / 2f;

    float q = (2f / 3f * location.x) / _hexSizePx;
    float r = (-1f / 3f * location.x + Mathf.Sqrt(3f) / 3f * location.y) / _hexSizePx;
    Vector3 cubeLocationRaw = new Vector3(q, -q - r, r);
    return Util.CubeRound(cubeLocationRaw);
  }

  private List<HexLocation> GetNeighbors(HexLocation location)
  {
    List<CubeHexLocation> neighborsCube = GetNeighborsCube(Util.OddQToCube(location));
    List<HexLocation> result = new List<HexLocation>(neighborsCube.Count);
    for (int i = 0; i < neighborsCube.Count; i++)
      result.Insert(i, Util.CubeToOddQ(neighborsCube[i]));

    return result;
  }

  private List<CubeHexLocation> GetNeighborsCube(CubeHexLocation cubeLocation)
  {
    // the 6 directions you can travel for a given hex in cube coordinates
    List<CubeHexLocation> cubeDirections = new List<CubeHexLocation>(6);
    cubeDirections.Add(new CubeHexLocation(1, -1, 0));
    cubeDirections.Add(new CubeHexLocation(1, 0, -1));
    cubeDirections.Add(new CubeHexLocation(0, 1, -1));
    cubeDirections.Add(new CubeHexLocation(-1, 1, 0));
    cubeDirections.Add(new CubeHexLocation(-1, 0, 1));
    cubeDirections.Add(new CubeHexLocation(0, -1, 1));

    // maybe there is a cleaner way to do this???
    List<CubeHexLocation> result = new List<CubeHexLocation>(cubeDirections.Count);
    for (int i = 0; i < 6; i++)
    {
      CubeHexLocation potLocation = cubeDirections[i] + cubeLocation;
      if (_map.Contains(potLocation))
        result.Add(potLocation);
    }
    return result;
  }

  private void PrintText(string text, Vector2 position)
  {
    Label label = new Label();
    label.Text = text;
    label.SetPosition(position);
    AddChild(label);
  }

  public Line2D ConnectHex(HexLocation hex1, HexLocation hex2)
  {
    Line2D line = new Line2D();
    line.AddPoint(OddQToWorld(hex1));
    line.AddPoint(OddQToWorld(hex2));
    AddChild(line);
    return line;
  }
}
