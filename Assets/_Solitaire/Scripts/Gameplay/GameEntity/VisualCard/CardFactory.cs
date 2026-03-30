using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class CardFactoryParam
    {
        public CardModel CardModel;
        public ICardPlaceholder CardPlaceholder;
        public Vector3 Position;
    }
    
    public class CardFactory
    {
        private readonly ICard _playingCardPrefab;

        public CardFactory(ICard playingCardPrefab)
        {
            this._playingCardPrefab = playingCardPrefab;
        }

        public ICard Create(CardFactoryParam param)
        {
            if (this._playingCardPrefab is not PlayingCard playingCard)
                return null;

            ICard cardInstance = GameObjectPoolManager.SpawnInstance(playingCard, param.Position, Quaternion.identity,
                param.CardPlaceholder.CurrentTransform);
            cardInstance.BindModel(param.CardModel);
            cardInstance.SetCardPlaceholder(param.CardPlaceholder);
            cardInstance.FlipCard(false, true);
            cardInstance.SetCardInteractable(false);
            return cardInstance;
        }
    }
}
