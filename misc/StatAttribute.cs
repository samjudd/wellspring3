using Godot;
using System.Collections.Generic;

public class StatAttribute
{
  public int value
  {
    get { return _currentValue; }
  }

  private int _baseValue = 0;
  private int _currentValue = 0;

  private Dictionary<string, int> _flatModifiers = new Dictionary<string, int>();
  private Dictionary<string, int> _percentModifiers = new Dictionary<string, int>();

  public StatAttribute(int baseValue)
  {
    _baseValue = baseValue;
    _currentValue = baseValue;
  }

  private void UpdateMaxValue()
  {
    float percentModifier = 0;
    foreach (int value in _percentModifiers.Values)
      percentModifier *= value / 10.0f + 1.0f;

    int flatModifier = 0;
    foreach (int value in _flatModifiers.Values)
      flatModifier += value;

    _currentValue = Mathf.RoundToInt(_baseValue * percentModifier) + flatModifier;
  }

  public void AddFlatModifier(string source, int value)
  {
    _flatModifiers.Add(source, value);
    UpdateMaxValue();
  }

  public void RemoveFlatModifier(string source)
  {
    _flatModifiers.Remove(source);
    UpdateMaxValue();
  }

  public void AddPercentModifier(string source, int value)
  {
    _percentModifiers.Add(source, value);
    UpdateMaxValue();
  }

  public void RemovePercentModifier(string source)
  {
    _percentModifiers.Remove(source);
    UpdateMaxValue();
  }
}
