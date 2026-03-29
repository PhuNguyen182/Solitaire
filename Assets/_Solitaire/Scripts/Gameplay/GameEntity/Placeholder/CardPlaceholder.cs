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
        
        private int _cardPlaceHolderID;
        private ICardGroup _cardGroup;
        
        private void Awake()
        {
            this._cardGroup = new CardGroup(this.cardLayer);
        }

        public int CardPlaceHolderID => this._cardPlaceHolderID;

        public void BindModelData(CardPlaceholderModel model)
        {
            this.cardType = model.CardType;
            this.ToggleFoundationMark(model.CardType == CardType.Foundation);
            this.SetupCardPlaceholderInitialEnableState();
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

            this._cardGroup.AppendCards(card);
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
            this._cardGroup.AppendCards(cards);
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
            
            // To do: Execute flip card logic here
        }

        public void SetCardID(int cardID)
        {
            this._cardPlaceHolderID = cardID;
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
