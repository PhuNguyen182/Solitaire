using System.Collections.Generic;
using CsvHelper.Configuration;
using DracoRuan.Foundation.DataFlow.LocalData;
using DracoRuan.Foundation.DataFlow.LocalData.StaticDataControllers.CSVs;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Models
{
    [GameData(nameof(CardCategoryConfigData))]
    public class CardCategoryConfigData : CustomRecordData<CardCategoryRecord>, IGameData
    {
        public int Version { get; set; }
    }
    
    public class CardCategoryRecord
    {
        public int ID;
        public string Category;
        public bool UseImage;
        public List<string> Words = new();
        
        public int Capacity => this.Words.Count;
        public bool ContainWord(string word) => this.Words.Contains(word);
    }

    public sealed class CardCategoryRecordClassMap : ClassMap<CardCategoryRecord>
    {
        private const string IdFieldName = "ID";
        private const string CategoryFieldName = "Category";
        private const string UseImageFieldName = "Image";
        private const string WordsFieldName = "Word";
        
        public CardCategoryRecordClassMap()
        {
            this.Map(record => record.ID).Name(IdFieldName);
            this.Map(record => record.Category).Name(CategoryFieldName);
            this.Map(record => record.UseImage).Name(UseImageFieldName);
            this.Map(record => record.Words).Convert(args =>
            {
                List<string> list = new();
                for (int i = 1; i <= 20; i++)
                {
                    string word = args.Row.GetField($"{WordsFieldName}{i}");
                    if (string.IsNullOrEmpty(word) || string.IsNullOrWhiteSpace(word))
                        continue;
                    
                    list.Add(word);
                }

                return list;
            });
        }
    }
}
