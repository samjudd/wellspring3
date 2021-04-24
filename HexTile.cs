using Godot;

namespace HexMapUtil
{
  public class HexTile
  {
    public int ID;
    public HexLocation location;
    public CubeHexLocation cubeLocation;
    public int baseMovement;
    public int characterID;
    public int movement;

    public HexTile(int ID, HexLocation location, int baseMovement)
    {
      this.ID = ID;
      this.location = location;
      this.cubeLocation = Util.OddQToCube(location);
      this.baseMovement = baseMovement;
      this.characterID = Constants.NOCHARACTER;
      movement = baseMovement;
    }
    public HexTile(int ID, Vector2 location, int baseMovement)
    {
      this.ID = ID;
      this.location = new HexLocation(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y));
      this.cubeLocation = Util.OddQToCube(new HexLocation(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y)));
      this.baseMovement = baseMovement;
      this.characterID = Constants.NOCHARACTER;
      movement = baseMovement;
    }
    public HexTile(int ID, int x, int y, int baseMovement)
    {
      this.ID = ID;
      this.location = new HexLocation(x, y);
      this.cubeLocation = Util.OddQToCube(new HexLocation(x, y));
      this.baseMovement = baseMovement;
      this.characterID = Constants.NOCHARACTER;
      movement = baseMovement;
    }

    public void AddCharacter(int characterID)
    {
      this.characterID = characterID;
      // tile is impassable
      movement = 10000;
    }

    public void RemoveCharacter()
    {
      this.characterID = Constants.NOCHARACTER;
      movement = baseMovement;
    }
  }
}
