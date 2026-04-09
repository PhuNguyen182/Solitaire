using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
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

        public bool CheckCategory(string cardCategory, int cardCount)
        {
            CategoryData cardCategoryData = null;
            int categoryCount = this._levelModel.availableCategories.Count;
            for (int i = 0; i < categoryCount; i++)
            {
                if (this._levelModel.availableCategories[i].categoryName != cardCategory) 
                    continue;
                
                cardCategoryData = this._levelModel.availableCategories[i];
                break;
            }

            // Check if cardCount is equal to max card count (including foundation card)
            bool isCategoryComplete = cardCategoryData != null && cardCategoryData.maxCardCount == cardCount - 1;
            if (!isCategoryComplete) 
                return false;
            
            this._playCardManager.MarkCategoryAsCompleted(cardCategory);
            this._playCardManager.RemoveCardCategory(cardCategory);
            return true;
        }
    }
}
