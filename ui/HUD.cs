using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class HUD : CanvasLayer
{
  // references to all the UI elements
  private Label _turnCounter;
  private TextureProgress _hpBar;
  private Label _hpLabel;
  private List<TextureProgress> _actionPoints = new List<TextureProgress>();

  public override void _Ready()
  {
    _turnCounter = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/TurnCounterCenter/TurnCounterLabel");
    _hpBar = GetNode<TextureProgress>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/HitPointsHBox/CenterContainer/HPProgress");
    _hpLabel = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/HitPointsHBox/CenterContainer/CurrentHPLabel");
    HBoxContainer actionPointsCont = GetNode<HBoxContainer>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/ActionPointsHBox");
    Array actionPoints = actionPointsCont.GetChildren();
    // start at 1 since index 0 should always be label
    for (int i = 1; i < actionPoints.Count; i++)
      _actionPoints.Add((TextureProgress)actionPoints[i]);
  }

  public void UpdateTurn(int turn)
  {
    _turnCounter.Text = "Turn: " + turn.ToString();
  }

  public void UpdateHP(int HP)
  {
    // textureProgress will clamp if HP > maxHP
    _hpBar.Value = HP;
    _hpLabel.Text = HP.ToString() + "/" + _hpBar.MaxValue.ToString();
  }

  public void UpdateAP(int AP)
  {
    for (int i = 0; i < _actionPoints.Count; i++)
    {
      if (i < AP)
        _actionPoints[i].Value = 1;
      else
        _actionPoints[i].Value = 0;
    }
  }

  public void OnSelectCharacter(Character character)
  {
    // update HP bar 
    _hpBar.MaxValue = character._maxHP;
    UpdateHP(character.HP);
    UpdateAP(character.AP);
  }
}
