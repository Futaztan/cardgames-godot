using System;
using System.Threading.Tasks;
using cardgames.Games.Lorum.Scripts.Cards;
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

        public async Task<int> StartRound()
        {
            Random random = new Random();
            int whichCard = random.Next(0, 8); //TODO OUTOFINDEX? hiba volt 1x?

            int value = CardNodes[whichCard].getValue();
            Texture2D texture = CardNodes[whichCard].getTexture();
            Cell cell = Lorum.CenterCells[WhichCell(value)];

            BackCard playedCard = (BackCard)CardNodes[whichCard];


            PlayCardSound();
            Tween tween = playedCard.Animate(_name, cell);
            await cell.ToSignal(tween, Tween.SignalName.Finished);
            cell.setDatas(value, texture);
            CardNodes.Remove(playedCard);
            await Task.Delay(WaitMillisAfterCardPlace);
            playedCard.deleteCard();
            return value;
        }

        public async Task<int> NormalRound()
        {
            for (int i = 0; i < CardNodes.Count; i++)
            {
                int value = CardNodes[i].getValue();
                Cell cell = Lorum.CenterCells[WhichCell(value)];
                if (IsPlaceable(value, cell))
                {
                    Texture2D texture = CardNodes[i].getTexture();
                    BackCard playedCard = (BackCard)CardNodes[i];

                    PlayCardSound();
                    Tween tween = playedCard.Animate(_name, cell);
                    await cell.ToSignal(tween, Tween.SignalName.Finished);
                    cell.setDatas(value, texture);
                    CardNodes.Remove(playedCard);
                    await Task.Delay(WaitMillisAfterCardPlace);
                    playedCard.deleteCard();
                    return CardNodes.Count - 1;
                }
            }

            GD.Print("bot passz");
            await PassTurn();
            return -1;
        }
        

        public override async Task PassTurn()
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

            await _passIcon.MoveTo(pos);
        }
    }
}