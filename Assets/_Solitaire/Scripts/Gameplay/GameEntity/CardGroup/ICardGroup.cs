using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.Interfaces;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.CardGroup
{
    public interface ICardGroup : IFollowable
    {
        public List<ICard> ElementCards { get; }
        public bool IsEmpty { get; }
        
        public event Action OnCardAdded;
        public event Action OnCardGroupFreed;

        public bool ContainFoundationCard();
        public void SetCardsInGroupInteractable(bool isInteractable);
        public void AppendCards(params ICard[] cards);
        public void RemoveCard(params ICard[] cards);
        public void SnapDown(Vector3 snappedPosition);
        public void ReleaseDraggingCard();
    }
}