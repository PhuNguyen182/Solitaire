using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Controller;
using _Solitaire.Scripts.Gameplay.GameEntity.Group;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.Placeholder
{
    public class CardPlaceholder : MonoBehaviour, ICardPlaceholder
    {
        [SerializeField] private CardType cardType;
        [SerializeField] private GameObject foundationMark;
        [SerializeField] private BoxCollider2D placeholderCollider;
        [SerializeField] private LayerMask cardLayer;
        [SerializeField] private Canvas cardCanvas;

        private ICard _foundationCard;
        private ICardGroup _cardGroup;
        private CardFactory _cardFactory;
        private CardPlaceholderModel _cardPlaceholderModel;
        private PlayCardManager _playCardManager;
        private Transform _cardContainer;
        private WordPool _wordPool;

        public int CardPlaceHolderID { get; private set; }
        public CardType CardType => this.cardType;
        public string CardCategory => this._cardGroup?.CardCategory;
        public Transform CurrentTransform => this.transform;
        public ICard FoundationCard => this._foundationCard;
        public bool IsEmpty => this._cardGroup.IsEmpty;
        
        private void Awake()
        {
            this.cardCanvas.worldCamera = Camera.main;
            this._cardGroup = new CardGroup(this.cardLayer);
            this._cardGroup.SetCardPlaceholder(this);
        }

        public void BindModelData(CardPlaceholderModel model)
        {
            this._cardPlaceholderModel = model;
            this.cardType = model.CardType;
            this._playCardManager = model.PlayCardManager;
            this._wordPool = model.WordPool;
            this.ToggleFoundationMark(model.CardType == CardType.Foundation);
            this.SetupCardPlaceholderInitialEnableState();
        }

        public void SetCardContainer(Transform cardContainer)
        {
            this._cardContainer = cardContainer;
        }

        public void SetCardFactory(CardFactory cardFactory)
        {
            this._cardFactory = cardFactory;
        }

        public void BuildCardColumn()
        {
            if (this._cardPlaceholderModel.CardColumnModel == null)
                return;

            int count = this._cardPlaceholderModel.CardColumnModel.cardModel.Count;
            for (int i = 0; i < count; i++)
            {
                CardModel cardModel = this._cardPlaceholderModel.CardColumnModel.cardModel[i];
                cardModel.PlayCardManager = this._playCardManager;
                Vector3 cardPosition = this.transform.position + Vector3.down * (i * CardConstants.CardPositionOffset);
                CardFactoryParam param = new CardFactoryParam
                {
                    CardContainer = this._cardContainer,
                    CardModel = cardModel,
                    CardPlaceholder = this,
                    Position = cardPosition,
                };
                
                ICard cardInstance = this._cardFactory.Create(param);
                cardInstance.UpdateCardPlacingState(true);
                if (i == count - 1)
                    cardInstance.FlipCard(true, true);

                this._wordPool.RemoveWordByCategory(cardInstance.CardCategory, cardInstance.CardModel);
                cardInstance.SetCardPlaceholder(this);
                this.TryAppendCard(cardInstance);
            }
        }

        #region Append Cards
        
        public bool TryAppendCard(ICard card)
        {
            bool assignGroupToCard = this.cardType == CardType.Foundation;
            bool result = card.IsSingleCard 
                ? this.AppendSingleCard(card, assignGroupToCard) 
                : this.AppendMultipleCards(card, assignGroupToCard);
            this.CheckCardPlaceholderCollider();
            if (result && assignGroupToCard)
                this._foundationCard = card;

            if (!assignGroupToCard && card.CardGroup != null)
                card.CardGroup.ReupdateCardGroupInitialPosition(this.transform.position);
            
            return result;
        }

        private bool AppendSingleCard(ICard card, bool assignGroupToCard)
        {
            if (this.cardType == CardType.Foundation)
            {
                if (card.CardType != CardType.Foundation) 
                    return false;
            }

            this._cardGroup.AppendCards(assignGroupToCard, this.transform.position, card);
            this._playCardManager.AddCard(card);
            return true;
        }

        private bool AppendMultipleCards(ICard card, bool assignGroupToCard)
        {
            if (this.cardType == CardType.Foundation)
            {
                if (!card.CardGroup.ContainFoundationCard())
                    return false;
            }

            int cardCount = card.CardGroup.ElementCards.Count;
            List<ICard> cards = card.CardGroup.ElementCards;
            for (int i = 0; i < cardCount; i++)
            {
                this._cardGroup.AppendCards(assignGroupToCard, this.transform.position,
                    cards[i]);
            }
            
            for (int i = 0; i < cardCount; i++)
                this._playCardManager.AddCard(cards[i]);
            
            return true;
        }

        #endregion

        public void RemoveCard(ICard card)
        {
            List<ICard> cards = card?.CardGroup?.ElementCards;
            if (cards is not { Count: > 0 })
                this._cardGroup.RemoveCard(card);
            else
            {
                foreach (ICard cardToRemove in cards)
                    this._cardGroup.RemoveCard(cardToRemove);
            }

            this.CheckCardPlaceholderCollider();
        }

        public void FlipLastCard()
        {
            if (this.cardType == CardType.Foundation)
                return;

            ICard lastCard = this._cardGroup.GetLastCard();
            if (lastCard == null) 
                return;
            
            lastCard.FlipCard(true, true);
            this._wordPool.RemoveWordByCategory(lastCard.CardCategory, lastCard.CardModel);
        }

        public void SetCardID(int cardID)
        {
            this.CardPlaceHolderID = cardID;
        }

        private void CheckCardPlaceholderCollider()
        {
            this.placeholderCollider.enabled = this._cardGroup.IsEmpty;
        }

        private void SetupCardPlaceholderInitialEnableState()
        {
            this.placeholderCollider.enabled = this.cardType == CardType.Foundation;
        }

        private void ToggleFoundationMark(bool isFoundation)
        {
            if (this.foundationMark)
                this.foundationMark.SetActive(isFoundation);
        }

        public void Cleanup()
        {
            this._cardGroup.Cleanup();
            this.CheckCardPlaceholderCollider();
        }

        private void OnDisable()
        {
            this.Cleanup();
        }
    }
}
