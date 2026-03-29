using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.CardGroup
{
    public class CardGroup : ICardGroup, IDisposable
    {
        private const string VisualCardLayerName = "Card";
        
        private readonly LayerMask _visualCardLayer = LayerMask.GetMask(VisualCardLayerName);
        private readonly List<Vector3> _cardPositionOffsets = new();
        private readonly List<ICard> _elementCards = new();
        
        private bool _isDisposed;
        private ICard _selectedCard;
        private Vector3 _initialPosition;
        private string _cardCategory;
        
        public event Action OnCardAdded;
        public event Action OnCardGroupFreed;
        
        public List<ICard> ElementCards => this._elementCards;
        public bool IsEmpty => this._elementCards.Count <= 0;

        public bool ContainFoundationCard()
        {
            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
            {
                if (this._elementCards[i].CardType == CardType.Foundation)
                    return true;
            }
            
            return false;
        }

        public void SetCardsInGroupInteractable(bool isInteractable)
        {
            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
                this._elementCards[i].SetCardInteractable(isInteractable);
        }

        public void AppendCards(params ICard[] cards)
        {
            int count = cards.Length;
            int currentCardsInGroupCount = this._elementCards.Count;
            for (int i = 0; i < count; i++)
            {
                ICard card = cards[i];
                if (this._elementCards.Count <= 0)
                {
                    this._cardCategory = card.CardCategory;
                    this._initialPosition = card.WorldPosition;
                    card.UpdateNewInitialPosition(this._initialPosition);
                    card.SetOrderLayer(0);
                    card.SetCardGroup(this);
                    this._elementCards.Add(card);
                }
                else
                {
                    if (string.CompareOrdinal(card.CardCategory, this._cardCategory) != 0)
                        continue;

                    int step = currentCardsInGroupCount + i;
                    Vector3 stepPosition =
                        this._initialPosition + Vector3.down * (step * CardConstants.CardPositionOffset);
                    card.UpdateNewInitialPosition(stepPosition);
                    card.SetOrderLayer(step);
                    card.SetCardGroup(this);
                    this._elementCards.Add(card);
                }
            }

            this.OnCardAdded?.Invoke();
        }
        
        public void RemoveCard(params ICard[] cards)
        {
            int count = cards.Length;
            for (int i = 0; i < count; i++)
            {
                this._elementCards.Remove(cards[i]);
                cards[i].SetCardGroup(null);
            }

            if (this._elementCards.Count <= 0)
                this._cardCategory = null;
            
            this.OnCardAdded?.Invoke();
        }

        public void FollowPointer(Vector3 pointerPosition)
        {
            if (this._selectedCard == null)
            {
                Collider2D cardCollider = Physics2D.OverlapPoint(pointerPosition, this._visualCardLayer);
                if (cardCollider && cardCollider.TryGetComponent(out ICard card))
                    this._selectedCard = card;
                
                this.CalculateCardPositionOffsets(pointerPosition);
            }

            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 newPosition = pointerPosition + this._cardPositionOffsets[i];
                this._elementCards[i].SetToPositionImmediately(newPosition);
            }
        }

        public void SnapDown(Vector3 snappedPosition)
        {
            this.FollowPointer(snappedPosition);
        }

        public void ReleaseDraggingCard()
        {
            this._selectedCard = null;
            this._cardPositionOffsets.Clear();
        }

        private void CalculateCardPositionOffsets(Vector3 pointerPosition)
        {
            this._cardPositionOffsets.Clear();
            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = pointerPosition - this._elementCards[i].WorldPosition;
                this._cardPositionOffsets.Add(offset);
            }
        }

        private void ReleaseCards()
        {
            this._elementCards.Clear();
            this.OnCardGroupFreed?.Invoke();
            this.OnCardGroupFreed = null;
        }

        private void ReleaseUnmanagedResources()
        {
            this.ReleaseCards();
        }

        private void Dispose(bool disposing)
        {
            if (this._isDisposed)
                return;
            
            if (disposing)
                this.ReleaseUnmanagedResources();

            this._isDisposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CardGroup()
        {
            this.Dispose(false);
        }
    }
}
