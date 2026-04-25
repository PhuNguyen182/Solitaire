using System;
using _Solitaire.Scripts.Gameplay.Controller.StateMachine;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class GameResultChecker : IDisposable
    {
        private readonly WordPool _wordPool;
        private readonly CardSupplier _cardSupplier;
        private readonly GameStateMachineController _gameStateMachineController;

        private int _currentMove;
        private bool _isLevelCleared;

        public GameResultChecker(WordPool wordPool, CardSupplier cardSupplier,
            GameStateMachineController gameStateMachineController)
        {
            this._wordPool = wordPool;
            this._cardSupplier = cardSupplier;
            this._gameStateMachineController = gameStateMachineController;
            this.RegisterEvent();
        }

        private void RegisterEvent()
        {
            this._wordPool.OnWordPoolCleaning += this.CheckLevelClear;
            this._cardSupplier.OnCardSupply += this.CheckLevelMoveCount;
        }

        private void UnregisterEvent()
        {
            this._wordPool.OnWordPoolCleaning -= this.CheckLevelClear;
            this._cardSupplier.OnCardSupply -= this.CheckLevelMoveCount;
        }

        private void CheckLevelClear(bool isLevelCleared)
        {
            this._isLevelCleared = isLevelCleared;
            this.CheckLevelResult();
        }

        private void CheckLevelMoveCount(int moveCount)
        {
            this._currentMove = moveCount;
            this.CheckLevelResult();
        }

        private void CheckLevelResult()
        {
            switch (this._isLevelCleared)
            {
                case true when this._currentMove >= 0:
                    this._gameStateMachineController.EndGame(true);
                    break;
                case false when this._currentMove == 0:
                    this._gameStateMachineController.EndGame(false);
                    break;
            }
        }

        public void Dispose()
        {
            this._gameStateMachineController?.Dispose();
            this.UnregisterEvent();
        }
    }
}
