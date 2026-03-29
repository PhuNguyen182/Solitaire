using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.Group;
using _Solitaire.Scripts.Gameplay.GameEntity.Interfaces;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public interface ICard : IFollowable
    {
        public bool IsSingleCard { get; }
        public string CardCategory{ get; }
        public CardType CardType { get; }
        public ICardGroup CardGroup { get; }
        public Vector3 WorldPosition { get; }

        public void BindModel(CardModel model);
        public void CardPickedUp();
        public void SetOrderLayer(int sortingOrder);
        public void SetCardGroup(ICardGroup cardGroup);
        public void UpdateNewInitialPosition(Vector3 position);
        public void SnapBackToInitialedPosition();
        public void MoveToPositionImmediately(Vector3 position);
        public void SetCardInteractable(bool isInteractable);
        public void AppendCardToGroup(params ICard[] card);
        public bool IsSameCategory(ICard card);
        public List<ICard> CheckAvailableCardOnDropDown();
    }
}