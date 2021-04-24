using System.Collections.Generic;

namespace HexMapUtil
{
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
      if (Contains(location))
        return this[location.x][location.y];
      else
        return null;
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
}
