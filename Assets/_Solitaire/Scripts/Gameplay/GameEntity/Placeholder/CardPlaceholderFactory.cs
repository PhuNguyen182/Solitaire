using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.Placeholder
{
    public class CardPlaceholderFactory
    {
        private readonly ICardPlaceholder _cardPlaceholderPrefab;
        private readonly Transform _cardPlaceholderParentTransform;

        public CardPlaceholderFactory(ICardPlaceholder cardPlaceholderPrefab, Transform cardPlaceholderParentTransform)
        {
            this._cardPlaceholderPrefab = cardPlaceholderPrefab;
            this._cardPlaceholderParentTransform = cardPlaceholderParentTransform;
        }

        public ICardPlaceholder Create(CardPlaceholderModel param)
        {
            if (this._cardPlaceholderPrefab is not CardPlaceholder cardPlaceholder)
                return null;

            CardPlaceholder instance = Object.Instantiate(cardPlaceholder, param.Position, Quaternion.identity,
                this._cardPlaceholderParentTransform);
            instance.BindModelData(param);
            return instance;
        }
    }
}
