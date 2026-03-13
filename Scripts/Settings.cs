using Godot;
using System;
using System.Reflection.Metadata;

public class Settings
{
	private GameTypeEnum gameType;

  
	private string path;
	public Settings(GameTypeEnum type) { gameType = type; path = $"user://{gameType.ToString()}_settings.cfg"; }
	public void saveSettings(BaseSettings baseSettings)
	{
		ConfigFile configFile = new ConfigFile();
		configFile.Load(path);
		switch (gameType)
		{
			case GameTypeEnum.LORUM:
				LorumSettings settings = (LorumSettings)baseSettings;
				configFile.SetValue("gameplay", "score", settings.score);
				configFile.SetValue("gameplay", "index", settings.optionIndex);

				break;
			default: break;
		}
		configFile.Save(path);
	}

	public BaseSettings loadSettings()
	{
		ConfigFile configFile = new ConfigFile();
		if (configFile.Load(path) != Error.Ok)
		{
			GD.Print("nincs korábbi mentés");
			return new LorumSettings();
		}
		switch (gameType)
		{
			case GameTypeEnum.LORUM:
				int score = (int)configFile.GetValue("gameplay", "score", 20);
				int index = (int)configFile.GetValue("gameplay", "index", 0);
				return new LorumSettings(score, index);

			default: throw new Exception("load hiba");
		}
	}
}
