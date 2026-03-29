using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;

namespace _Solitaire.Scripts.Gameplay.GameEntity.Placeholder
{
    public interface ICardPlaceholder
    {
        public int CardPlaceHolderID { get; }
        
        public void BindModelData(CardPlaceholderModel model);
        public bool TryAppendCard(ICard card);
        public void RemoveCard(ICard card);
        public void FlipLastCard();
        public void SetCardID(int cardID);
    }
}