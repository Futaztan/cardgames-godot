using Godot;


[GlobalClass]
public partial class Cell : TextureRect
{
	private int value = -1;
	[Export]
	private Texture2D originalTexture;

	public void setDatas(PlayerCard card)
	{
		value = card.getValue();
		Texture = card.getTexture();

	}
	public void setDatas(int _value, Texture2D _text)
	{
		value = _value;
		Texture = _text;

	}
	public void resetCell()
	{
		value = -1;
		Texture = originalTexture;
		
	}
	public int getValue() { return value; }
	

}

 
