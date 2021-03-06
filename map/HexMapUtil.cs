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

    public static int HexDistance(CubeHexLocation a, CubeHexLocation b)
    {
      return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
    }

    public static int HexDistance(HexLocation a, HexLocation b)
    {
      CubeHexLocation aCube = OddQToCube(a);
      CubeHexLocation bCube = OddQToCube(b);
      return HexDistance(aCube, bCube);
    }

    public static float GetTravelAngleDeg(HexLocation origin, HexLocation hex1, HexLocation hex2)
    {
      // convert to cube
      CubeHexLocation originCube = Util.OddQToCube(origin);
      CubeHexLocation hex1Cube = Util.OddQToCube(hex1);
      CubeHexLocation hex2Cube = Util.OddQToCube(hex2);

      // get angle between cube vectors
      Vector3 origin2Hex1 = hex1Cube.vector3 - originCube.vector3;
      Vector3 origin2Hex2 = hex2Cube.vector3 - originCube.vector3;
      return Mathf.Deg2Rad(origin2Hex1.AngleTo(origin2Hex2));
    }

    public static void PrintHexList(List<HexLocation> hexList)
    {
      if (hexList.Count == 0)
        GD.Print("[]");
      else
      {
        string result = "[ ";
        foreach (HexLocation hex in hexList)
        {
          result += hex.ToString() + ", ";
        }
        result = result.Substring(0, result.Length - 2);
        result += " ]";
        GD.Print(result);
      }
    }
  }
}