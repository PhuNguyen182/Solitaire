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
        
        private CardGroupData _cardGroupData;

        private void Awake()
        {
            this._cardGroupData = new CardGroupData();
        }

        public void AppendCard(Card card)
        {
            this._cardGroupData.AppendCards(card);
        }

        public void RemoveCard(Card card)
        {
            this._cardGroupData.RemoveCard(card);
        }

        private void ToggleFoundationMark(bool isFoundation)
        {
            if (this.foundationMark)
                this.foundationMark.SetActive(isFoundation);
        }
    }
}
