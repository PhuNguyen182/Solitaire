using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.Interfaces;
using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.Group
{
    public interface ICardGroup : IFollowable
    {
        public string CardCategory { get; }
        public List<ICard> ElementCards { get; }
        public ICardPlaceholder CardPlaceholder { get; }
        public int ElementCount { get; }
        public bool IsEmpty { get; }
        
        public event Action OnCardAdded;
        public event Action OnCardGroupFreed;

        public ICard GetLastCard();
        public void ChangeLayerOnDragging();
        public void BackToOriginalLayer();
        public bool ContainFoundationCard();
        public bool ContainNormalCard(ICard normalCard);
        public void SetCardPlaceholder(ICardPlaceholder placeholder);
        public void SetCardsInGroupInteractable(bool isInteractable);
        public void AppendCards(bool assignGroupToCard, Vector3 samplePosition, params ICard[] cards);
        public void RemoveCard(ICard card);
        public void ReupdateCardGroupInitialPosition(Vector3 samplePosition);
        public void SnapDownToOldPosition();
        public void ReleaseDraggingCard();
        public void Cleanup();
    }
}