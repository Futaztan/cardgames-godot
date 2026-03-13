using Godot;
using System;

public class LorumSettings : BaseSettings
{
	public LorumSettings(int s = 20, int i = 0) { score = s;  optionIndex = i; }
	public int score;
	public int optionIndex;
}
