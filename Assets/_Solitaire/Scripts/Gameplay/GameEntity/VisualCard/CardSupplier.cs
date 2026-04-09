using System.Linq;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Controller;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers;
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

        private float _generousProbability;
        private CardFactory _cardFactory;
        private PlayCardManager _playCardManager;
        private CardSupplyProbabilityConfigDataController _cardSupplyProbabilityConfigDataController;
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

        public void InitServices(PlayCardManager playCardManager,
            CardSupplyProbabilityConfigDataController dataController)
        {
            this._playCardManager = playCardManager;
            this._cardSupplyProbabilityConfigDataController = dataController;
            this._generousProbability = this._cardSupplyProbabilityConfigDataController.GetGenerousProbability();
        }

        public void SetLevelModel(LevelModel levelModel, WordPool wordPool)
        {
            this._wordPool = wordPool;
            HashSet<string> cardCategories =
                levelModel.availableCategories.Select(data => data.categoryName).ToHashSet();
            this._playCardManager.InitializeCardCategories(cardCategories);
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
            if (this._supplyCards.Count >= this.maxSupplyCardCount)
            {
                foreach (ICard card in this._supplyCards)
                    card.Cleanup();
                this._supplyCards.Clear();
            }
            else
            {
                int supplyCardCount = this._supplyCards.Count;
                CardModel providedCardModel = this.GetSuppliedCardModel();
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

        private CardModel GetSuppliedCardModel()
        {
            CardModel cardModel;
            float random = Random.value;
            if (random > this._generousProbability)
            {
                cardModel = this._wordPool.GetFullyRandomWord();
                return cardModel;
            }
            
            string cardCategory = this._playCardManager.GetRandomGenerousCategory();
            cardModel = this._wordPool.GetRandomWordByCategory(cardCategory);
            return cardModel;
        }

        private void OnDisable()
        {
            this.DeregisterButtons();
            this._wordPool?.Dispose();
        }
    }
}
