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
        public List<ICard> ElementCards { get; }
        public ICardPlaceholder CardPlaceholder { get; }
        public bool IsEmpty { get; }
        
        public event Action OnCardAdded;
        public event Action OnCardGroupFreed;

        public ICard GetLastCard();
        public bool ContainFoundationCard();
        public void SetCardPlaceholder(ICardPlaceholder placeholder);
        public void SetCardsInGroupInteractable(bool isInteractable);
        public void AppendCards(bool assignGroupToCard, ICard sampleCard = null, params ICard[] cards);
        public void RemoveCard(params ICard[] cards);
        public void SnapDown(Vector3 snappedPosition);
        public void ReleaseDraggingCard();
        public void Cleanup();
    }
}