using Godot;
using System;
using System.Linq;
using cardgames.Settings.Scripts;
using cardgames.Settings.Scripts.Models;

public partial class Settings : Control
{
    [Export] private PackedScene MainMenuScene { get; set; }
    [Export] private LineEdit Global_NameUI {get; set;}
    
    [Export] private HBoxContainer Lorum_MoneyContainerUI { get; set; }
    [Export] private HBoxContainer Lorum_GameLengthContainerUI { get; set; }
    [Export] private HBoxContainer _21_MoneyContainerUI { get; set; }
    private readonly SettingsManager _settingsManager = SettingsManager.Instance;

    public override void _Ready()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        SettingsValues values = _settingsManager.LoadSettings();
        Global_NameUI.Text = values.GlobalSettings.GlobalName;
        LoadLorumSettingsToUI(values);
        Load21SettingsToUI(values);
    }

    private void LoadLorumSettingsToUI(SettingsValues values)
    {
        foreach (Button button in Lorum_GameLengthContainerUI.GetChildren().OfType<Button>())
        {
            if (button.GetMeta("game_length").AsInt32() == values.LorumSettings.Length)
            {
                button.SetPressed(true);
            }
        }
        
        foreach (Button button in Lorum_MoneyContainerUI.GetChildren().OfType<Button>())
        {
            if (button.GetMeta("starter_money").AsInt32() == values.LorumSettings.Money)
            {
                button.SetPressed(true);
            }
        }
    }
    private void Load21SettingsToUI(SettingsValues values)
    {
        foreach (Button button in _21_MoneyContainerUI.GetChildren())
        {
            if (button.GetMeta("starter_money").AsInt32() == values._21Settings.Money)
            {
                button.SetPressed(true);
            }
        }
    }

    private void OnSaveButtonPressed()
    {
        _settingsManager.SaveSettings(GetCurrentSettings());
        GetTree().ChangeSceneToPacked(MainMenuScene);
    }

    private SettingsValues GetCurrentSettings()
    {
        var global = new GlobalSettings(Global_NameUI.Text);
        var lorum = GetLorumSettingsFromUI();
        var _21 = Get21SettingsFromUI();
        return new SettingsValues(global,lorum,_21);
    }

    private LorumSettings GetLorumSettingsFromUI()
    {
       
        int length = -1;
        foreach (var button in Lorum_GameLengthContainerUI.GetChildren().OfType<Button>())
        {
            if (button.IsPressed())
            {
                length = button.GetMeta("game_length").AsInt32();
            }
        }
        int money = 20;
        foreach (var button in Lorum_MoneyContainerUI.GetChildren().OfType<Button>())
        {
            if (button.IsPressed())
            {
                money = button.GetMeta("starter_money").AsInt32();
            }
        }

        return new LorumSettings(money, length);
    }

    private _21Settings Get21SettingsFromUI()
    {
        int money = 20;
        foreach (Button button in _21_MoneyContainerUI.GetChildren().OfType<Button>())
        {
            if (button.IsPressed())
            {
                money = button.GetMeta("starter_money").AsInt32();
                
            }
        }
        return new _21Settings(money);
    }
}
