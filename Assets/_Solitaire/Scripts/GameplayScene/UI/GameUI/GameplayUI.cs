using TMPro;
using UnityEngine;

namespace _Solitaire.Scripts.GameplayScene.UI.GameUI
{
    public class GameplayUI : MonoBehaviour
    {
        private const string InfinitySign = "\u221E";
        
        [SerializeField] private TMP_Text moveCountText;

        public void SetMoveCount(int moveCount)
        {
            string moveCountString = moveCount >= 0 ? $"{moveCount}" : InfinitySign;
            this.moveCountText.text = moveCountString;
        }
    }
}
