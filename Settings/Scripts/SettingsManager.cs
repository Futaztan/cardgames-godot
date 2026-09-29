using cardgames.Settings.Scripts.Models;
using Godot;

namespace cardgames.Settings.Scripts;

public class SettingsManager
{
    private const  string Path = "user://settings.cfg";
    private readonly ConfigFile _configFile = new ConfigFile();

    public static SettingsManager Instance { get; } = new();
    
    private SettingsManager() { }
    public SettingsValues LoadSettings()
    {
        _configFile.Load(Path);
        string globalName = (string)_configFile.GetValue("Global", "Name", "Játékos");
        var globalSettings = new GlobalSettings(globalName);
        int lorumPoints = (int)_configFile.GetValue("Lorum", "Points", 20);
        int lorumLength = (int)_configFile.GetValue("Lorum", "GameLength", -1);
        var lorumSettings = new LorumSettings(lorumPoints, lorumLength);
        int _21Money = (int)_configFile.GetValue("21", "Money", 20);
        var _21Settings = new _21Settings(_21Money);
        return new  SettingsValues(globalSettings, lorumSettings, _21Settings);
    }
    
    

    public void SaveSettings(SettingsValues values)
    {
        _configFile.SetValue("Global", "Name", values.GlobalSettings.GlobalName);
        _configFile.SetValue("Lorum", "Points", values.LorumSettings.Money);
        _configFile.SetValue("Lorum", "GameLength", values.LorumSettings.Length);
        _configFile.SetValue("21","Money", values._21Settings.Money);
        Error err = _configFile.Save(Path);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Nem sikerült elmenteni a beállításokat: {err}");
        }
        else GD.Print("Sikeres mentés");
        
        
    }
}