using System.Linq;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Models;
using DracoRuan.Foundation.DataFlow.LocalData.StaticDataControllers;
using DracoRuan.Foundation.DataFlow.DataProcessors;
using DracoRuan.Foundation.DataFlow.DataProviders;
using DracoRuan.Foundation.DataFlow.LocalData;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers
{
    [StaticGameDataController(nameof(CardCategoryConfigDataController))]
    public class CardCategoryConfigDataController : StaticGameDataControllerWithRecord<CardCategoryConfigData,
        CardCategoryRecord, CardCategoryRecordClassMap>
    {
        private Dictionary<int, CardCategoryRecord> _cardCategoryRecords;
        private Dictionary<string, int> _keyMappings;
        
        protected override CardCategoryConfigData SourceData { get; set; }

        protected override List<DataProcessSequence> DataProcessSequences => new()
        {
            new DataProcessSequence($"CSVs/{this.GetDataKey()}", DataSourceType.Resources)
        };

        protected override void OnDataInitialized()
        {
            this._cardCategoryRecords = this.SourceData.Records.ToDictionary(record => record.ID);
            this._keyMappings = this.SourceData.Records.ToDictionary(record => record.Category, record => record.ID);
            
            /*var duplicates = this.SourceData.Records
                .GroupBy(r => r.Category) // Nhóm theo tên Category
                .Where(g => g.Count() > 1) // Chỉ lấy những nhóm có nhiều hơn 1 phần tử
                .Select(g => new { 
                    CategoryName = g.Key, 
                    Count = g.Count() 
                });

            Debug.Log("Các giá trị bị lặp:");
            string s = "";
            foreach (var item in duplicates)
            {
                s += $"{item.CategoryName},";
                Debug.Log($"- {item.CategoryName}: xuất hiện {item.Count} lần");
            }
            Debug.Log(s);*/
        }

        public CardCategoryRecord GetCardCategoryConfig(int cardCategoryID) =>
            this._cardCategoryRecords.GetValueOrDefault(cardCategoryID);

        public bool IsWordBelongToCategory(int cardCategoryID, string word)
        {
            CardCategoryRecord cardCategory = this._cardCategoryRecords.GetValueOrDefault(cardCategoryID);
            return cardCategory != null && cardCategory.ContainWord(word);
        }
        
        public int GetCardCategoryID(string categoryName) => this._keyMappings.GetValueOrDefault(categoryName);

        protected override void ReleaseManagedResources()
        {
            this._cardCategoryRecords?.Clear();
            this._cardCategoryRecords = null;
        }
    }
}
