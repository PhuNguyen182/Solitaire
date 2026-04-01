using _Solitaire.Scripts.Gameplay.GameEntity.Placeholder;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using _Solitaire.Scripts.Gameplay.Level;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Transform cardPlaceholderContainer;
        [SerializeField] private Transform foundationPlaceholderStartPoint;
        [SerializeField] private Transform normalPlaceholderStartPoint;
        [SerializeField] private CardPlaceholder cardPlaceholderPrefab;
        [SerializeField] private PlayingCard playingCardPrefab;
        [SerializeField] private CardSupplier cardSupplier;
        
        private CardFactory _cardFactory;
        private CardPlaceholderManager _cardPlaceholderManager;

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
            this._cardFactory = new CardFactory(this.playingCardPrefab);
            this._cardPlaceholderManager = new CardPlaceholderManager(this.cardPlaceholderPrefab,
                this.foundationPlaceholderStartPoint.position, this.normalPlaceholderStartPoint.position,
                this.cardPlaceholderContainer, this._cardFactory);
        }

        private void SetupLevelModel(LevelModel levelModel)
        {
            this.cardSupplier.SetLevelModel(levelModel);
        }

        private void StartGameLevel()
        {
            
        }
    }
}
