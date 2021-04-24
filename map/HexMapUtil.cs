using Godot;
using System.Collections.Generic;

namespace HexMapUtil
{
  public static class Util
  {
    public static List<HexLocation> HexPathfind(HexLocation start, HexLocation destination, System.Collections.Generic.Dictionary<HexLocation, PathToHex> hexPaths)
    {
      if (hexPaths.ContainsKey(destination))
      {
        HexLocation current = destination;
        List<HexLocation> path = new List<HexLocation>();
        path.Add(destination);
        // trace path back from destination to start hex 
        while (hexPaths[current].pathToHex != start)
        {
          current = hexPaths[current].pathToHex;
          path.Add(current);
        }
        // reverse path so it goes from start to destination
        path.Reverse();
        return path;
      }
      else
        return null;
    }

    public static int GetPathCost(List<HexLocation> path, System.Collections.Generic.Dictionary<HexLocation, PathToHex> hexPaths)
    {
      int cost = 0;
      foreach (HexLocation hex in path)
      {
        cost += hexPaths[hex].costToHex;
      }
      return cost;
    }

    public static int GetPathCost(HexLocation start, HexLocation destination, System.Collections.Generic.Dictionary<HexLocation, PathToHex> hexPaths)
    {
      List<HexLocation> path = HexPathfind(start, destination, hexPaths);
      return GetPathCost(path, hexPaths);
    }

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