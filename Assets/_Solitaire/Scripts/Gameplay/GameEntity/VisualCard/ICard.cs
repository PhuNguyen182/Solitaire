using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.Group;
using _Solitaire.Scripts.Gameplay.GameEntity.Interfaces;
using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public interface ICard : IFollowable
    {
        public bool IsSingleCard { get; }
        public bool HasCardPlacedInColumn { get; }
        public int SortingOrder { get; }
        public string CardCategory { get; }
        public CardType CardType { get; }
        public ICardGroup CardGroup { get; }
        public ICardPlaceholder CardPlaceholder { get; }
        public Vector3 WorldPosition { get; }
        public CardModel CardModel { get; }

        public void BindModel(CardModel model);
        public UniTask FlipCard(bool isFaceUp, bool isImmediately);
        public void CardPickedUp();
        public void UpdateCardPlacingState(bool isPlacedInColumn);
        public void BackToOriginalLayer();
        public void SetOrderLayer(int sortingOrder);
        public void SetCardGroup(ICardGroup cardGroup);
        public void SetCardPlaceholder(ICardPlaceholder cardPlaceholder);
        public void CardReleased(Vector3 snapPosition);
        public void UpdateNewInitialPosition(Vector3 position);
        public void SnapBackToInitialedPosition(bool shouldReleaseCard);
        public void MoveToPositionImmediately(Vector3 position);
        public void SetCardInteractable(bool isInteractable);
        public void AppendCardToGroup(params ICard[] card);
        public bool IsSameCategory(ICard card);
        public List<ICard> CheckAvailableCardOnDropDown();
        public void SetCardActive(bool isActive);
        public void Cleanup();
    }
}