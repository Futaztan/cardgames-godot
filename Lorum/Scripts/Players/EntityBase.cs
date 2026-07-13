using System;
using System.Collections.Generic;
using System.Linq;
using cardgames.Lorum.Scripts.Cards;
using Godot;

namespace cardgames.Lorum.Scripts.Players
{
    public abstract partial class EntityBase
    {
        protected List<CardBase> CardNodes = new List<CardBase>();
        protected string _name = "placeholder";
        public string Name => _name;
        private int _score = 0;
        public int Score => _score;
        private CardContainer _cardContainer;
        public int CardsInHandCount => CardNodes.Count;


        private RichTextLabel _label;


        public EntityBase(string name, int score, RichTextLabel label, CardContainer container)
        {
            _name = name;
            _label = label;
            _score = score;
            _cardContainer = container;
            UpdateLabel();
        }

        private void UpdateLabelWithDiff(int diff)
        {
            string color;
            string diffText;
            if (diff < 0)
            {
                color = "red";
                diffText = diff.ToString();
            }
            else
            {
                color = "green";
                diffText = "+" + diff.ToString();
            }

            _label.Text = "[b]" + _name + "\n" + _score + " pont \n [color=" + color + "]" + diffText + "[/color] [/b]";
            //_label.Text = "[b]" + _name + "\n" + Score + " pont [/b]";
        }

        public void UpdateLabel()
        {
            var stylebox = new StyleBoxFlat();
            stylebox.BgColor = new Color(0.2f, 0.2f, 0.8f, 1.0f); // kék háttér
            _label.AddThemeStyleboxOverride("normal", stylebox);
            _label.Size = _label.GetMinimumSize();
            _label.Text = "[b]" + _name + "\n" + _score + " pont [/b]";
        }

        protected int WhichCell(int value)
        {
            if (value >= 1 && value <= 8) return 0;
            else if (value >= 11 && value <= 18) return 1;
            else if (value >= 21 && value <= 28) return 2;
            else if (value >= 31 && value <= 38) return 3;
            else throw new ArgumentException("nem ide valo value");
        }

        protected bool IsPlaceable(int value, Cell cell)
        {
            return value % 10 == LorumGameLogic.StartingCardValueMod || value == cell.getValue() + 1 ||
                   value % 10 == 1 && cell.getValue() % 10 == 8;
        }

        public void OnWin(int winsum)
        {
            GD.Print(_name + " nyert!");
            _score += winsum;
            UpdateLabelWithDiff(winsum);
            //Score += winsum;
        }

        public void OnLose()
        {
            _score -= CardsInHandCount;
            UpdateLabelWithDiff(-CardsInHandCount);
            //Score -= getCardsInHandCount();
        }

        public void ResetState()
        {
            CardNodes = new List<CardBase>();
            foreach (CardBase item in _cardContainer.GetChildren())
            {
                item.QueueFree();
            }
        }

        public void DealNewCards(LorumGameLogic gameLogic)
        {
            for (int i = 0; i < 8; i++)
            {
                CardBase newcard = (CardBase)_cardContainer.CardScene.Instantiate();
                _cardContainer.AddChild(newcard);
                CardNodes.Add(newcard);
                int rnd = gameLogic.DrawCardIndex();
                CardNodes[i].setDatas(CardDatabase.CardDatas[rnd].Item1, CardDatabase.CardDatas[rnd].Item2);
            }

            GD.Print("-------------");
            GD.Print(_name);
            foreach (var item in CardNodes)
            {
                _cardContainer.RemoveChild(item);
            }

            CardNodes = CardNodes.OrderBy(node => node.getValue()).ToList();
            foreach (var item in CardNodes)
            {
                GD.Print(item.getValue() + " " + item.getTexture());
                _cardContainer.AddChild(item);
            }
            /*var ordered =  _cardNodes.OrderBy(node => node.getValue()).ToList();
            for (int i = 0; i < 8; i++)
            {
                _cardNodes[i].setDatas(ordered[i].getValue(),ordered[i].getTexture());
            }*/
        }
    }
}