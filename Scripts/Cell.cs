using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Cell : TextureRect
{
	private int value = -1;

	public void setDatas(Card card)
	{
		value = card.getValue();
		Texture = card.getTexture();

	}
	public void setDatas(int _value, Texture2D _text)
	{
		value = _value;
		Texture = _text;

	}
	public int getValue() { return value; }
	

}

 
