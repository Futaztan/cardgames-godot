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
        int lorumPoints = (int)_configFile.GetValue("Lorum", "Points", 10);
        int lorumLength = (int)_configFile.GetValue("Lorum", "GameLength", 0);
        return new  SettingsValues(globalName, lorumPoints, lorumLength);
    }
    
    

    public void SaveSettings(SettingsValues values)
    {
        _configFile.SetValue("Global", "Name", values.GlobalName);
        _configFile.SetValue("Lorum", "Points", values.LorumPoints);
        _configFile.SetValue("Lorum", "GameLength", values.LorumLength);
        Error err = _configFile.Save(Path);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Nem sikerült elmenteni a beállításokat: {err}");
        }
        else GD.Print("Sikeres mentés");
        
        
    }
}