using System.Collections.Generic;
using CsvHelper.Configuration;
using DracoRuan.Foundation.DataFlow.LocalData;
using DracoRuan.Foundation.DataFlow.ProcessingSequence.CustomDataProcessor.CSVs;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Models
{
    [GameData(nameof(CardCategoryConfigData))]
    public class CardCategoryConfigData : SerializableRecordClass<CardCategoryRecord, CardCategoryRecordClassMap>, IGameData
    {
        public int Version { get; set; }
    }
    
    public class CardCategoryRecord
    {
        public int ID;
        public string Category;
        public int Capacity;
        public bool UseImage;
        public List<string> Words = new();
    }

    public sealed class CardCategoryRecordClassMap : ClassMap<CardCategoryRecord>
    {
        public CardCategoryRecordClassMap()
        {
            this.Map(record => record.ID).Name("ID");
            this.Map(record => record.Category).Name("Category");
            this.Map(record => record.Capacity).Name("Capacity");
            this.Map(record => record.UseImage).Name("Image");
            this.Map(record => record.Words).Convert(args =>
            {
                List<string> list = new();
                for (int i = 1; i <= 20; i++)
                {
                    string word = args.Row.GetField($"Word{i}");
                    if (!string.IsNullOrWhiteSpace(word))
                        list.Add(word);
                }

                return list;
            });
        }
    }
}
