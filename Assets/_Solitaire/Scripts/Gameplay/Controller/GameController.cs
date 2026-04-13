using _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers;
using _Solitaire.Scripts.Gameplay.Level;
using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using DracoRuan.Foundation.DataFlow.MasterDataController;
using Cysharp.Threading.Tasks;
using ServiceLocators.Core;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class GameController : MonoBehaviour
    {
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

        private bool _isDataInitialized;
        private CardFactory _cardFactory;
        private PlayCardManager _playCardManager;
        private CardPlaceholderManager _cardPlaceholderManager;
        private RawLevelConfigDataController _rawLevelConfigDataController;
        private CardSupplyProbabilityConfigDataController _cardSupplyProbabilityConfigDataController;
        private MainDataManager _mainDataManager;
        private LevelManager _levelManager;
        private WordPool _wordPool;

        private void Awake()
        {
            this.TestInitializeData().Forget();
        }

        private void Start()
        {
            this.StartGameLevel();
        }

        private async UniTask TestInitializeData()
        {
            this._mainDataManager = new MainDataManager();
            await this._mainDataManager.InitializeDataHandlers();
            this._rawLevelConfigDataController =
                this._mainDataManager.GetStaticDataController<RawLevelConfigDataController>();
            this._cardSupplyProbabilityConfigDataController = this._mainDataManager
                .GetStaticDataController<CardSupplyProbabilityConfigDataController>();
            this._isDataInitialized = true;
            this.InitializeGame();
        }

        private void InitializeGame()
        {
            this._playCardManager = new PlayCardManager();
            this._cardFactory = new CardFactory(this.playingCardPrefab, this.mainCamera);
            this.dragAndDropController.SetPlayCardManager(this._playCardManager);
            this.cardSupplier.InitServices(this._playCardManager, this._cardFactory,
                this._cardSupplyProbabilityConfigDataController);
            ServiceLocator.ForSceneOf(this).Register(this.cardSupplier);
        }

        private void StartGameLevel()
        {
            LevelModel levelModel = this._rawLevelConfigDataController.BuildLevelModel(1, this._playCardManager);
            this.InitializeWordPool(levelModel);
            this.SetupLevelModel(levelModel).Forget();
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
        }

        private async UniTask SetupLevelModel(LevelModel levelModel)
        {
            await UniTask.WaitUntil(() => this._isDataInitialized);
            this._levelManager = new LevelManager(levelModel, this._playCardManager);
            this._cardPlaceholderManager = new CardPlaceholderManager(this.cardPlaceholderPrefab,
                this.foundationPlaceholderStartPoint.position, this.normalPlaceholderStartPoint.position,
                this._playCardManager, this._levelManager, this.cardPlaceholderContainer, this.cardContainerPoint,
                this._cardFactory, this._wordPool);
            this._cardPlaceholderManager.BuildLevel(levelModel);
            this.dragAndDropController.SetCardPlaceholderManager(this._cardPlaceholderManager);
            this.cardSupplier.SetLevelModel(levelModel, this._wordPool);
        }

        private void OnDestroy()
        {
            this._playCardManager.Dispose();
            this._wordPool?.Dispose();
        }
    }
}
