using System;
using System.Collections.Generic;
using Extensions;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class WordPool : IDisposable
    {
        private bool _isDisposed;

        private readonly Dictionary<string, CardModelByCategory> _wordsPool = new();

        public event Action<bool> OnWordPoolCleaning;
        public bool HasWordPoolInitialized { get; set; }

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
            if (string.IsNullOrEmpty(categoryName))
                return;
            
            CardModelByCategory cardModelByCategory = this._wordsPool.GetValueOrDefault(categoryName);
            if (cardModelByCategory == null)
                return;
            
            cardModelByCategory.RemoveCardModel(word);
            this._wordsPool[categoryName] = cardModelByCategory;
            if (this.HasWordPoolInitialized)
            {
                int remainWordCount = GetRemainWordCount();
                this.OnWordPoolCleaning?.Invoke(remainWordCount <= 0);
            }

            return;

            int GetRemainWordCount()
            {
                int wordCount = 0;
                foreach (var kvp in _wordsPool)
                {
                    wordCount += kvp.Value.CardCount;
                }

                return wordCount;
            }
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
}