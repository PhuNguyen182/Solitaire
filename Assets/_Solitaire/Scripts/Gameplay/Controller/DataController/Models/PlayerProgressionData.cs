using System;
using DracoRuan.Foundation.DataFlow.LocalData;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Models
{
    [Serializable]
    [GameData(nameof(PlayerProgressionData))]
    public class PlayerProgressionData : IGameData, IDisposable
    {
        public int Version { get; set; }

        public int currentLevel = 1;

        public void Dispose()
        {
            
        }
    }
}
