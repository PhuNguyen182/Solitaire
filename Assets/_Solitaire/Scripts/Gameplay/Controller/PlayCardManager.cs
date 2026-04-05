using System;
using System.Linq;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using System.Collections.Generic;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class PlayCardManager : IDisposable
    {
        private readonly HashSet<int> _completedCategories = new();
        private readonly Dictionary<int, List<string>> _existingCards = new();

        private bool _isDisposed;

        #region Card Management

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

        #endregion

        public void RemoveCardCategory(int cardCategory)
        {
            this._existingCards.Remove(cardCategory);
        }

        #region Check Card Category Complete

        public bool IsCategoryCard(int cardCategory) => this._completedCategories.Contains(cardCategory);

        public void MarkCategoryAsCompleted(int category) => this._completedCategories.Add(category);

        public HashSet<int> GetCompletedCategories() => this._completedCategories;

        #endregion

        #region Level Querying

        public List<int> GetCurrentCardCategories() => this._existingCards.Keys.ToList();

        public List<string> GetCurrentCardsWithCategory(int category) =>
            this._existingCards.GetValueOrDefault(category);

        #endregion

        #region Disposing

        private void ReleaseManagedResources()
        {
            this._existingCards.Clear();
            this._completedCategories.Clear();
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

        #endregion
    }
}
