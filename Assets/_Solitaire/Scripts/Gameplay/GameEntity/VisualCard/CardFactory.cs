using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class CardFactoryParam
    {
        public CardModel CardModel;
        public Transform CardContainer;
        public ICardPlaceholder CardPlaceholder;
        public Vector3 Position;
    }
    
    public class CardFactory
    {
        private readonly ICard _playingCardPrefab;
        private readonly Camera _mainCamera;

        public CardFactory(ICard playingCardPrefab, Camera mainCamera)
        {
            this._playingCardPrefab = playingCardPrefab;
            this._mainCamera = mainCamera;
        }

        public ICard Create(CardFactoryParam param)
        {
            if (this._playingCardPrefab is not PlayingCard playingCard)
                return null;

            PlayingCard cardInstance = GameObjectPoolManager.SpawnInstance(playingCard, param.Position, 
                Quaternion.identity, param.CardContainer);
            cardInstance.BindModel(param.CardModel);
            cardInstance.SetCardPlaceholder(param.CardPlaceholder);
            cardInstance.FlipCard(false, true).Forget();
            cardInstance.SetupCamera(this._mainCamera);
            cardInstance.SetCardInteractable(false);
            return cardInstance;
        }
    }
}
