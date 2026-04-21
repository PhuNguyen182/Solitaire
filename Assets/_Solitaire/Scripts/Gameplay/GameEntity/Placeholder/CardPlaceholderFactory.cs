using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.Placeholder
{
    public class CardPlaceholderFactory
    {
        private readonly ICardPlaceholder _cardPlaceholderPrefab;
        private readonly Transform _cardPlaceholderParentTransform;
        private readonly Transform _cardContainerTransform;

        public CardPlaceholderFactory(ICardPlaceholder cardPlaceholderPrefab, Transform cardPlaceholderParentTransform, 
            Transform cardContainerTransform)
        {
            this._cardPlaceholderPrefab = cardPlaceholderPrefab;
            this._cardPlaceholderParentTransform = cardPlaceholderParentTransform;
            this._cardContainerTransform = cardContainerTransform;
        }

        public ICardPlaceholder Create(CardPlaceholderModel param)
        {
            if (this._cardPlaceholderPrefab is not CardPlaceholder cardPlaceholder)
                return null;

            ICardPlaceholder instance = Object.Instantiate(cardPlaceholder, param.Position, Quaternion.identity,
                this._cardPlaceholderParentTransform);
            instance.SetCardContainer(this._cardContainerTransform);
            instance.BindModelData(param);
            return instance;
        }
    }
}
