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
        private LevelController _levelController;
        private MainDataManager _mainDataManager;

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
            this._cardPlaceholderManager = new CardPlaceholderManager(this.cardPlaceholderPrefab,
                this.foundationPlaceholderStartPoint.position, this.normalPlaceholderStartPoint.position,
                this._playCardManager, this.cardPlaceholderContainer, this.cardContainerPoint, this._cardFactory);
            this.dragAndDropController.SetPlayCardManager(this._playCardManager);
        }

        private void SetupLevelModel(LevelModel levelModel)
        {
            this._levelController = new LevelController(levelModel, this._playCardManager);
            this.cardSupplier.SetLevelModel(levelModel);
        }

        private void StartGameLevel()
        {
            
        }

        private void OnDestroy()
        {
            this._playCardManager.Dispose();
        }
    }
}
