using ProjectScope;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace _Solitaire.Scripts.BootScene
{
    public class BootSceneInitializer : MonoBehaviour
    {
        private ProjectInitializer _projectInitializer;
        
        private void Awake()
        {
            this._projectInitializer = FindFirstObjectByType<ProjectInitializer>();
            this.WaitAndLoadNextScene().Forget();
        }

        private async UniTask WaitAndLoadNextScene()
        {
            await UniTask.WaitUntil(AllServiceRegistered);
            await SceneManager.LoadSceneAsync(SceneName.Loading);
            return;

            bool AllServiceRegistered() => this._projectInitializer.AllServiceRegistered;
        }
    }
}
