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
        private int _firstSortingOrder;
        
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

        public bool ContainNormalCard(ICard normalCard)
        {
            int cardCount = this._elementCards.Count;
            for (int i = 0; i < cardCount; i++)
            {
                if (string.CompareOrdinal(this._elementCards[i].CardModel.cardContent,
                        normalCard.CardModel.cardContent) == 0)
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
            return this._elementCards.Count > 0 ? this._elementCards[^1] : null;
        }

        public void AppendCards(bool assignGroupToCard, ICard sampleCard = null, params ICard[] cards)
        {
            int count = cards.Length;
            for (int i = 0; i < count; i++)
            {
                ICard card = cards[i];
                if (this._elementCards.Count <= 0)
                {
                    this._initialPosition = sampleCard?.WorldPosition ?? card.WorldPosition;
                    this._firstSortingOrder = card.SortingOrder;
                    this.AppendSingleCard(card, this._firstSortingOrder, this._initialPosition, assignGroupToCard);
                }
                else
                {
                    int currentCardsInGroupCount = this._elementCards.Count;
                    int step = currentCardsInGroupCount + i;
                    int sortingOrder = this._firstSortingOrder + currentCardsInGroupCount + i;
                    Vector3 stepPosition =
                        this._initialPosition + Vector3.down * (step * CardConstants.CardPositionOffset);
                    this.AppendSingleCard(card, sortingOrder, stepPosition, assignGroupToCard);
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
            {
                if (card.CardType != CardType.Foundation)
                    card.SetCardActive(false);
                else
                    card.UpdateNewInitialPosition(position);
            }

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

        public void ChangeLayerOnDragging()
        {
            int highestLayer = CardConstants.HighestCardSortingOrder;
            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
            {
                int cardLayer = this._elementCards[i].SortingOrder;
                int newLayer = cardLayer + highestLayer;
                this._elementCards[i].SetOrderLayer(newLayer);
            }
        }

        public void BackToOriginalLayer()
        {
            int highestLayer = CardConstants.HighestCardSortingOrder;
            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
            {
                int cardLayer = this._elementCards[i].SortingOrder;
                if (cardLayer <= highestLayer)
                    continue;
                
                int newLayer = cardLayer - highestLayer;
                this._elementCards[i].SetOrderLayer(newLayer);
            }
        }

        public void FollowPosition(Vector3 pointerPosition)
        {
            this.PrepareToDragAndMove(pointerPosition);
            if (this._selectedCard == null)
                return;
            
            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 newPosition = pointerPosition + this._cardPositionOffsets[i];
                this._elementCards[i].MoveToPositionImmediately(newPosition);
            }
        }

        public void SnapDownToOldPosition()
        {
            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
                this._elementCards[i].SnapBackToInitialedPosition(false);
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

        private void PrepareToDragAndMove(Vector3 pointerPosition)
        {
            if (this._selectedCard != null) 
                return;
            
            Collider2D cardCollider = Physics2D.OverlapPoint(pointerPosition, this._visualCardLayer);
            if (!cardCollider || !cardCollider.TryGetComponent(out ICard card)) 
                return;
            
            this._selectedCard = card;
            this.CalculateCardPositionOffsets();
        }

        private void CalculateCardPositionOffsets()
        {
            this._cardPositionOffsets.Clear();
            int count = this._elementCards.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = this._elementCards[i].WorldPosition - this._elementCards[0].WorldPosition;
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
