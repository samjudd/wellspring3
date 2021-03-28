using Godot;
using System.Collections.Generic;

namespace HexMapUtil
{
  public struct HexTile
  {
    public int ID;
    public HexLocation location;
    public CubeHexLocation cubeLocation;
    public int movement;
    public HexTile(int ID, Vector2 location, int movement)
    {
      this.ID = ID;
      this.location = new HexLocation(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y));
      this.cubeLocation = Util.OddQToCube(new HexLocation(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y)));
      this.movement = movement;
    }
    public HexTile(int ID, HexLocation location, int movement)
    {
      this.ID = ID;
      this.location = location;
      this.cubeLocation = Util.OddQToCube(location);
      this.movement = movement;
    }
    public HexTile(int ID, int x, int y, int movement)
    {
      this.ID = ID;
      this.location = new HexLocation(x, y);
      this.cubeLocation = Util.OddQToCube(new HexLocation(x, y));
      this.movement = movement;
    }
  }

  public struct HexLocation
  {
    public int x, y;
    public HexLocation(int x, int y)
    {
      this.x = x;
      this.y = y;
    }
    public HexLocation(Vector2 vec)
    {
      this.x = Mathf.RoundToInt(vec.x);
      this.y = Mathf.RoundToInt(vec.y);
    }
  }

  public struct CubeHexLocation
  {
    public int x, y, z;
    public CubeHexLocation(int x, int y, int z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
    }
    public CubeHexLocation(float x, float y, float z)
    {
      this.x = Mathf.RoundToInt(x);
      this.y = Mathf.RoundToInt(y);
      this.z = Mathf.RoundToInt(z);
    }
    public CubeHexLocation(Vector3 vec)
    {
      this.x = Mathf.RoundToInt(vec.x);
      this.y = Mathf.RoundToInt(vec.y);
      this.z = Mathf.RoundToInt(vec.z);
    }

    public static CubeHexLocation operator +(CubeHexLocation a, CubeHexLocation b)
      => new CubeHexLocation(a.x + b.x, a.y + b.y, a.z + b.z);
  }

  public struct PathToHex
  {
    public HexLocation pathToHex { get; set; }
    public int costToHex { get; set; }
    public PathToHex(HexLocation pathToHex, int costToHex)
    {
      this.pathToHex = pathToHex;
      this.costToHex = costToHex;
    }
  }

  public class HexMap : List<List<HexTile>>
  {
    public HexMap(int x, int y)
    {
      Capacity = x;
      for (int i = 0; i < x; i++)
      {
        this.Add(new List<HexTile>(y));
        for (int j = 0; j < y; j++)
          this[i].Add(new HexTile(-1, x, y, 1000));
      }
    }

    public HexTile GetHexTile(HexLocation location)
    {
      return this[location.x][location.y];
    }

    public void SetHexTile(HexLocation location, HexTile tile)
    {
      this[location.x][location.y] = tile;
    }

    public HexTile GetHexTile(CubeHexLocation location)
    {
      return GetHexTile(Util.CubeToOddQ(location));
    }

    public bool Contains(CubeHexLocation tile)
    {
      return Contains(Util.CubeToOddQ(tile));
    }

    public bool Contains(HexTile tile)
    {
      return Contains(tile.location);
    }

    public bool Contains(HexLocation location)
    {
      int x = location.x;
      int y = location.y;

      // no negative locations
      if (x < 0 || y < 0)
        return false;

      // check that the index exists in both cases and that the tile exists (ID -1 is no tile)
      return this.Capacity > x && this[x].Capacity > y && this[x][y].ID != -1;
    }
  }

  public static class Util
  {
    public static System.Collections.Generic.Dictionary<int, int> TileIDToMovement =
      new Dictionary<int, int>()
      {
        // {tile index in godot, movement cost}
        {-1, 1000}, // no tile 
        {0, 2},     // basic grass tile
        {2, 2},     // selected grass tile
        {3, 2},     // movement highlighted grass tile
        {4, 4}      // forest tile 
      };

    public static Vector2 HexLocToVec2(HexLocation location)
    {
      return new Vector2(location.x, location.y);
    }

    public static CubeHexLocation OddQToCube(HexLocation oddQCoordinates)
    {
      int col = oddQCoordinates.x;
      int row = oddQCoordinates.y;

      int x = col;
      int z = row - (col - (col & 1)) / 2;
      int y = -x - z;
      return new CubeHexLocation(x, y, z);
    }

    public static HexLocation CubeToOddQ(CubeHexLocation cubeCoordinates)
    {
      int x = cubeCoordinates.x;
      int y = cubeCoordinates.y;
      int z = cubeCoordinates.z;

      int row = z + (x - (x & 1)) / 2;
      return new HexLocation(x, row);
    }

    public static CubeHexLocation CubeRound(Vector3 cubeLocationRaw)
    {
      float rx = Mathf.Round(cubeLocationRaw.x);
      float ry = Mathf.Round(cubeLocationRaw.y);
      float rz = Mathf.Round(cubeLocationRaw.z);

      float xDiff = Mathf.Abs(rx - cubeLocationRaw.x);
      float yDiff = Mathf.Abs(ry - cubeLocationRaw.y);
      float zDiff = Mathf.Abs(rz - cubeLocationRaw.z);

      if (xDiff > yDiff && xDiff > zDiff)
        rx = -ry - rz;
      else if (yDiff > zDiff)
        ry = -rx - rz;
      else
        rz = -rx - ry;

      return new CubeHexLocation(rx, ry, rz);
    }
  }
}