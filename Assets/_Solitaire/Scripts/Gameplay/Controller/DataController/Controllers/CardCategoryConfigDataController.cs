using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Models;
using DracoRuan.Foundation.DataFlow.LocalData.StaticDataControllers;
using DracoRuan.Foundation.DataFlow.ProcessingSequence;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers
{
    public class CardCategoryConfigDataController : StaticGameDataControllerWithRecord<CardCategoryConfigData,
        CardCategoryRecord, CardCategoryRecordClassMap>
    {
        protected override CardCategoryConfigData SourceData { get; set; }

        protected override List<DataProcessSequence> DataProcessSequences => new()
        {
            new DataProcessSequence(this.GetDataKey(), DataProcessorType.ResourceCsv)
        };

        protected override void OnDataInitialized()
        {

        }
    }
}
