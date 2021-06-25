using Godot;
using System.Collections.Generic;

public class PointAttribute
{
  public int value
  {
    get { return _currentValue; }
    set { _currentValue = Mathf.Clamp(value, 0, _maxValue); }
  }

  public int maxValue
  {
    get { return _maxValue; }
  }

  private int _baseValue = 0;
  private int _currentValue = 0;
  private int _maxValue = 0;

  private Dictionary<string, int> _flatModifiers = new Dictionary<string, int>();
  private Dictionary<string, int> _percentModifiers = new Dictionary<string, int>();

  public PointAttribute(int baseValue, int currentValue)
  {
    _baseValue = baseValue;
    _currentValue = currentValue;
    _maxValue = baseValue;
  }

  public PointAttribute(int baseValue)
  {
    _baseValue = baseValue;
    _currentValue = baseValue;
    _maxValue = baseValue;
  }

  private void UpdateMaxValue()
  {
    float percentModifier = 0;
    foreach (int value in _percentModifiers.Values)
      percentModifier *= value / 10.0f + 1.0f;

    int flatModifier = 0;
    foreach (int value in _flatModifiers.Values)
      flatModifier += value;

    int newMaxValue = Mathf.RoundToInt(_baseValue * percentModifier) + flatModifier;

    // if max value increased, add the increase to the current value
    _currentValue += Mathf.Max(newMaxValue, 0);
    // since the max changed re-clamp the current value
    _currentValue = Mathf.Clamp(_currentValue, 0, newMaxValue);
    // change the max value
    _maxValue = newMaxValue;
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