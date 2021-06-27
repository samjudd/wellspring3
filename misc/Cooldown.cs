using Godot;
using System;

public class Cooldown : Node
{
  public int timeLeft
  {
    get { return _timeLeft; }
  }

  public int length
  {
    get { return _length; }
    set { _length = value; }
  }

  private int _timeLeft = 1;
  private int _length = 1;

  public Cooldown(int baseLength)
  {
    _length = baseLength;
    _timeLeft = 0;
  }

  public bool OnCooldown()
  {
    return _timeLeft == 0;
  }

  public void OnTurnStart()
  {
    if (_timeLeft > 0)
      _timeLeft -= 1;
  }

  public void SetOnCooldown()
  {
    _timeLeft = _length;
  }

  public void SetOffCooldown()
  {
    _timeLeft = 0;
  }
}
