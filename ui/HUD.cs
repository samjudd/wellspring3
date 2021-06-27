using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class HUD : CanvasLayer
{
  private GameManager _gameManager;

  // references to all the UI elements
  private Label _turnCounter;
  private Label _nameLabel;
  private TextureProgress _hpBar;
  private Label _hpLabel;
  private TextureProgress _apBar;
  private Label _apLabel;
  private Label _attackLabel;
  private Label _armorLabel;
  private Label _spellPowerLabel;
  private Label _spellResistLabel;
  private Button _abilityButton;
  private Control _statsDisplay;

  public override void _Ready()
  {
    _gameManager = GetNode<GameManager>("..");
    _turnCounter = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/TurnCounterCenter/TurnCounterLabel");
    _nameLabel = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/NameHBox/Name");
    _hpBar = GetNode<TextureProgress>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/HitPointsHBox/CenterContainer/HPProgress");
    _hpLabel = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/HitPointsHBox/CenterContainer/CurrentHPLabel");
    _apBar = GetNode<TextureProgress>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/ActionPointsHBox/CenterContainer/APProgress");
    _apLabel = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/ActionPointsHBox/CenterContainer/CurrentAPLabel");
    _attackLabel = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/AttackDamageHBox/Stat");
    _armorLabel = GetNode<Label>(
    "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/ArmorHBox/Stat");
    _spellPowerLabel = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/SpellPowerHBox/Stat");
    _spellResistLabel = GetNode<Label>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/SpellResistHBox/Stat");
    _abilityButton = GetNode<Button>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats/CastAbilityButton");
    _statsDisplay = GetNode<Control>(
      "HUD/CharInfoVBox/CharInfo/MarginContainer/CharInfoVBoxInner/Stats");

    // hide stats display since nothing selected
    _statsDisplay.Visible = false;

    // connect ability button press signal
    _abilityButton.Connect("pressed", this, nameof(OnAbilityButtonPress));
  }

  public void OnAbilityButtonPress()
  {
    _gameManager.CastCharacterAbility();
  }

  public void UpdateTurn(int turn, string letter)
  {
    int dispTurn = (int)Mathf.Ceil(turn / 2.0f);
    _turnCounter.Text = "Turn: " + dispTurn.ToString() + letter;
  }

  public void UpdateName(string name)
  {
    _nameLabel.Text = name;
  }

  public void UpdateHP(int HP, int maxHP)
  {
    _hpBar.MaxValue = maxHP;
    // textureProgress will clamp if HP > maxHP
    _hpBar.Value = HP;
    _hpLabel.Text = HP.ToString() + "/" + _hpBar.MaxValue.ToString();
  }

  public void UpdateAP(int AP, int maxAP)
  {
    _apBar.MaxValue = maxAP;
    // textureProgress will clamp if AP > maxAP
    _apBar.Value = AP;
    _apLabel.Text = AP.ToString() + "/" + _apBar.MaxValue.ToString();
  }

  public void UpdateAttackDamage(int value)
  {
    _attackLabel.Text = value.ToString();
  }

  public void UpdateArmor(int value)
  {
    _armorLabel.Text = value.ToString();
  }

  public void UpdateSpellPower(int value)
  {
    _spellPowerLabel.Text = value.ToString();
  }

  public void UpdateSpellResist(int value)
  {
    _spellResistLabel.Text = value.ToString();
  }

  public void UpdateAbilityButton(int value, string abilityName)
  {
    if (value > 0)
    {
      _abilityButton.Text = value.ToString() + " turns";
      _abilityButton.Disabled = true;
    }
    else
    {
      _abilityButton.Text = abilityName;
      _abilityButton.Disabled = false;
    }
  }

  public void OnSelectCharacter(Character character)
  {
    _statsDisplay.Visible = true;
    UpdateStats(character);
  }

  public void OnDeselectCharacter()
  {
    _statsDisplay.Visible = false;
  }

  public void UpdateStats(Character character)
  {
    UpdateName(character._characterName);
    UpdateHP(character.HP.value, character.HP.maxValue);
    UpdateAP(character.AP.value, character.AP.maxValue);
    UpdateAttackDamage(character.AttackDamage.value);
    UpdateArmor(character.Armor.value);
    UpdateSpellPower(character.SpellPower.value);
    UpdateSpellResist(character.SpellResist.value);
    UpdateAbilityButton(character.AbilityCD.timeLeft, character._abilityName);
  }
}
