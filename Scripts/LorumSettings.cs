using Godot;
using System;

public class LorumSettings : BaseSettings
{
    public LorumSettings(int s, int i) { score = s;  optionIndex = i; }
    public int score;
    public int optionIndex;
}
