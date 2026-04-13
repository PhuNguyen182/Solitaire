using System;
using System.Collections.Generic;
using Extensions;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class WordPool : IDisposable
    {
        private bool _isDisposed;

        private readonly Dictionary<string, CardModelByCategory> _wordsPool = new();

        public int GetCardCountByCategory(string categoryName)
        {
            CardModelByCategory cardModelByCategory = this._wordsPool.GetValueOrDefault(categoryName);
            return cardModelByCategory?.CardCount ?? 0;
        }
        
        public CardModel GetFullyRandomWord()
        {
            CardModelByCategory cardModelByCategory = this._wordsPool.GetRandomValue();
            CardModel result = cardModelByCategory.GetRandomCardModel();
            return result;
        }

        public CardModel GetRandomWordByCategory(string wordCategory)
        {
            CardModelByCategory cardModelByCategory = this._wordsPool.GetValueOrDefault(wordCategory);
            CardModel result = cardModelByCategory?.GetRandomCardModel();
            return result;
        }

        public void AddWordCategory(CardModelByCategory cardModelByCategory) =>
            this._wordsPool.Add(cardModelByCategory.CategoryName, cardModelByCategory);

        public void AddNewWordByCategory(string categoryName, CardModel word)
        {
            CardModelByCategory cardModelByCategory = this._wordsPool.GetValueOrDefault(categoryName);
            if (cardModelByCategory == null)
                return;
            
            cardModelByCategory.AddCardModel(word);
            this._wordsPool[categoryName] = cardModelByCategory;
        }

        public void RemoveWordByCategory(string categoryName, CardModel word)
        {
            CardModelByCategory cardModelByCategory = this._wordsPool.GetValueOrDefault(categoryName);
            if (cardModelByCategory == null)
                return;
            
            cardModelByCategory.RemoveCardModel(word);
            this._wordsPool[categoryName] = cardModelByCategory;
        }

        private void ReleaseUnmanagedResources()
        {
            foreach (var kvp in this._wordsPool)
                kvp.Value.Cleanup();
        }

        private void Dispose(bool disposing)
        {
            if (this._isDisposed)
                return;
            
            if (disposing)
                this.ReleaseUnmanagedResources();
            
            this._isDisposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WordPool()
        {
            this.Dispose(false);
        }
    }

    public class CardModelByCategory
    {
        private readonly HashSet<CardModel> _cardModels;
        
        public string CategoryName { get; private set; }
        public int CardCount => this._cardModels?.Count ?? 0;

        public CardModelByCategory(string categoryName, int numberOfWord)
        {
            this.CategoryName = categoryName;
            this._cardModels = new HashSet<CardModel>(numberOfWord);
        }
        
        public void AddCardModel(CardModel cardModel) => this._cardModels.Add(cardModel);

        public void RemoveCardModel(CardModel cardModel) => this._cardModels.Remove(cardModel);

        public CardModel GetRandomCardModel() => this._cardModels.GetRandomElement();

        public void Cleanup()
        {
            this.CategoryName = null;
            this._cardModels.Clear();
        }
    }
}