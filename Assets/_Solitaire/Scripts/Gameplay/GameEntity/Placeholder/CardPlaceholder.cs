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

        private ICardGroup _cardGroup;
        private CardFactory _cardFactory;
        private CardPlaceholderModel _cardPlaceholderModel;

        public int CardPlaceHolderID { get; private set; }

        public Transform CurrentTransform => this.transform;
        
        private void Awake()
        {
            this._cardGroup = new CardGroup(this.cardLayer);
            this._cardGroup.SetCardPlaceholder(this);
        }

        public void BindModelData(CardPlaceholderModel model)
        {
            this._cardPlaceholderModel = model;
            this.cardType = model.CardType;
            this.ToggleFoundationMark(model.CardType == CardType.Foundation);
            this.SetupCardPlaceholderInitialEnableState();
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
                Vector3 cardPosition = this.transform.position + Vector3.down * (i * CardConstants.CardPositionOffset);
                CardFactoryParam param = new CardFactoryParam
                {
                    CardModel = cardModel,
                    CardPlaceholder = this,
                    Position = cardPosition,
                };
                
                ICard cardInstance = this._cardFactory.Create(param);
                this.TryAppendCard(cardInstance);
            }
        }

        #region Append Cards
        
        public bool TryAppendCard(ICard card)
        {
            bool result = card.IsSingleCard 
                ? this.AppendSingleCard(card) 
                : this.AppendMultipleCards(card);
            this.CheckCardPlaceholderCollider();
            return result;
        }

        private bool AppendSingleCard(ICard card)
        {
            if (this.cardType == CardType.Foundation)
            {
                if (card.CardType != CardType.Foundation) 
                    return false;
            }

            this._cardGroup.AppendCards(false, card);
            return true;
        }

        private bool AppendMultipleCards(ICard card)
        {
            if (this.cardType == CardType.Foundation)
            {
                if (!card.CardGroup.ContainFoundationCard())
                    return false;
            }

            ICard[] cards = card.CardGroup.ElementCards.ToArray();
            this._cardGroup.AppendCards(false, cards);
            return true;
        }

        #endregion

        public void RemoveCard(ICard card)
        {
            this._cardGroup.RemoveCard(card);
            this.CheckCardPlaceholderCollider();
        }

        public void FlipLastCard()
        {
            if (this.cardType == CardType.Foundation)
                return;

            ICard lastCard = this._cardGroup.GetLastCard();
            lastCard.FlipCard(true, true);
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
    }
}
