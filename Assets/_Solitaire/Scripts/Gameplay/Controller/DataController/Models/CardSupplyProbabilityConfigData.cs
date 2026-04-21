using System;
using DracoRuan.Foundation.DataFlow.LocalData;
using UnityEngine;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Models
{
    [Serializable]
    [GameData(nameof(CardSupplyProbabilityConfigData))]
    [CreateAssetMenu(fileName = "CardSupplyProbabilityConfigData",
        menuName = "SO/Solitaire/ConfigData/CardSupplyProbabilityConfigData")]
    public class CardSupplyProbabilityConfigData : ScriptableObject, IGameData
    {
        public int Version { get; set; }

        [Range(0f, 1f)] public float generousProbability = 0.5f;
    }
}
