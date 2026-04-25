using ProjectScope;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers;
using DracoRuan.Foundation.DataFlow.MasterDataController;
using ServiceLocators.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace _Solitaire.Scripts.HomeScene.UI.MainUI
{
    public class MainHomeUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private TMP_InputField levelInput;
        [SerializeField] private bool isCheatMode = true;

        private PlayerProgressionDataController _playerProgressionDataController;
        
        private void Awake()
        {
            this.RegisterServices();
            this.RegisterUIElements();
        }

        private void RegisterServices()
        {
            var mainDataManager = ServiceLocator.Global.Get<MainDataManager>();
            this._playerProgressionDataController =
                mainDataManager.GetDynamicDataController<PlayerProgressionDataController>();
        }

        private void RegisterUIElements()
        {
            this.playButton.onClick.AddListener(this.PlayLevel);
            this.levelInput.onValueChanged.AddListener(this.OnLevelInputFieldUpdate);
        }
        
        private void UnregisterUIElements()
        {
            this.playButton.onClick.RemoveListener(this.PlayLevel);
            this.levelInput.onValueChanged.RemoveListener(this.OnLevelInputFieldUpdate);
        }

        private void PlayLevel()
        {
            int level = int.Parse(this.levelInput.text);
            this._playerProgressionDataController.IsCheatMode = this.isCheatMode;
            this._playerProgressionDataController.SelectCheatingLevel(level);
            SceneManager.LoadSceneAsync(SceneName.Gameplay);
        }

        private void OnLevelInputFieldUpdate(string input)
        {
            bool isInputValid = !string.IsNullOrEmpty(input);
            if (!isInputValid)
            {
                this.playButton.interactable = false;
                return;
            }
            
            int level = int.Parse(input);
            if (level < 1)
                this.levelInput.text = "1";
            this.playButton.interactable = true;
        }

        private void OnDestroy()
        {
            this.UnregisterUIElements();
        }
    }
}
