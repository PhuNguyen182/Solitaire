using System;
using _Solitaire.Scripts.GameplayScene.UI.GameUI;
using Stateless;

namespace _Solitaire.Scripts.Gameplay.Controller.StateMachine
{
    public class GameStateMachineController : IDisposable
    {
        private readonly GameplayUI _gameplayUI;
        private StateMachine<GameState, GameTrigger> _stateMachine;
        private StateMachine<GameState, GameTrigger>.TriggerWithParameters<bool> _endGameStateTrigger;

        public GameStateMachineController(GameplayUI gameplayUI)
        {
            this._gameplayUI = gameplayUI;
            this.BuildGameStateMachine();
        }

        private void BuildGameStateMachine()
        {
            this._stateMachine = new StateMachine<GameState, GameTrigger>(GameState.Start);
            this._endGameStateTrigger = this._stateMachine.SetTriggerParameters<bool>(GameTrigger.EndGame);
            
            this._stateMachine.Configure(GameState.Start)
                .Permit(GameTrigger.PlayGame, GameState.Playing)
                .OnActivate(this.GameReady);
            
            this._stateMachine.Configure(GameState.Playing)
                .OnEntryFrom(GameTrigger.PlayGame, this.PlayGame)
                .Permit(this._endGameStateTrigger.Trigger, GameState.EndGame)
                .Permit(GameTrigger.QuitGame, GameState.Quit);
            
            this._stateMachine.Configure(GameState.EndGame)
                .OnEntryFrom(this._endGameStateTrigger, this.OnEndGame)
                .Permit(GameTrigger.QuitGame, GameState.Quit);

            this._stateMachine.Configure(GameState.Quit)
                .OnEntry(this.OnQuitGame);
        }

        public void StartPlayGame()
        {
            this._stateMachine.Activate();
        }

        public void EndGame(bool isWinGame)
        {
            if (this._stateMachine.CanFire(this._endGameStateTrigger.Trigger))
                this._stateMachine.Fire(this._endGameStateTrigger, isWinGame);
        }

        public void QuitGame()
        {
            if (this._stateMachine.CanFire(GameTrigger.QuitGame))
                this._stateMachine.Fire(GameTrigger.QuitGame);
        }

        private void GameReady()
        {
            if (this._stateMachine.CanFire(GameTrigger.PlayGame))
                this._stateMachine.Fire(GameTrigger.PlayGame);
        }

        private void PlayGame()
        {
            
        }

        private void OnEndGame(bool isWinGame)
        {
            this._gameplayUI.ShowEndGameScreen(isWinGame);
        }

        private void OnQuitGame()
        {
            
        }

        public void Dispose()
        {
            this._stateMachine.Deactivate();
        }
    }
}
