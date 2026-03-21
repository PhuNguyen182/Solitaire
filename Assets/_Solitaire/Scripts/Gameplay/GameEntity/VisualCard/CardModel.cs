using System;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    [Serializable]
    public class CardModel
    {
        public CardType cardType;
        public CardContentType contentType;
        public string cardContent;
    }
}
