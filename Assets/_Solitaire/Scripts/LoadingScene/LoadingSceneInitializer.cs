using ProjectScope;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace _Solitaire.Scripts.LoadingScene
{
    public class LoadingSceneInitializer : MonoBehaviour
    {
        private void Awake()
        {
            this.WaitAndLoadNextScene().Forget();
        }

        private async UniTask WaitAndLoadNextScene()
        {
            await SceneManager.LoadSceneAsync(SceneName.Home);
        }
    }
}
