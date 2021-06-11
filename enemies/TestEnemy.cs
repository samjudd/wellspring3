
public class TestEnemy : Character
{
  public override void _Ready()
  {
    _characterID = 1;
    _maxHP = 20;
    _maxAP = 8;
    _faction = GameUtil.Faction.ENEMY;
    base._Ready();
  }
}
