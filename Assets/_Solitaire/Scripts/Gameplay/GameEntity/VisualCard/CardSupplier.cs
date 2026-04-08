using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Controller;
using _Solitaire.Scripts.Gameplay.Level;
using UnityEngine;
using UnityEngine.UI;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class CardSupplier : MonoBehaviour
    {
        [SerializeField] private PlayingCard cardPrefab;
        [SerializeField] private Button provideCardButton;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private Transform[] cardPositions;
        [SerializeField] private int maxSupplyCardCount = 3;
        
        private LevelModel _levelModel; 
        private CardFactory _cardFactory;
        private PlayCardManager _playCardManager;
        private List<ICard> _supplyCards;
        private WordPool _wordPool;

        private void Awake()
        {
            this._supplyCards = new List<ICard>();
        }

        private void OnEnable()
        {
            this.RegisterButtons();
        }
        
        public void SetPlayCardManager(PlayCardManager playCardManager) => this._playCardManager = playCardManager;

        public void SetLevelModel(LevelModel levelModel)
        {
            this._levelModel = levelModel;
            this.BuildWordPoolForCardSupplier(levelModel);
        }

        private void BuildWordPoolForCardSupplier(LevelModel levelModel)
        {
            this._wordPool = new WordPool();
            foreach (CategoryData categoryData in levelModel.availableCategories)
            {
                int numberOfWord = categoryData.maxCardCount;
                string categoryName = categoryData.categoryName;
                
                CardModelByCategory cardModelByCategory = new CardModelByCategory(categoryName, numberOfWord);
                foreach (var cardModel in categoryData.cards)
                    cardModelByCategory.AddCardModel(cardModel);
                
                this._wordPool.AddWordCategory(cardModelByCategory);
            }
        }

        private void RegisterButtons()
        {
            this.provideCardButton.onClick.AddListener(this.ProvideCard);
        }
        
        private void DeregisterButtons()
        {
            this.provideCardButton.onClick.RemoveListener(this.ProvideCard);
        }

        private void ProvideCard()
        {
            /*
             * 1. Check in spawned cards, if exceed maxSupplyCardCount, regain all supply card into deck
             * 2. Check in full board, spawn a random card but different from all available cards in the board. The spawned card
             * must be included in available card group in level model
             */
            // Sample code here

            if (this._supplyCards.Count >= this.maxSupplyCardCount)
            {
                // Regain all cards
                foreach (ICard card in this._supplyCards)
                    card.Cleanup();
                this._supplyCards.Clear();
            }
            else
            {
                int supplyCardCount = this._supplyCards.Count;
                CardModel providedCardModel = this._wordPool.GetFullyRandomWord();
                providedCardModel.PlayCardManager = this._playCardManager;
                CardFactoryParam cardParam = new CardFactoryParam
                {
                    CardContainer = this.cardContainer,
                    Position = this.cardPositions[supplyCardCount].position,
                    CardModel = providedCardModel,
                };
                ICard supplyCard = this._cardFactory.Create(cardParam);
                supplyCard.FlipCard(true, true);
                this._supplyCards.Add(supplyCard);
            }
        }

        private void OnDisable()
        {
            this.DeregisterButtons();
            this._wordPool?.Dispose();
        }
    }
}
