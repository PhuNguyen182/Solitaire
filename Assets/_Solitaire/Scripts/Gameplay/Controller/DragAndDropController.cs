using System;
using System.Collections.Generic;
using _Solitaire.Scripts.GameInput;
using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class DragAndDropController : MonoBehaviour
    {
        [SerializeField] private InputController inputController;
        [SerializeField] private LayerMask visualCardLayer;
        [SerializeField] private LayerMask cardPlaceholderLayer;

        private bool _isCardDragging;
        private int _pickedCardPlaceholderID;
        private Collider2D[] _cardColliders;
        
        private ICard _pickedCard;
        private ICardPlaceholder _pickedCardPlaceholder;
        private CardPlaceholderManager _cardPlaceholderManager;
        private PlayCardManager _playCardManager;
        private WordPool _wordPool;
        
        public event Action<bool> OnCardDropped;

        private void OnEnable()
        {
            this.inputController.OnPointerDown += this.PickupCard;
            this.inputController.OnPointerUp += this.DropCard;
        }

        private void Update()
        {
            this.UpdatePickedCardPosition();
        }
        
        public void SetWordPool(WordPool wordPool) => this._wordPool = wordPool;

        public void SetPlayCardManager(PlayCardManager playCardManager)
        {
            this._playCardManager = playCardManager;
        }

        public void SetCardPlaceholderManager(CardPlaceholderManager cardPlaceholderManager)
        {
            this._cardPlaceholderManager = cardPlaceholderManager;
        }

        private void UpdatePickedCardPosition()
        {
            if (!this._isCardDragging || this._pickedCard == null)
                return;

            Vector2 pointerPosition = this.inputController.WorldPointerPosition;
            this._pickedCard.FollowPosition(pointerPosition);
        }

        private ICard FindClosestCardToPointer(Vector3 pointerPosition, List<ICard> cards)
        {
            ICard closestCard = null;
            int count = cards.Count;
            if (count <= 0)
                return null;
            
            float minDistance = Mathf.Infinity;
            bool hasCardPlacedInColumn = cards[0].HasCardPlacedInColumn;

            for (int i = 0; i < count; i++)
            {
                float distance = hasCardPlacedInColumn 
                    ? (cards[i].WorldPosition - pointerPosition).y
                    : (cards[i].WorldPosition - pointerPosition).x;
                
                distance = Mathf.Abs(distance);
                if (distance >= minDistance) 
                    continue;
                
                closestCard = cards[i];
                minDistance = distance;
            }

            return closestCard;
        }

        private void PickupCard()
        {
            Vector2 pointerPosition = this.inputController.WorldPointerPosition;
            this._cardColliders = Physics2D.OverlapPointAll(pointerPosition, this.visualCardLayer);
            int count = this._cardColliders.Length;
            List<ICard> cards = new();

            for (int i = 0; i < count; i++)
            {
                if (this._cardColliders[i].TryGetComponent(out ICard card))
                    cards.Add(card);
            }

            ICard closestCard = this.FindClosestCardToPointer(pointerPosition, cards);
            if (closestCard == null)
                return;

            this._isCardDragging = true;
            this._pickedCard = closestCard;
            this._pickedCardPlaceholderID = closestCard.CardPlaceholder?.CardPlaceHolderID ?? 0;
            this._pickedCardPlaceholder = this._cardPlaceholderManager.GetCardPlaceholder(this._pickedCardPlaceholderID);
            this._pickedCard.CardPickedUp();
        }

        private void DropCard()
        {
            if (this._pickedCard == null)
                return;

            this._isCardDragging = false;
            bool isSnapToCardPlaceholder = this.SnapToCardPlaceholder();
            bool isCardFromSupplier = this._pickedCard.CardPlaceholder == null;
            if (!isSnapToCardPlaceholder)
            {
                bool isCurrentCardAddedToNewGroup = this.StackCardInAGroup();
                if (!isCurrentCardAddedToNewGroup)
                    DropInvalidatedCard();
                else
                    DropValidatedCard();
            }
            else
            {
                DropValidatedCard();
            }

            this._pickedCard = null;
            this._cardColliders = null;
            return;

            void DropValidatedCard()
            {
                if (isCardFromSupplier)
                    this._playCardManager.AddCard(this._pickedCard);
                
                this._wordPool.RemoveWordByCategory(this._pickedCard.CardCategory, this._pickedCard.CardModel);
                this._pickedCardPlaceholder?.RemoveCard(this._pickedCard);
                this._pickedCardPlaceholder?.FlipLastCard();
                this.OnCardDropped?.Invoke(true);
            }
            
            void DropInvalidatedCard()
            {
                this._pickedCard.SnapBackToInitialedPosition();
                this.OnCardDropped?.Invoke(false);
            }
        }

        private bool SnapToCardPlaceholder()
        {
            Vector2 pointerPosition = this.inputController.WorldPointerPosition;
            Collider2D cardPlaceholderCollider = Physics2D.OverlapPoint(pointerPosition, this.cardPlaceholderLayer);

            if (!cardPlaceholderCollider ||
                !cardPlaceholderCollider.TryGetComponent(out ICardPlaceholder cardPlaceholder))
                return false;

            bool result = cardPlaceholder.TryAppendCard(this._pickedCard);
            if (!result) 
                return false;
            
            this._pickedCard.UpdateNewInitialPosition(cardPlaceholder.CurrentTransform.position);
            this._pickedCard.CardReleased(cardPlaceholder.CurrentTransform.position);
            return true;
        }

        private bool StackCardInAGroup()
        {
            List<ICard> checkCards = this.GetCheckCards(this._pickedCard);
            if (checkCards is not { Count: > 0 })
                return false;

            int checkCardCount = checkCards.Count;
            for (int i = 0; i < checkCardCount; i++)
            {
                if (!checkCards[i].HasCardPlacedInColumn)
                    return false;
            }

            ICard sampleCard = checkCards[0];
            if (!sampleCard.IsSameCategory(this._pickedCard))
                return false;

            if (this._pickedCard.CardGroup == null)
            {
                this._pickedCard.SetCardPlaceholder(sampleCard.CardPlaceholder);
                sampleCard.AppendCardToGroup(this._pickedCard);
            }
            else
            {
                ICard[] elementCards = this._pickedCard.CardGroup.ElementCards.ToArray();
                sampleCard.AppendCardToGroup(elementCards);
            }

            return true;
        }

        private List<ICard> GetCheckCards(ICard sampleCard)
        {
            List<ICard> result = new();
            if (sampleCard.IsSingleCard)
            {
                result = sampleCard.CheckAvailableCardOnDropDown();
                return result;
            }

            int count = sampleCard.CardGroup.ElementCards.Count;
            for (int i = 0; i < count; i++)
            {
                List<ICard> cards = sampleCard.CardGroup.ElementCards[i].CheckAvailableCardOnDropDown();
                result.AddRange(cards);
            }

            return result;
        }

        private void OnDisable()
        {
            this.inputController.OnPointerDown -= this.PickupCard;
            this.inputController.OnPointerUp -= this.DropCard;
        }
    }
}
