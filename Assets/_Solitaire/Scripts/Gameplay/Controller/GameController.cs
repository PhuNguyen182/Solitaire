using _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers;
using _Solitaire.Scripts.Gameplay.Level;
using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using DracoRuan.Foundation.DataFlow.MasterDataController;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class GameController : MonoBehaviour
    {
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
        private RawLevelConfigDataController _rawLevelConfigDataController;
        private MainDataManager _mainDataManager;
        private LevelManager _levelManager;

        private void Awake()
        {
            this.TestInitializeData();
            this.InitializeGame();
        }

        private void Start()
        {
            this.StartGameLevel();
        }

        private void TestInitializeData()
        {
            this._mainDataManager = new MainDataManager();
            this._mainDataManager.InitializeDataHandlers().Forget();
        }

        private void InitializeGame()
        {
            this._playCardManager = new PlayCardManager();
            this._cardFactory = new CardFactory(this.playingCardPrefab);
            this.dragAndDropController.SetPlayCardManager(this._playCardManager);
            this.cardSupplier.SetPlayCardManager(this._playCardManager);
        }

        private void StartGameLevel()
        {
            LevelModel levelModel = this._rawLevelConfigDataController.BuildLevelModel(1);
            this.SetupLevelModel(levelModel);
        }

        private void SetupLevelModel(LevelModel levelModel)
        {
            this._levelManager = new LevelManager(levelModel, this._playCardManager);
            this._cardPlaceholderManager = new CardPlaceholderManager(this.cardPlaceholderPrefab,
                this.foundationPlaceholderStartPoint.position, this.normalPlaceholderStartPoint.position,
                this._playCardManager, this._levelManager, this.cardPlaceholderContainer, this.cardContainerPoint,
                this._cardFactory);
            this.cardSupplier.SetLevelModel(levelModel);
            this._cardPlaceholderManager.BuildLevel(levelModel);
        }

        private void OnDestroy()
        {
            this._playCardManager.Dispose();
        }
    }
}
