using _Solitaire.Scripts.Gameplay.GameEntity.CardGroup;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.Placeholder
{
    public class CardPlaceholder : MonoBehaviour
    {
        [SerializeField] private CardType cardType;
        [SerializeField] private GameObject foundationMark;
        
        public CardType CardType => cardType;
        
        private CardGroup.CardGroup _cardGroup;

        private void Awake()
        {
            this._cardGroup = new CardGroup.CardGroup();
        }

        public void AppendCard(Card card)
        {
            this._cardGroup.AppendCards(card);
            // TODO: Add card to visual card, update card group data and update card positions
        }

        public void RemoveCard(Card card)
        {
            this._cardGroup.RemoveCard(card);
            // TODO: Add card to visual card, update card group data and update card positions
        }

        private void ToggleFoundationMark(bool isFoundation)
        {
            if (this.foundationMark)
                this.foundationMark.SetActive(isFoundation);
        }
    }
}
