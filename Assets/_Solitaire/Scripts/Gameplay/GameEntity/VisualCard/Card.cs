using _Solitaire.Scripts.Gameplay.GameEntity.CardGroup;
using TMPro;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private CardType cardType;
        [SerializeField] private TMP_Text cardText;
        [SerializeField] private SpriteRenderer cardIcon;
        [SerializeField] private GameObject foundationMark;
        [SerializeField] private BoxCollider2D cardCollider;

        private CardModel _cardModel;
        private CardGroupData _cardGroupData;
        private Vector3 _initialPosition;
        private bool _isDraggable;
        
        public CardType CardType => cardType;
        public bool IsDraggable => this._isDraggable;
        public string CardCategory => this._cardModel.cardCategory;
        
        public void SetCardGroupData(CardGroupData cardGroupData) => this._cardGroupData = cardGroupData;

        #region Card Movement

        public void SetDraggable(bool isDraggable) => this._isDraggable = isDraggable;

        public void SetInitialPosition(Vector3 position)
        {
            this._initialPosition = position;
            this.transform.position = position;
        }

        public void FollowPosition(Vector3 position) => this.transform.position = position;

        public void SnapToInitialPosition() => this.transform.position = this._initialPosition;

        #endregion

        #region Card Interaction
        
        public void SetCardInteractable(bool isInteractable) => this.cardCollider.enabled = isInteractable;

        public bool IsSameCategory(Card card)
        {
            bool isSameCategory = string.CompareOrdinal(this.CardCategory, card.CardCategory) == 0;
            return isSameCategory;
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
    }
}
