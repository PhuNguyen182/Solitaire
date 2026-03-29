using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using _Solitaire.Scripts.Gameplay.Level;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.GameEntity.Placeholder
{
    public class CardPlaceholderManager
    {
        private readonly CardPlaceholderFactory _cardPlaceholderFactory;
        private readonly Vector3 _foundationPlaceholderStartPosition;
        private readonly Vector3 _normalPlaceholderStartPosition;
        private readonly Dictionary<int, ICardPlaceholder> _cardPlaceholderMap;

        public CardPlaceholderManager(ICardPlaceholder cardPlaceholder, Vector3 foundationPlaceholderStartPosition,
            Vector3 normalPlaceholderStartPosition, Transform cardPlaceholderParentTransform)
        {
            this._cardPlaceholderMap = new Dictionary<int, ICardPlaceholder>();
            this._foundationPlaceholderStartPosition = foundationPlaceholderStartPosition;
            this._normalPlaceholderStartPosition = normalPlaceholderStartPosition;
            this._cardPlaceholderFactory = new CardPlaceholderFactory(cardPlaceholder, cardPlaceholderParentTransform);
        }

        public void CreateCardPlaceholdersFromLevel(LevelModel levelModel)
        {
            int columnCount = levelModel.NumberOfColumns;
            for (int i = 0; i < columnCount; i++)
            {
                AddCardPlaceholderToMap(i, CardType.Foundation);
                AddCardPlaceholderToMap(i, CardType.Normal);
            }

            return;

            void AddCardPlaceholderToMap(int columnIndex, CardType cardType)
            {
                int cardPlaceholderKey = GetCardPlaceholderKey(columnIndex, cardType);
                ICardPlaceholder cardPlaceholder = CreateCardPlaceholder(columnIndex, cardType);
                cardPlaceholder.SetCardID(cardPlaceholderKey);
                this._cardPlaceholderMap.Add(cardPlaceholderKey, cardPlaceholder);
            }

            ICardPlaceholder CreateCardPlaceholder(int columnIndex, CardType cardType)
            {
                Vector3 startPosition = cardType == CardType.Foundation
                    ? this._foundationPlaceholderStartPosition
                    : this._normalPlaceholderStartPosition;
                Vector3 step = Vector3.right * CardConstants.CardColumnDistance * columnIndex;
                Vector3 cardPlaceholderPosition = startPosition + step;
                CardPlaceholderModel cardPlaceholderModel = new CardPlaceholderModel
                {
                    Position = cardPlaceholderPosition,
                    CardType = cardType
                };

                ICardPlaceholder foundationCardPlaceholder = this._cardPlaceholderFactory.Create(cardPlaceholderModel);
                return foundationCardPlaceholder;
            }

            int GetCardPlaceholderKey(int columnIndex, CardType cardType)
            {
                int cardTypeKey = (int)cardType;
                string formattedKey = $"{cardTypeKey}{columnIndex:D2}";
                int cardPlaceholderKey = int.Parse(formattedKey);
                return cardPlaceholderKey;
            }
        }
        
        public ICardPlaceholder GetCardPlaceholder(int cardPlaceholderKey)
        {
            return this._cardPlaceholderMap.GetValueOrDefault(cardPlaceholderKey);
        }
    }
}
