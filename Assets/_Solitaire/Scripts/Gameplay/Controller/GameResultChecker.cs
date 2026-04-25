using System;
using _Solitaire.Scripts.Gameplay.Controller.StateMachine;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;

namespace _Solitaire.Scripts.Gameplay.Controller
{
    public class GameResultChecker : IDisposable
    {
        private readonly PlayCardManager _playCardManager;
        private readonly GameStateMachineController _gameStateMachineController;
        private readonly CardSupplier _cardSupplier;

        private int _currentMove;
        private bool _isLevelCleared;

        public GameResultChecker(PlayCardManager playCardManager, GameStateMachineController gameStateMachineController,
            CardSupplier cardSupplier)
        {
            this._playCardManager = playCardManager;
            this._gameStateMachineController = gameStateMachineController;
            this._cardSupplier = cardSupplier;
            this.RegisterEvent();
        }

        private void RegisterEvent()
        {
            this._playCardManager.OnLevelCleaning += this.CheckLevelClear;
            this._cardSupplier.OnCardSupply += this.CheckLevelMoveCount;
        }

        private void UnregisterEvent()
        {
            this._playCardManager.OnLevelCleaning -= this.CheckLevelClear;
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
                case false when this._currentMove == 0 && !this._cardSupplier.IsInfinityMove:
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
