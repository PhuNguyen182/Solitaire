using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;

namespace _Solitaire.Scripts.Gameplay.GameEntity.Placeholder
{
    public interface ICardPlaceholder
    {
        public bool TryAppendCard(ICard card);
        public void RemoveCard(ICard card);
    }
}