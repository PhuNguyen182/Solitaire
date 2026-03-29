using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.CardGroup;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public interface ICard
    {
        public bool IsSingleCard { get; }
        public string CardCategory{ get; }
        public CardType CardType { get; }
        public ICardGroup CardGroup { get; }
        public Vector3 WorldPosition { get; }

        public void SetOrderLayer(int sortingOrder);
        public void SetCardGroup(ICardGroup cardGroup);
        public void CardPickedUp();
        public void UpdateNewInitialPosition(Vector3 position);
        public void SnapToInitialPosition();
        public void SetToPositionImmediately(Vector3 position);
        public void FollowPosition(Vector3 position);
        public void SetCardInteractable(bool isInteractable);
        public void AppendCardToGroup(params ICard[] card);
        public bool IsSameCategory(ICard card);
        public List<ICard> CheckAvailableCardOnDropDown();
        public void BindModel(CardModel model);
    }
}