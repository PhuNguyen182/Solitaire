using Cysharp.Threading.Tasks;
using DracoRuan.Foundation.UISystem.Canvases;
using DracoRuan.Foundation.UISystem.Popups.PopupManager;
using ServiceLocators.Core;
using UnityEngine;

namespace _Solitaire.Scripts.HomeScene
{
    public class HomeSceneInitializer : MonoBehaviour
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
            if (this.uiCanvasManager is IUICanvasManager canvasManager)
                ServiceLocator.ForSceneOf(this).Register(canvasManager);
            
            this._popupManager = new UIPopupManager(this.popupCollection);
            ServiceLocator.ForSceneOf(this).Register(this._popupManager);
        }
    }
}
