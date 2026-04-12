using System;
using System.Linq;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using System.Collections.Generic;
using Extensions;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class PlayCardManager : IDisposable
    {
        private readonly HashSet<string> _generousCategories = new();
        private readonly Dictionary<string, HashSet<string>> _existingCards = new();
        private HashSet<string> _cardCategories = new();

        private bool _isDisposed;

        #region Card Management

        public void AddCard(ICard card)
        {
            if (!this._existingCards.ContainsKey(card.CardCategory))
            {
                this._existingCards.Add(card.CardCategory, new HashSet<string> { card.CardModel.cardContent });
            }

            else
            {
                HashSet<string> cardContent = this._existingCards[card.CardCategory];
                cardContent.Add(card.CardModel.cardContent);
                this._existingCards[card.CardCategory] = cardContent;
            }
        }

        #endregion

        public void RemoveCardCategory(string cardCategory)
        {
            this._existingCards.Remove(cardCategory);
        }

        #region Check Card Category Complete

        public void InitializeCardCategories(HashSet<string> cardCategories) =>
            this._cardCategories = cardCategories;

        public bool HasCategoryCardSolved(string cardCategory) => !this._cardCategories.Contains(cardCategory);

        public void MarkCategoryAsCompleted(string category)
        {
            this._cardCategories.Remove(category);
            this.RemoveGenerousCategory(category);
        }

        public HashSet<string> GetIncompletedCategories() => this._existingCards.Keys.ToHashSet();

        #endregion

        #region Check Generous Categories
        
        public string GetRandomGenerousCategory() => this._generousCategories.GetRandomElement();
        
        public void AddGenerousCategory(string cardCategory)
        {
            this._generousCategories.Add(cardCategory);
        }

        private void RemoveGenerousCategory(string cardCategory)
        {
            if (this._generousCategories.Contains(cardCategory))
                this._generousCategories.Remove(cardCategory);
        }

        #endregion

        #region Level Querying

        public bool ContainWord(string category, string word)
        {
            HashSet<string> words = this._existingCards.GetValueOrDefault(category);
            if (words is not { Count: > 0 }) 
                return false;

            bool contains = words.Contains(word);
            return contains;
        }

        public HashSet<string> GetCurrentCardsWithCategory(string category) =>
            this._existingCards.GetValueOrDefault(category);

        #endregion

        #region Disposing

        private void ReleaseManagedResources()
        {
            this._existingCards.Clear();
            this._cardCategories.Clear();
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
