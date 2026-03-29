using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private CardType cardType;
        [SerializeField] private TMP_Text cardText;
        [SerializeField] private LayerMask cardLayer;
        [SerializeField] private SpriteRenderer cardIcon;
        [SerializeField] private GameObject foundationMark;
        [SerializeField] private BoxCollider2D cardCollider;
        [SerializeField] private SortingGroup cardSortingGroup;

        private CardModel _cardModel;
        private CardGroup.CardGroup _cardGroup;
        private Vector3 _initialPosition;
        private bool _isDraggable;
        
        public bool IsDraggable => this._isDraggable;
        public bool IsSingleCard => this._cardGroup == null || this._cardGroup.IsEmpty;

        public string CardCategory => this._cardModel.cardCategory;
        public CardType CardType => this.cardType;
        public CardGroup.CardGroup CardGroup => this._cardGroup;

        private void Awake()
        {
            this._initialPosition = this.transform.position;
        }

        public void SetOrderLayer(int sortingOrder)
        {
            this.cardSortingGroup.sortingOrder = sortingOrder;
        }

        public void SetCardGroup(CardGroup.CardGroup cardGroup)
        {
            this._cardGroup = cardGroup;
        }

        public void CardPickedUp()
        {
            if (this._cardGroup == null)
                this.SetCardInteractable(false);
            else
                this._cardGroup.SetCardsInGroupInteractable(false);
        }

        #region Card Movement

        public void SetDraggable(bool isDraggable)
        {
            this._isDraggable = isDraggable;
        }

        public void UpdateNewInitialPosition(Vector3 position)
        {
            this._initialPosition = position;
            this.transform.position = position;
        }

        public void FollowPosition(Vector3 position)
        {
            if (this._cardGroup == null)
                this.transform.position = position;
            else
                this._cardGroup.FollowPointer(position);
        }

        public void SnapToInitialPosition()
        {
            this.transform.position = this._initialPosition;
            this.CardReleased(this._initialPosition);
        }

        #endregion

        #region Card Interaction
        
        public void SetCardInteractable(bool isInteractable)
        {
            this.cardCollider.enabled = isInteractable;
        }

        public void AppendCardToGroup(params Card[] card)
        {
            this._cardGroup ??= new CardGroup.CardGroup();
            this._cardGroup.AppendCards(card);
        }
        
        public bool IsSameCategory(Card card)
        {
            bool isSameCategory = string.CompareOrdinal(this.CardCategory, card.CardCategory) == 0;
            return isSameCategory;
        }

        public List<Card> CheckAvailableCardOnDropDown()
        {
            Collider2D[] cardColliders =
                Physics2D.OverlapBoxAll(this.transform.position, this.cardCollider.size, 0, this.cardLayer);
            if (cardColliders is not { Length: > 0 }) 
                return null;
            
            int count = cardColliders.Length;
            List<Card> result = new();

            for (int i = 0; i < count; i++)
            {
                if (cardColliders[i].gameObject.TryGetComponent(out Card card))
                    result.Add(card);
            }

            return result;
        }

        #endregion

        public void BindModel(CardModel model)
        {
            this._cardModel = model;
            this.OnModelBound(model);
        }

        private void OnModelBound(CardModel model)
        {
            this.cardType = model.cardType;
            this.BindCardTextContent(model);
            this.BindCardImageContent(model);
            this.ToggleFoundationMark(model.cardType == CardType.Foundation);
        }

        private void BindCardTextContent(CardModel model)
        {
            if (!this.cardText) 
                return;
            
            if (model.contentType != CardContentType.Text)
            {
                this.cardText.gameObject.SetActive(false);
                return;
            }

            this.cardText.text = model.cardContent;
            this.cardText.gameObject.SetActive(true);
        }

        private void BindCardImageContent(CardModel model)
        {
            if (!this.cardIcon) 
                return;
            
            if (model.contentType != CardContentType.Image)
            {
                this.cardIcon.gameObject.SetActive(false);
                return;
            }

            //TODO: this.cardIcon.sprite = model.cardContent;
            this.cardIcon.gameObject.SetActive(true);
        }

        private void ToggleFoundationMark(bool isFoundation)
        {
            if (this.foundationMark)
                this.foundationMark.SetActive(isFoundation);
        }
        
        private void CardReleased(Vector3 snapPosition)
        {
            this._cardGroup?.SnapDown(snapPosition);
            this._cardGroup?.ReleaseDraggingCard();
            this.SetCardInteractable(true);
            this._cardGroup?.SetCardsInGroupInteractable(true);
        }
    }
}
