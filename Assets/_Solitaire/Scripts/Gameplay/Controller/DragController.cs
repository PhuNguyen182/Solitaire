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
        
        private void PickupCard()
        {
            Vector2 pointerPosition = this.inputController.WorldPointerPosition;
            Collider2D cardCollider = Physics2D.OverlapPoint(pointerPosition, this.cardPlaceholderLayer);
            if (!cardCollider || !cardCollider.TryGetComponent(out Card card)) 
                return;
            
            this._isCardDragging = true;
            this._pickedCard = card;
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
            
            this._pickedCard.UpdateNewInitialPosition(cardPlaceholder.transform.position);
            cardPlaceholder.AppendCard(this._pickedCard);
            return true;
        }

        private bool StackCardInAGroup()
        {
            Vector2 pointerPosition = this.inputController.WorldPointerPosition;
            Collider2D cardPlaceholderCollider = Physics2D.OverlapPoint(pointerPosition, this.visualCardLayer);

            if (!cardPlaceholderCollider ||
                !cardPlaceholderCollider.TryGetComponent(out Card card)) 
                return false;

            if (!card.IsSameCategory(this._pickedCard))
                return false;
            
            this._pickedCard.UpdateNewInitialPosition(card.transform.position);
            card.CardGroup.AppendCards(this._pickedCard);
            return true;
        }

        private void OnDisable()
        {
            this.inputController.OnPointerDown -= this.PickupCard;
            this.inputController.OnPointerUp -= this.DropCard;
        }
    }
}
