using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using _Solitaire.Scripts.Gameplay.Level;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class LevelManager
    {
        private readonly LevelModel _levelModel;
        private readonly PlayCardManager _playCardManager;

        public LevelManager(LevelModel levelModel, PlayCardManager playCardManager)
        {
            this._levelModel = levelModel;
            this._playCardManager = playCardManager;
        }

        public bool CheckCategory(ICardPlaceholder cardPlaceholder)
        {
            CategoryData cardCategoryData = GetCardCategoryByCategory(cardPlaceholder.CardCategory);
            int cardCountByCategory = cardPlaceholder.FoundationCard?.CardGroup?.ElementCount ?? 0;
            bool isCategoryComplete = cardCategoryData != null && cardCategoryData.maxCardCount == cardCountByCategory - 1;
            if (!isCategoryComplete) 
                return false;
            
            this._playCardManager.MarkCategoryAsCompleted(cardPlaceholder.CardCategory);
            this._playCardManager.RemoveCardCategory(cardPlaceholder.CardCategory);
            cardPlaceholder.Cleanup();
            return true;
        }

        public CategoryData GetCardCategoryByCategory(string category)
        {
            CategoryData cardCategoryData = null;
            int categoryCount = this._levelModel.availableCategories.Count;
            for (int i = 0; i < categoryCount; i++)
            {
                if (string.CompareOrdinal(this._levelModel.availableCategories[i].categoryName, category) != 0) 
                    continue;
                
                cardCategoryData = this._levelModel.availableCategories[i];
                break;
            }

            return cardCategoryData;
        }
    }
}
