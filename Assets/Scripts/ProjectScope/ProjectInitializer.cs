using Cysharp.Threading.Tasks;
using DracoRuan.Foundation.DataFlow.MasterDataController;
using ServiceLocators.Core;
using UnityEngine;

namespace ProjectScope
{
    public class ProjectInitializer : MonoBehaviour
    {
        private MainDataManager _mainDataManager;

        private void Awake()
        {
            this.RegisterServices().Forget();
        }

        private async UniTask RegisterServices()
        {
            await this.InitializeDataManager();
        }

        private async UniTask InitializeDataManager()
        {
            this._mainDataManager = new MainDataManager();
            await this._mainDataManager.InitializeDataHandlers();
            ServiceLocator.Global.Register(this._mainDataManager);
        }
    }
}
