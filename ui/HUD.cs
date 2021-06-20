using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class HUD : CanvasLayer
{
  // references to all the UI elements
  private Label _turnCounter;
  private TextureProgress _hpBar;
  private Label _hpLabel;
  private TextureProgress _apBar;
  private Label _apLabel;

  public override void _Ready()
  {
    _turnCounter = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/TurnCounterCenter/TurnCounterLabel");
    _hpBar = GetNode<TextureProgress>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/HitPointsHBox/CenterContainer/HPProgress");
    _hpLabel = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/HitPointsHBox/CenterContainer/CurrentHPLabel");
    _apBar = GetNode<TextureProgress>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/ActionPointsHBox/CenterContainer/APProgress");
    _apLabel = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/ActionPointsHBox/CenterContainer/CurrentAPLabel");
  }

  public void UpdateTurn(int turn, string letter)
  {
    int dispTurn = (int)Mathf.Ceil(turn / 2.0f);
    _turnCounter.Text = "Turn: " + dispTurn.ToString() + letter;
  }

  public void UpdateHP(int HP)
  {
    // textureProgress will clamp if HP > maxHP
    _hpBar.Value = HP;
    _hpLabel.Text = HP.ToString() + "/" + _hpBar.MaxValue.ToString();
  }

  public void UpdateAP(int AP)
  {
    // textureProgress will clamp if AP > maxAP
    _apBar.Value = AP;
    _apLabel.Text = AP.ToString() + "/" + _apBar.MaxValue.ToString();
  }

  public void OnSelectCharacter(Character character)
  {
    // update HP bar 
    _hpBar.MaxValue = character._maxHP;
    UpdateHP(character.HP);
    UpdateAP(character.AP);
  }
}
