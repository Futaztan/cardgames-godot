using Godot;
using System;
using cardgames.Settings.Scripts;

public partial class Settings : Control
{
    [Export] private PackedScene MainMenuScene { get; set; }
    [Export] private LineEdit Global_Name {get; set;}
    [Export] private SpinBox Lorum_Points {get; set;}
    [Export] private OptionButton Lorum_GameLength {get; set;}
    private readonly SettingsManager _settingsManager = SettingsManager.Instance;

    public override void _Ready()
    {
        SettingsValues values = _settingsManager.LoadSettings();
        Global_Name.Text = values.GlobalName;
        Lorum_Points.Value = values.LorumPoints;
        Lorum_GameLength.Selected = values.LorumLength;
    }

    private void OnSaveButtonPressed()
    {
        _settingsManager.SaveSettings(GetCurrentSettings());
        GetTree().ChangeSceneToPacked(MainMenuScene);
    }

    private SettingsValues GetCurrentSettings()
    {
        return new SettingsValues(Global_Name.Text, (int) Lorum_Points.Value, Lorum_GameLength.Selected);
    }
}
