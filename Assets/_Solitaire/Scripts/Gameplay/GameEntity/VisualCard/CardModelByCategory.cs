using System.Collections.Generic;
using Extensions;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class CardModelByCategory
    {
        private HashSet<CardModel> _cardModels;
        
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
            this._cardModels = null;
        }
    }
}