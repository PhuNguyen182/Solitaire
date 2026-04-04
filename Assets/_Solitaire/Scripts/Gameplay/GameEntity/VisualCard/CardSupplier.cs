using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Level;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace _Solitaire.Scripts.Gameplay.GameEntity.VisualCard
{
    public class CardSupplier : MonoBehaviour
    {
        [SerializeField] private PlayingCard cardPrefab;
        [SerializeField] private Button provideCardButton;
        [SerializeField] private Transform[] cardPositions;
        [SerializeField] private int maxSupplyCardCount = 3;
        
        private LevelModel _levelModel; 
        private CardFactory _cardFactory;
        private List<ICard> _supplyCards;

        private void Awake()
        {
            this._supplyCards = new List<ICard>();
        }

        private void OnEnable()
        {
            this.RegisterButtons();
        }

        public void SetLevelModel(LevelModel levelModel)
        {
            this._levelModel = levelModel;
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
                this._supplyCards.Clear();
            }
            else
            {
                int supplyCardCount = this._supplyCards.Count;
                this._levelModel.availableCategories.Shuffle();
                CardFactoryParam cardParam = new CardFactoryParam
                {
                    Position = this.cardPositions[supplyCardCount].position,
                };
                ICard supplyCard = this._cardFactory.Create(cardParam);
                this._supplyCards.Add(supplyCard);
            }
        }

        private void OnDisable()
        {
            this.DeregisterButtons();
        }
    }
}
