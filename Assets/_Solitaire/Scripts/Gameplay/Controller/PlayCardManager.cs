using System;
using System.Collections.Generic;
using System.Linq;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class PlayCardManager : IDisposable
    {
        private readonly Dictionary<int, List<string>> _existingCards = new();
        
        private bool _isDisposed;

        public void AddCard(ICard card)
        {
            if (!this._existingCards.ContainsKey(card.CardCategory))
            {
                this._existingCards.Add(card.CardCategory, new List<string> { card.CardModel.cardContent });
            }

            else
            {
                List<string> cardContent = this._existingCards[card.CardCategory];
                cardContent.Add(card.CardModel.cardContent);
                this._existingCards[card.CardCategory] = cardContent;
            }
        }

        public bool RemoveCard(ICard card)
        {
            if (!this._existingCards.TryGetValue(card.CardCategory, out List<string> cardContent))
                return false;

            bool cardRemoved = cardContent.Remove(card.CardModel.cardContent);
            if (!cardRemoved) 
                return false;
            
            if (cardContent.Count <= 0)
            {
                this._existingCards.Remove(card.CardCategory);
                return true;
            }
                
            this._existingCards[card.CardCategory] = cardContent;
            return true;
        }

        public List<int> GetCurrentCardCategories() => this._existingCards.Keys.ToList();

        public List<string> GetCurrentCardsWithCategory(int category) =>
            this._existingCards.GetValueOrDefault(category);

        private void ReleaseManagedResources()
        {
            this._existingCards.Clear();
        }

        private void Dispose(bool disposing)
        {
            if (this._isDisposed)
                return;
            
            if (disposing)
            {
                this.ReleaseManagedResources();
            }
            
            this._isDisposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PlayCardManager()
        {
            this.Dispose(false);
        }
    }
}
