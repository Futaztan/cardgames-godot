using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cardgames.Games.Lorum.Scripts.Cards;
using cardgames.Lorum.Scripts.Cards;
using cardgames.Lorum.Scripts.UI;
using Godot;

namespace cardgames.Games.Lorum.Scripts.Players
{
    public abstract class EntityBase
    {
        protected List<CardBase> CardNodes = new List<CardBase>();
        protected string _name = "placeholder";
        protected Pass _passIcon;
        public string Name => _name;
        private int _score = 0;
        public int Score => _score;
        private CardContainer _cardContainer;
        public CardContainer CardContainer => _cardContainer;
        public int CardsInHandCount => CardNodes.Count;
        protected const int WaitMillisAfterCardPlace = 300;


        private AudioStreamPlayer _soundPlayer;
        private AudioStream _cardPlaceSound;
        private RichTextLabel _label;


        protected EntityBase(string name, int score, RichTextLabel label, CardContainer container, Pass passIcon)
        {
            _name = name;
            _label = label;
            _score = score;
            _cardContainer = container;
            _passIcon = passIcon;
            _cardPlaceSound = GD.Load<AudioStream>("res://Assets/Sound/card_placed.mp3");
        
            _soundPlayer = new AudioStreamPlayer();
            _soundPlayer.Stream = _cardPlaceSound;
            _cardContainer.AddChild(_soundPlayer);    
            UpdateLabel();
        }
        
        protected void PlayCardSound()
        {
            if (_soundPlayer == null || _cardPlaceSound == null) return;
            _soundPlayer.PitchScale = (float)GD.RandRange(0.95, 1.05);
            _soundPlayer.Play();
        }

        public abstract Task PassTurn();

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
            _label.Text = "[b]" + _name + "\n" + _score + " pont [/b]";
            _label.Size = _label.GetMinimumSize();
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
        
        public bool HasPlayableCard()
        {
            foreach (var card in CardNodes)
            {
                int value = card.getValue();
                Cell cell = Lorum.CenterCells[WhichCell(value)];
                if (IsPlaceable(value, cell)) return true;
            }
            return false;
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
            foreach (CardBase item in _cardContainer.GetChildren().OfType<CardBase>())
            {
                item.QueueFree();
            }
        }

        public void NewCardToHand(int rnd)
        {
            CardBase newcard = (CardBase)_cardContainer.CardScene.Instantiate();
            _cardContainer.AddChild(newcard);
            CardNodes.Add(newcard);
            CardNodes.Last().setDatas(CardDatabase.CardDatas[rnd].Item1, CardDatabase.CardDatas[rnd].Item2);

            GD.Print("-------------");
            GD.Print(Name);
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
        }
    }
}