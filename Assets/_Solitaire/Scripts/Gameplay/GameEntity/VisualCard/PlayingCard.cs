using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.Group;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class PlayingCard : MonoBehaviour, ICard
    {
        [SerializeField] private CardType cardType;
        
        [Header("Card Visual")]
        [SerializeField] private Image cardIcon;
        [SerializeField] private TMP_Text cardText;
        [SerializeField] private GameObject foundationMark;
        [SerializeField] private Canvas cardSortingGroup;
        
        [Header("Card Physics")]
        [SerializeField] private LayerMask cardLayer;
        [SerializeField] private BoxCollider2D cardCollider;

        private CardModel _cardModel;
        private ICardGroup _cardGroup;
        private Vector3 _initialPosition;
        
        public bool IsSingleCard => this._cardGroup == null || this._cardGroup.IsEmpty;

        public string CardCategory => this._cardModel.cardCategory;
        public CardType CardType => this.cardType;
        public ICardGroup CardGroup => this._cardGroup;
        public Vector3 WorldPosition => this.transform.position;

        private void Awake()
        {
            this._initialPosition = this.transform.position;
        }

        public void MoveToPositionImmediately(Vector3 position)
        {
            this.transform.position = position;
        }

        public void SetOrderLayer(int sortingOrder)
        {
            this.cardSortingGroup.sortingOrder = sortingOrder;
        }

        public void SetCardGroup(ICardGroup cardGroup)
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
                this._cardGroup.FollowPosition(position);
        }

        public void SnapBackToInitialedPosition()
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

        public void AppendCardToGroup(params ICard[] card)
        {
            this._cardGroup ??= new CardGroup();
            this._cardGroup.AppendCards(card);
        }
        
        public bool IsSameCategory(ICard card)
        {
            bool isSameCategory = string.CompareOrdinal(this.CardCategory, card.CardCategory) == 0;
            return isSameCategory;
        }

        public List<ICard> CheckAvailableCardOnDropDown()
        {
            Collider2D[] cardColliders =
                Physics2D.OverlapBoxAll(this.transform.position, this.cardCollider.size, 0, this.cardLayer);
            if (cardColliders is not { Length: > 0 }) 
                return null;
            
            int count = cardColliders.Length;
            List<ICard> result = new();

            for (int i = 0; i < count; i++)
            {
                if (cardColliders[i].gameObject.TryGetComponent(out ICard card))
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
