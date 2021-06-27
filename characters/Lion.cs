using Godot;
using System;

public class Lion : Character
{
  public override void CastAbility()
  {
    AP.value -= AbilityCost.value;
    AbilityCD.SetOnCooldown();
  }
}
