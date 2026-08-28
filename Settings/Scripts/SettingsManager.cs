using Godot;

namespace cardgames.Settings.Scripts;

public class SettingsManager
{
    private const  string Path = "user://settings.cfg";
    private ConfigFile _configFile = new ConfigFile();
    public SettingsValues LoadSettings()
    {
        _configFile.Load(Path);
        string globalName = (string)_configFile.GetValue("Global", "Name", "Játékos");
        int lorumPoints = (int)_configFile.GetValue("Lorum", "Points", 10);
        int lorumLengthIndex = (int)_configFile.GetValue("Lorum", "GameLength", 0);
        return new  SettingsValues(globalName, lorumPoints, lorumLengthIndex);
    }

    public void SaveSettings(SettingsValues values)
    {
        _configFile.SetValue("Global", "Name", values.GlobalName);
        _configFile.SetValue("Lorum", "Points", values.LorumPoints);
        _configFile.SetValue("Lorum", "GameLength", values.LorumLengthIndex);
        
        Error err = _configFile.Save(Path);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Nem sikerült elmenteni a beállításokat: {err}");
        }
        else GD.Print("Sikeres mentés");
        
        
    }
}