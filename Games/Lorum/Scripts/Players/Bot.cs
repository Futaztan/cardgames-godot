using System;
using cardgames.Lorum.Scripts.UI;
using Godot;

namespace cardgames.Games.Lorum.Scripts.Players
{
    public class Bot : EntityBase
    {
        public Bot(string name, int score, RichTextLabel label, CardContainer container, Pass passIcon) : base(name, score, label,
            container, passIcon)
        {
        }

        public int StartRound()
        {
            Random random = new Random();
            int whichCard = random.Next(0, 8);

            int value = CardNodes[whichCard].getValue();
            Texture2D texture = CardNodes[whichCard].getTexture();
            Cell cell = Games.Lorum.Scripts.Lorum.CenterCells[WhichCell(value)];

            BackCard playedCard = (BackCard)CardNodes[whichCard];


            PlayCardSound();
            playedCard.Animate(_name, cell, () =>
            {
                cell.setDatas(value, texture);
                CardNodes.Remove(playedCard);
                playedCard.deleteCard();
            });

            return value;
        }

        public int NormalRound()
        {
            for (int i = 0; i < CardNodes.Count; i++)
            {
                int value = CardNodes[i].getValue();
                Texture2D texture = CardNodes[i].getTexture();
                Cell cell = Games.Lorum.Scripts.Lorum.CenterCells[WhichCell(value)];


                if (IsPlaceable(value, cell))
                {
                    BackCard playedCard = (BackCard)CardNodes[i];

                    PlayCardSound();
                    playedCard.Animate(_name, cell, () =>
                    {
                        cell.setDatas(value, texture);
                        CardNodes.Remove(playedCard);
                        playedCard.deleteCard();
                    });


                    return CardNodes.Count - 1;
                }
            }

            GD.Print("bot passz");
            PassTurn();
            return -1;
        }

        private void PassTurn()
        {
            BoxContainer box = (BoxContainer)CardNodes[0].GetParent();
            Vector2 pos;
            switch (_name)
            {
                case "bot1":
                    pos = box.GlobalPosition +
                          new Vector2(box.Size.X, box.Size.Y * 0.5f - _passIcon.Size.Y * 0.5f); break;
                case "bot2":
                    pos = box.GlobalPosition +
                          new Vector2(box.Size.X * 0.5f - _passIcon.Size.Y * 0.5f, box.Size.Y); break;
                case "bot3":
                    pos = box.GlobalPosition + new Vector2(0, box.Size.Y * 0.5f - _passIcon.Size.Y * 0.5f); break;
                default: throw new Exception();
            }

            _passIcon.moveTo(pos);
        }
    }
}