using _Solitaire.Scripts.Gameplay.Level;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers;
using _Solitaire.Scripts.Gameplay.Controller.StateMachine;
using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using _Solitaire.Scripts.GameplayScene.UI.GameUI;
using DracoRuan.Foundation.DataFlow.MasterDataController;
using ServiceLocators.Core;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameplayUI gameplayUI;
        [SerializeField] private Camera mainCamera;
        
        [Header("Card Mapping")]
        [SerializeField] private Transform cardPlaceholderContainer;
        [SerializeField] private Transform foundationPlaceholderStartPoint;
        [SerializeField] private Transform normalPlaceholderStartPoint;
        [SerializeField] private Transform cardContainerPoint;
        
        [Header("Card Controlling")]
        [SerializeField] private DragAndDropController dragAndDropController;
        [SerializeField] private CardPlaceholder cardPlaceholderPrefab;
        [SerializeField] private PlayingCard playingCardPrefab;
        [SerializeField] private CardSupplier cardSupplier;
        
        private CardFactory _cardFactory;
        private PlayCardManager _playCardManager;
        private CardPlaceholderManager _cardPlaceholderManager;
        private GameStateMachineController _stateMachineController;
        private PlayerProgressionDataController _playerProgressionDataController;
        private CardSupplyProbabilityConfigDataController _cardSupplyProbabilityConfigDataController;
        private RawLevelConfigDataController _rawLevelConfigDataController;
        private GameResultChecker _gameResultChecker;
        private LevelManager _levelManager;
        private WordPool _wordPool;

        private void Awake()
        {
            this.InitializeGame();
        }

        private void Start()
        {
            this.StartGameLevel();
        }

        private void InitializeGame()
        {
            var mainDataManager = ServiceLocator.Global.Get<MainDataManager>();
            this._rawLevelConfigDataController =
                mainDataManager.GetStaticDataController<RawLevelConfigDataController>();
            this._cardSupplyProbabilityConfigDataController = mainDataManager
                .GetStaticDataController<CardSupplyProbabilityConfigDataController>();
            this._playerProgressionDataController =
                mainDataManager.GetDynamicDataController<PlayerProgressionDataController>();
            
            this._playCardManager = new PlayCardManager();
            this._stateMachineController = new GameStateMachineController(this.gameplayUI);
            this._cardFactory = new CardFactory(this.playingCardPrefab, this.mainCamera);
            this.dragAndDropController.SetPlayCardManager(this._playCardManager);
            this.cardSupplier.InitServices(this._playCardManager, this._cardFactory,
                this._cardSupplyProbabilityConfigDataController, this.gameplayUI);
            ServiceLocator.ForSceneOf(this).Register(this.cardSupplier);
        }

        private void StartGameLevel()
        {
            int level = this._playerProgressionDataController.GetCurrentPlayLevel();
            LevelModel levelModel = this._rawLevelConfigDataController.BuildLevelModel(level, this._playCardManager);
            this.InitializeWordPool(levelModel);
            this.SetupLevelModel(levelModel);
            this.SetupGameResultChecker();
            this._stateMachineController.StartPlayGame();
        }

        private void InitializeWordPool(LevelModel levelModel)
        {
            this._wordPool = new WordPool();
            foreach (CategoryData categoryData in levelModel.availableCategories)
            {
                int numberOfWord = categoryData.maxCardCount;
                string categoryName = categoryData.categoryName;
                
                CardModelByCategory cardModelByCategory = new CardModelByCategory(categoryName, numberOfWord);
                foreach (var cardModel in categoryData.cards)
                {
                    cardModelByCategory.AddCardModel(cardModel);
                    if (this._playCardManager.ContainWord(cardModel.cardCategory, cardModel.cardContent))
                        cardModelByCategory.RemoveCardModel(cardModel);
                }
                
                this._wordPool.AddWordCategory(cardModelByCategory);
            }
            
            this.dragAndDropController.SetWordPool(this._wordPool);
            ServiceLocator.ForSceneOf(this).Register(this._wordPool);
        }

        private void SetupGameResultChecker()
        {
            this._gameResultChecker =
                new GameResultChecker(this._playCardManager, this._stateMachineController, this.cardSupplier);
        }

        private void SetupLevelModel(LevelModel levelModel)
        {
            this._levelManager = new LevelManager(levelModel, this._playCardManager);
            ServiceLocator.ForSceneOf(this).Register(this._levelManager);

            this._cardPlaceholderManager = new CardPlaceholderManager(this.cardPlaceholderPrefab,
                this.foundationPlaceholderStartPoint.position, this.normalPlaceholderStartPoint.position,
                this._playCardManager, this.cardPlaceholderContainer, this.cardContainerPoint, this._cardFactory, 
                this._wordPool);
            this.dragAndDropController.SetLevelManager(this._levelManager);
            this._cardPlaceholderManager.BuildLevel(levelModel);
            this.dragAndDropController.SetCardPlaceholderManager(this._cardPlaceholderManager);
            this.cardSupplier.SetLevelModel(levelModel, this._wordPool);
            this._wordPool.HasWordPoolInitialized = true;
        }

        private void OnDestroy()
        {
            this._gameResultChecker.Dispose();
            this._playCardManager.Dispose();
            this._wordPool?.Dispose();
        }
    }
}
