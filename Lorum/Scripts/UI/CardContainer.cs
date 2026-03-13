using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardContainer : BoxContainer //TODO ez a cards list ahasznaalta strb animacio
{

	[Export]
	public PackedScene CardScene; // Ide húzzátok be a Card.tscn-t a szerkesztőben


	/*
		public override void _Ready()
		{
			SpawnCards();
		}
		public void setCardList(List<CardBase> list) { _cards = list; }

		private void SpawnCards()
		{
			// Előző kártyák törlése
			foreach (var card in _cards)
			{
				card.QueueFree();
			}
			_cards.Clear();

			// 8 kártya létrehozása
			for (int i = 0; i < 8; i++)
			{
				BackCard cardInstance = (BackCard)CardScene.Instantiate();
				AddChild(cardInstance);
				_cards.Add(cardInstance);

				// opcionálisan beállítható texture:
				// ((TextureRect)cardInstance).Texture = GD.Load<Texture2D>($"res://cards/card_{i}.png");
			}
		}

		public void RemoveCard(int index)
		{
			if (index >= 0 && index < _cards.Count)
			{
				_cards[index].QueueFree();
				_cards.RemoveAt(index);
			}
		}

		public void NewRound()
		{
			SpawnCards();
		}*/
}
