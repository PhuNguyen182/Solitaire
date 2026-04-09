using System.Collections.Generic;
using DracoRuan.Foundation.DataFlow.LocalData;
using DracoRuan.Foundation.DataFlow.LocalData.StaticDataControllers;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Models;
using DracoRuan.Foundation.DataFlow.DataProcessors;
using DracoRuan.Foundation.DataFlow.DataProviders;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers
{
    [StaticGameDataController(nameof(CardSupplyProbabilityConfigDataController))]
    public class CardSupplyProbabilityConfigDataController : StaticGameDataController<CardSupplyProbabilityConfigData>
    {
        protected override CardSupplyProbabilityConfigData SourceData { get; set; }

        protected override List<DataProcessSequence> DataProcessSequences => new()
        {
            new DataProcessSequence($"SO/ConfigData/{this.GetDataKey()}", DataSourceType.Resources)
        };
        
        protected override void OnDataInitialized()
        {
            
        }
        
        public float GetGenerousProbability() => this.SourceData.generousProbability;
    }
}
