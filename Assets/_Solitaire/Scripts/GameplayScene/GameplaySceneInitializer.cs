using Cysharp.Threading.Tasks;
using DracoRuan.Foundation.UISystem.Canvases;
using DracoRuan.Foundation.UISystem.Popups.PopupManager;
using ServiceLocators.Core;
using UnityEngine;

namespace _Solitaire.Scripts.GameplayScene
{
    public class GameplaySceneInitializer : MonoBehaviour
    {
        [SerializeField] private PopupCollection popupCollection;
        [SerializeField] private UICanvasManager uiCanvasManager;
        
        private IUIPopupManager _popupManager;
        
        private void Awake()
        {
            this.RegisterServices().Forget();
        }

        private async UniTask RegisterServices()
        {
            await this.RegisterUIManager();
        }

        private async UniTask RegisterUIManager()
        {
            await UniTask.CompletedTask;
            ServiceLocator.ForSceneOf(this).Register(typeof(IUIPopupManager), this.uiCanvasManager);
            this._popupManager = new UIPopupManager(this.popupCollection);
            ServiceLocator.ForSceneOf(this).Register(this._popupManager);
        }
    }
}
