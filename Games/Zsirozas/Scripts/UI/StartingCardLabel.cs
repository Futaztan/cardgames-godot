using System;
using System.Collections.Generic;
using cardgames.Games.Zsirozas.Scripts.Players;
using Godot;

namespace cardgames.Games.Zsirozas.Scripts.UI;
public partial class StartingCardLabel : RichTextLabel
{

	private List<string> _imagePaths = new List<string>
	{
		"res://Assets/Icons/zold-icon@small.png",
		"res://Assets/Icons/piros-icon@small.png",
		"res://Assets/Icons/makk-icon@small.png",
		"res://Assets/Icons/tok-icon@small.png"
	};
	private List<string> _values = new List<string>
	{
		"ALSÓ",
		"FELSŐ",
		"KIRÁLY",
		"ÁSZ",
		"VII",
		"VIII",
		"IX",
		"X"
	};
	public void setText(EntityBase startingPlayer, int startingCardValue)
	{
		int path = whichCell(startingCardValue);
		string image = "[img]" + _imagePaths[path] + "[/img]";
		string txt = _values[(startingCardValue%10) - 1];
		this.Text = "Kezdőlap:\n" +startingPlayer.Name +  "\n" +image + " " + txt;
	}
	public void removeText()
	{
		this.Text = "";
	}

	private int whichCell(int value)
	{
		if (value >= 1 && value <= 8) return 0;
		else if (value >= 11 && value <= 18) return 1;
		else if (value >= 21 && value <= 28) return 2;
		else if (value >= 31 && value <= 38) return 3;
		else throw new ArgumentException("nem ide valo value");
	}
}
