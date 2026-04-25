using ProjectScope;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace _Solitaire.Scripts.GameplayScene.UI.GameUI
{
    public class GameplayUI : MonoBehaviour
    {
        private const string InfinitySign = "\u221E";
        
        [SerializeField] private TMP_Text moveCountText;
        [SerializeField] private GameObject endGameScreen;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text endGameText;

        private void Awake()
        {
            this.backButton.onClick.AddListener(this.QuitGame);
        }

        private void QuitGame()
        {
            SceneManager.LoadSceneAsync(SceneName.Home);
        }

        public void SetMoveCount(int moveCount)
        {
            string moveCountString = moveCount >= 0 ? $"{moveCount}" : InfinitySign;
            this.moveCountText.text = moveCountString;
        }

        public void ShowEndGameScreen(bool isWinGame)
        {
            this.endGameText.text = isWinGame ? "Win" : "Lose";
            this.endGameScreen.SetActive(true);
        }

        private void OnDestroy()
        {
            this.backButton.onClick.RemoveListener(this.QuitGame);
        }
    }
}
