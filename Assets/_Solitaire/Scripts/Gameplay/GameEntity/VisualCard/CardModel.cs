using System;
using _Solitaire.Scripts.Gameplay.Controller;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    [Serializable]
    public class CardModel
    {
        public CardType cardType;
        public CardContentType contentType;
        public string cardCategory;
        public string cardContent;
        public PlayCardManager PlayCardManager;
    }
}
