using System.Collections.Generic;
using _Solitaire.Scripts.GameInput;
using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class DragController : MonoBehaviour
    {
        [SerializeField] private InputController inputController;
        [SerializeField] private LayerMask visualCardLayer;
        [SerializeField] private LayerMask cardPlaceholderLayer;
        
        private Card _pickedCard;
        private bool _isCardDragging;
        
        private void OnEnable()
        {
            this.inputController.OnPointerDown += this.PickupCard;
            this.inputController.OnPointerUp += this.DropCard;
        }
        
        private void Update()
        {
            this.UpdatePickedCardPosition();
        }

        private void UpdatePickedCardPosition()
        {
            if (!this._isCardDragging || !this._pickedCard) 
                return;
            
            Vector2 pointerPosition = this.inputController.WorldPointerPosition;
            this._pickedCard.FollowPosition(pointerPosition);
        }

        private Card FindClosestCardToPointer(Vector3 pointerPosition, List<Card> cards)
        {
            Card closestCard = null;
            int count = cards.Count;
            float minDistance = Mathf.Infinity;
            
            for (int i = 0; i < count; i++)
            {
                float squaredDistance = (cards[i].transform.position - pointerPosition).sqrMagnitude;
                if (squaredDistance < minDistance * minDistance)
                {
                    closestCard = cards[i];
                    minDistance = Mathf.Sqrt(squaredDistance);
                }
            }
            
            return closestCard;
        }
        
        private void PickupCard()
        {
            Vector2 pointerPosition = this.inputController.WorldPointerPosition;
            Collider2D[] cardColliders = Physics2D.OverlapPointAll(pointerPosition, this.visualCardLayer);
            int count = cardColliders.Length;
            List<Card> cards = new();
            
            for (int i = 0; i < count; i++)
            {
                if (cardColliders[i].TryGetComponent(out Card card))
                    cards.Add(card);
            }
            
            Card closestCard = this.FindClosestCardToPointer(pointerPosition, cards);
            if (!closestCard)
                return;
            
            this._isCardDragging = true;
            this._pickedCard = closestCard;
            this._pickedCard.CardPickedUp();
        }

        private void DropCard()
        {
            if (!this._pickedCard)
                return;

            this._isCardDragging = false;
            bool isSnapToCardPlaceholder = this.SnapToCardPlaceholder();
            if (!isSnapToCardPlaceholder)
            {
                bool isCurrentCardAddedToNewGroup = this.StackCardInAGroup();
                if (!isCurrentCardAddedToNewGroup)
                    this._pickedCard.SnapToInitialPosition();
            }
            
            this._pickedCard = null;
        }

        private bool SnapToCardPlaceholder()
        {
            Vector2 pointerPosition = this.inputController.WorldPointerPosition;
            Collider2D cardPlaceholderCollider = Physics2D.OverlapPoint(pointerPosition, this.visualCardLayer);

            if (!cardPlaceholderCollider ||
                !cardPlaceholderCollider.TryGetComponent(out CardPlaceholder cardPlaceholder)) 
                return false;
            
            bool result = cardPlaceholder.TryAppendCard(this._pickedCard);
            return result;
        }

        private bool StackCardInAGroup()
        {
            List<Card> checkCards = this._pickedCard.CheckAvailableCardOnDropDown();
            if (checkCards is not { Count: > 0 })
                return false;

            Card sampleCard = checkCards[0];
            if (!sampleCard.IsSameCategory(this._pickedCard))
                return false;

            if (this._pickedCard.CardGroup == null)
                sampleCard.AppendCardToGroup(this._pickedCard);
            else
            {
                Card[] elementCards = this._pickedCard.CardGroup.ElementCards.ToArray();
                sampleCard.AppendCardToGroup(elementCards);
            }
            
            return true;
        }

        private void OnDisable()
        {
            this.inputController.OnPointerDown -= this.PickupCard;
            this.inputController.OnPointerUp -= this.DropCard;
        }
    }
}
