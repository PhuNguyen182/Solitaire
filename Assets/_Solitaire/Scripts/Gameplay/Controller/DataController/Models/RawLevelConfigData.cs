using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using DracoRuan.Foundation.DataFlow.LocalData;
using DracoRuan.Foundation.DataFlow.LocalData.StaticDataControllers.CSVs;
using CsvHelper.Configuration;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Models
{
    [GameData(nameof(RawLevelConfigData))]
    public class RawLevelConfigData : CustomRecordData<RawLevelData>, IGameData
    {
        public int Version { get; set; }
    }

    [Serializable]
    public enum LevelDifficulty
    {
        None = 0,
        Easy1 = 1,
        Easy2 = 2,
        Medium1 = 3,
        Medium2 = 4,
        Hard1 = 5,
        Hard2 = 6,
    }

    public class RawLevelData
    {
        public int Level;
        public LevelDifficulty Difficulty;
        public bool UseRandomize;
        public int WordColumns;
        public int Moves;
        public int CategoryIndex;
        public CardContentType ItemType;
        public string CategoryName;
        public int WordCount;
        
        public List<int> ColumnCounts = new();
        public List<string> Words = new();
    }

    public sealed class RawLevelDataClassMap : ClassMap<RawLevelData>
    {
        private int _lastLevel;
        private int _lastWordColumn;
        private LevelDifficulty _lastDifficulty;
        private bool _lastUseRandomize;
        private int _lastWordColumns;
        private int _lastMoves;
        private List<int> _lastColumnCounts = new();

        public RawLevelDataClassMap()
        {
            this.Map(rawLevelData => rawLevelData.Level).Convert(args =>
            {
                string level = args.Row.GetField("Level");
                if (int.TryParse(level, out int result)) 
                    this._lastLevel = result;
                return this._lastLevel;
            });

            this.Map(rawLevelData => rawLevelData.Difficulty).Convert(args =>
            {
                string difficulty = args.Row.GetField("Difficulty");
                if (!string.IsNullOrWhiteSpace(difficulty)) 
                    Enum.TryParse(difficulty, out this._lastDifficulty);
                return this._lastDifficulty;
            });

            this.Map(rawLevelData => rawLevelData.UseRandomize).Convert(args =>
            {
                string useRandomize = args.Row.GetField("UseRandomize");
                if (!string.IsNullOrWhiteSpace(useRandomize)) 
                    this._lastUseRandomize = string.CompareOrdinal(useRandomize.ToUpper(), "TRUE") == 0;
                return this._lastUseRandomize;
            });
            
            this.Map(rawLevelData => rawLevelData.ColumnCounts).Convert(args =>
            {
                string columnCount = args.Row.GetField("Column1Count");
                if (!string.IsNullOrWhiteSpace(columnCount))
                {
                    this._lastColumnCounts = new List<int>();
                    for (int i = 4; i <= 8; i++) 
                        this._lastColumnCounts.Add(args.Row.GetField<int>(i));
                }

                return this._lastColumnCounts;
            });

            this.Map(rawLevelData => rawLevelData.Moves).Convert(args =>
            {
                string move = args.Row.GetField("Moves");
                if (int.TryParse(move, out int result)) 
                    this._lastMoves = result;
                return this._lastMoves;
            });
            
            this.Map(rawLevelData => rawLevelData.WordColumns).Convert(args =>
            {
                string wordColumns = args.Row.GetField("WordColumns");
                if (int.TryParse(wordColumns, out int result)) 
                    this._lastWordColumn = result;
                return this._lastWordColumn;
            });
            
            this.Map(rawLevelData => rawLevelData.WordCount).Name("WordCount");
            this.Map(rawLevelData => rawLevelData.CategoryIndex).Name("CategoryIndex");
            this.Map(rawLevelData => rawLevelData.ItemType).Name("ItemType");
            this.Map(rawLevelData => rawLevelData.CategoryName).Name("CategoryName");
            this.Map(rawLevelData => rawLevelData.Words).Convert(args =>
            {
                List<string> words = new List<string>();
                int count = args.Row.GetField<int>("WordCount");
                for (int i = 0; i < count; i++)
                {
                    words.Add(args.Row.GetField(14 + i));
                }

                return words;
            });
        }
    }
    
    public class LevelConfigData
    {
        public int Level;
        public LevelDifficulty Difficulty;
        public bool UseRandomize;
        public int WordColumns;
        public int Moves;
        
        public List<int> ColumnCounts = new();
        public List<CategoryConfigData> Categories = new();
    }
    
    public class CategoryConfigData
    {
        public int CategoryIndex;
        public CardContentType ItemType;
        public string CategoryName;
        public List<string> Words = new();
        
        public int WordCount => this.Words.Count;
    }
}
