using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;

namespace _Solitaire.Scripts.Gameplay.GameEntity.CardGroup
{
    public class CardGroup : IDisposable
    {
        private string _cardCategory;
        private readonly List<Card> _elementCards = new();
        
        public event Action OnCardAdded;
        public event Action OnCardGroupFreed;
        
        public List<Card> ElementCards => this._elementCards;

        public CardGroup()
        {
            this._cardCategory = null;
            this._elementCards.Clear();
        }
        
        public void AppendCards(params Card[] cards)
        {
            int count = cards.Length;
            for (int i = 0; i < count; i++)
            {
                if (this._elementCards.Count <= 0)
                {
                    this._cardCategory = cards[i].CardCategory;
                    cards[i].SetCardGroupData(this);
                    this._elementCards.Add(cards[i]);
                }
                else
                {
                    if (string.CompareOrdinal(cards[i].CardCategory, this._cardCategory) != 0) 
                        continue;
                    
                    cards[i].SetCardGroupData(this);
                    this._elementCards.Add(cards[i]);
                }
            }

            this.OnCardAdded?.Invoke();
        }
        
        public void RemoveCard(params Card[] cards)
        {
            int count = cards.Length;
            for (int i = 0; i < count; i++)
            {
                this._elementCards.Remove(cards[i]);
                cards[i].SetCardGroupData(null);
            }

            if (this._elementCards.Count <= 0)
                this._cardCategory = null;
            
            this.OnCardAdded?.Invoke();
        }

        public void Dispose()
        {
            this._elementCards.Clear();
            this.OnCardGroupFreed?.Invoke();
        }
    }
}
