using System.Linq;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Controller;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers;
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

        public void InitServices(PlayCardManager playCardManager, CardFactory cardFactory,
            CardSupplyProbabilityConfigDataController dataController)
        {
            this._cardFactory = cardFactory;
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

        public void TryRemoveCardExternally(ICard card)
        {
            if (!this._supplyCards.Contains(card))
                return;
            
            this._supplyCards.Remove(card);
            this.RecalculateSuppliedCardPositionsAndLayers();
        }

        private void RegisterButtons()
        {
            this.provideCardButton.onClick.AddListener(this.ProvideCard);
        }
        
        private void DeregisterButtons()
        {
            this.provideCardButton.onClick.RemoveListener(this.ProvideCard);
        }

        private void RecalculateSuppliedCardPositionsAndLayers()
        {
            int count = this._supplyCards.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 recalculatedPosition = this.cardPositions[i].position;
                this._supplyCards[i].MoveToPositionImmediately(recalculatedPosition);
                this._supplyCards[i].SetOrderLayer(i);
            }
        }

        private void ProvideCard()
        {
            if (this._supplyCards.Count >= this.maxSupplyCardCount)
            {
                foreach (ICard card in this._supplyCards)
                {
                    this._wordPool.AddNewWordByCategory(card.CardCategory, card.CardModel);
                    card.Cleanup();
                }
                
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
                supplyCard.UpdateCardPlacingState(false);
                supplyCard.FlipCard(true, true);
                this._supplyCards.Add(supplyCard);
                this._wordPool.RemoveWordByCategory(providedCardModel.cardCategory, providedCardModel);
                this.RecalculateSuppliedCardPositionsAndLayers();
            }
        }

        private CardModel GetSuppliedCardModel()
        {
            CardModel cardModel;
            float random = Random.value;
            if (random > this._generousProbability)
            {
                HashSet<string> incompletedCategory = this._playCardManager.GetIncompletedCategories();
                string randomCategory = incompletedCategory.GetRandomElement();
                cardModel = this._wordPool.GetRandomWordByCategory(randomCategory);
                return cardModel;
            }
            
            string cardCategory = this._playCardManager.GetRandomGenerousCategory();
            cardModel = this._wordPool.GetRandomWordByCategory(cardCategory);
            return cardModel;
        }

        private void OnDisable()
        {
            this.DeregisterButtons();
        }
    }
}
