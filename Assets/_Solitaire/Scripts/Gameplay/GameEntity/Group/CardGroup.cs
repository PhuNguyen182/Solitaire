using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.Group
{
    public class CardGroup : ICardGroup, IDisposable
    {
        private readonly LayerMask _visualCardLayer;
        private readonly List<Vector3> _cardPositionOffsets;
        private readonly List<ICard> _elementCards;
        
        private bool _isDisposed;
        private ICard _selectedCard;
        private Vector3 _initialPosition;
        private ICardPlaceholder _selectedCardPlaceholder;
        
        public List<ICard> ElementCards => this._elementCards;
        public ICardPlaceholder CardPlaceholder => this._selectedCardPlaceholder;
        public bool IsEmpty => this._elementCards.Count <= 0;
        
        public event Action OnCardAdded;
        public event Action OnCardGroupFreed;

        public CardGroup(LayerMask visualCardLayer)
        {
            this._visualCardLayer = visualCardLayer;
            this._cardPositionOffsets = new List<Vector3>();
            this._elementCards = new List<ICard>();
        }

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

        public void SetCardPlaceholder(ICardPlaceholder placeholder)
        {
            this._selectedCardPlaceholder = placeholder;
        }

        public void SetCardsInGroupInteractable(bool isInteractable)
        {
            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
                this._elementCards[i].SetCardInteractable(isInteractable);
        }

        public ICard GetLastCard()
        {
            return this._elementCards[^1];
        }

        public void AppendCards(bool assignGroupToCard, params ICard[] cards)
        {
            int count = cards.Length;
            int currentCardsInGroupCount = this._elementCards.Count;
            for (int i = 0; i < count; i++)
            {
                ICard card = cards[i];
                if (this._elementCards.Count <= 0)
                {
                    this._initialPosition = card.WorldPosition;
                    this.AppendSingleCard(card, 0, this._initialPosition, assignGroupToCard);
                }
                else
                {
                    int step = currentCardsInGroupCount + i;
                    Vector3 stepPosition =
                        this._initialPosition + Vector3.down * (step * CardConstants.CardPositionOffset);
                    this.AppendSingleCard(card, step, stepPosition, assignGroupToCard);
                }

                card.UpdateCardPlacingState(true);
            }

            this.OnCardAdded?.Invoke();
        }

        private void AppendSingleCard(ICard card, int sortingOrder, Vector3 position, bool assignGroupToCard)
        {
            card.UpdateNewInitialPosition(position);
            card.SetOrderLayer(sortingOrder);
            if (assignGroupToCard)
                card.SetCardGroup(this);
            
            card.SetCardPlaceholder(this.CardPlaceholder);
            if (this.CardPlaceholder is { CardType: CardType.Foundation })
                card.UpdateNewInitialPosition(this.CardPlaceholder.CurrentTransform.position);
            
            this._elementCards.Add(card);
        }

        public void RemoveCard(params ICard[] cards)
        {
            int count = cards.Length;
            for (int i = 0; i < count; i++)
            {
                if (!this._elementCards.Contains(cards[i]))
                    continue;

                this._elementCards.Remove(cards[i]);
                cards[i].SetCardGroup(null);
            }

            this.OnCardAdded?.Invoke();
        }

        public void FollowPosition(Vector3 pointerPosition)
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
                this._elementCards[i].MoveToPositionImmediately(newPosition);
            }
        }

        public void SnapDown(Vector3 snappedPosition)
        {
            this.FollowPosition(snappedPosition);
        }

        public void ReleaseDraggingCard()
        {
            this._selectedCard = null;
            this._cardPositionOffsets.Clear();
        }

        public void Cleanup()
        {
            foreach (ICard card in this._elementCards)
                card.Cleanup();
            
            this._elementCards.Clear();
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

        private void ReleaseManagedResources()
        {
            this.ReleaseCards();
        }

        private void Dispose(bool disposing)
        {
            if (this._isDisposed)
                return;
            
            if (disposing)
                this.ReleaseManagedResources();

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
