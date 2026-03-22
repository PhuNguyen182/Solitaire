using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace _Solitaire.Scripts.GameInput
{
    public class InputController : MonoBehaviour
    {
        private SolitaireInputPlayer _solitaireInputPlayer;

        private void Awake()
        {
            this._solitaireInputPlayer = new SolitaireInputPlayer();
        }

        private void OnEnable()
        {
            this._solitaireInputPlayer.Enable();
            EnhancedTouchSupport.Enable();
        }
        
        private void OnDisable()
        {
            this._solitaireInputPlayer.Disable();
            EnhancedTouchSupport.Disable();
        }

        private void OnDestroy()
        {
            this._solitaireInputPlayer.Dispose();
        }
    }
}
