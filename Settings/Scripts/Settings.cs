using Godot;
using System;
using cardgames.Settings.Scripts;

public partial class Settings : Control
{
    [Export] private PackedScene MainMenuScene { get; set; }
    [Export] private LineEdit Global_NameUI {get; set;}
    [Export] private SpinBox Lorum_PointsUI {get; set;}
    [Export] private OptionButton Lorum_GameLengthUI {get; set;}
    private readonly SettingsManager _settingsManager = SettingsManager.Instance;

    public override void _Ready()
    {
        SettingsValues values = _settingsManager.LoadSettings();
        Global_NameUI.Text = values.GlobalName;
        Lorum_PointsUI.Value = values.LorumPoints;
        Lorum_GameLengthUI.Selected = Lorum_GameLengthUI.GetItemIndex(values.LorumLength);
    }

    private void OnSaveButtonPressed()
    {
        _settingsManager.SaveSettings(GetCurrentSettings());
        GetTree().ChangeSceneToPacked(MainMenuScene);
    }

    private SettingsValues GetCurrentSettings()
    {
        return new SettingsValues(Global_NameUI.Text, (int) Lorum_PointsUI.Value, Lorum_GameLengthUI.GetSelectedId());
    }
}
