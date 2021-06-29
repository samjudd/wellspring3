using Godot;
using System.Collections.Generic;

namespace HexMapUtil
{
  static class Constants
  {
    public const int NOCHARACTER = -1;

    public const int NOTILE = -1;

    public static System.Collections.Generic.Dictionary<int, int> TileIDToMovement = new Dictionary<int, int>()
    {
      // {tile index in godot tileset, movement cost}
      {-1, 1000}, // no tile 
      {0, 2},     // basic grass tile
      {1, 2},     // movement range grass tile
      {2, 2},     // attack range grass tile
      {3, 4},     // basic forest tile
      {4, 4},     // movement range forest tile
      {5, 4}      // attack range forest tile
    };

    public static List<CubeHexLocation> CubeUnitVectors = new List<CubeHexLocation>()
    {
      new CubeHexLocation(1, -1, 0),
      new CubeHexLocation(1, 0, -1),
      new CubeHexLocation(0, 1, -1),
      new CubeHexLocation(-1, 1, 0),
      new CubeHexLocation(-1, 0, 1),
      new CubeHexLocation(0, -1, 1)
    };
  }

  public struct HexLocation
  {
    public int x, y;
    public Vector2 vector2;
    public HexLocation(int x, int y)
    {
      this.x = x;
      this.y = y;
      this.vector2 = new Vector2(x, y);
    }
    public HexLocation(Vector2 vec)
    {
      this.x = Mathf.RoundToInt(vec.x);
      this.y = Mathf.RoundToInt(vec.y);
      this.vector2 = new Vector2(this.x, this.y);
    }
    public static bool operator ==(HexLocation a, HexLocation b)
    {
      return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(HexLocation a, HexLocation b)
    {
      return !(a == b);
    }

    public override bool Equals(object obj)
    {
      if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        return false;
      else
      {
        HexLocation hl = (HexLocation)obj;
        return hl == this;
      }
    }

    public override int GetHashCode()
    {
      return (this.x, this.y).GetHashCode();
    }

    public override string ToString()
    {
      return "[ " + x.ToString() + ", " + y.ToString() + " ]";
    }
  }

  public struct CubeHexLocation
  {
    public int x, y, z;
    public Vector3 vector3;
    public CubeHexLocation(int x, int y, int z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.vector3 = new Vector3(x, y, z);
    }
    public CubeHexLocation(float x, float y, float z)
    {
      this.x = Mathf.RoundToInt(x);
      this.y = Mathf.RoundToInt(y);
      this.z = Mathf.RoundToInt(z);
      this.vector3 = new Vector3(this.x, this.y, this.z);
    }
    public CubeHexLocation(Vector3 vec)
    {
      this.x = Mathf.RoundToInt(vec.x);
      this.y = Mathf.RoundToInt(vec.y);
      this.z = Mathf.RoundToInt(vec.z);
      this.vector3 = new Vector3(this.x, this.y, this.z);
    }

    public static CubeHexLocation operator +(CubeHexLocation a, CubeHexLocation b)
      => new CubeHexLocation(a.x + b.x, a.y + b.y, a.z + b.z);

    public static CubeHexLocation operator *(CubeHexLocation a, int b)
      => new CubeHexLocation(a.x * b, a.y * b, a.z * b);

    public override string ToString()
    {
      return "[ " + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + " ]";
    }
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
}
