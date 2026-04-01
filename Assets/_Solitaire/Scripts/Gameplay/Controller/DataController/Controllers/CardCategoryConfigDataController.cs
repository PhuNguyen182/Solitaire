using System.Linq;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Models;
using DracoRuan.Foundation.DataFlow.LocalData.StaticDataControllers;
using DracoRuan.Foundation.DataFlow.ProcessingSequence;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers
{
    public class CardCategoryConfigDataController : StaticGameDataControllerWithRecord<CardCategoryConfigData,
        CardCategoryRecord, CardCategoryRecordClassMap>
    {
        private Dictionary<int, CardCategoryRecord> _cardCategoryRecords;
        
        protected override CardCategoryConfigData SourceData { get; set; }

        protected override List<DataProcessSequence> DataProcessSequences => new()
        {
            new DataProcessSequence($"CSVs/{this.GetDataKey()}", DataProcessorType.ResourceCsv)
        };

        protected override void OnDataInitialized()
        {
            this._cardCategoryRecords = this.SourceData.Records.ToDictionary(record => record.ID, record => record);
        }

        public CardCategoryRecord GetCardCategoryConfig(int cardCategoryID) =>
            this._cardCategoryRecords.GetValueOrDefault(cardCategoryID);

        public bool IsWordBelongToCategory(int cardCategoryID, string word)
        {
            CardCategoryRecord cardCategory = this._cardCategoryRecords.GetValueOrDefault(cardCategoryID);
            return cardCategory != null && cardCategory.ContainWord(word);
        }
    }
}
