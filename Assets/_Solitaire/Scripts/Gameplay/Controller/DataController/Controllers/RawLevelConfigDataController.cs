using System.Linq;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Level;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Models;
using DracoRuan.Foundation.DataFlow.LocalData.StaticDataControllers;
using DracoRuan.Foundation.DataFlow.DataProcessors;
using DracoRuan.Foundation.DataFlow.DataProviders;
using DracoRuan.Foundation.DataFlow.LocalData;
using UnityEngine.Pool;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers
{
    [StaticGameDataController(nameof(RawLevelConfigDataController))]
    public class RawLevelConfigDataController : StaticGameDataControllerWithRecord<RawLevelConfigData, RawLevelData,
        RawLevelDataClassMap>
    {
        private Dictionary<int, LevelConfigData> _levelConfigData;

        protected override RawLevelConfigData SourceData { get; set; }

        protected override List<DataProcessSequence> DataProcessSequences => new()
        {
            new DataProcessSequence($"CSVs/{this.GetDataKey()}", DataSourceType.Resources)
        };

        protected override void OnDataInitialized()
        {
            this.SetupLevelConfigData();
        }

        private void SetupLevelConfigData()
        {
            using (ListPool<LevelConfigData>.Get(out List<LevelConfigData> levelConfigDataList))
            {
                levelConfigDataList = this.SourceData.Records
                    .GroupBy(rawLevelData => rawLevelData.Level)
                    .Select(grouping => new LevelConfigData
                    {
                        Level = grouping.Key,
                        Difficulty = grouping.First().Difficulty,
                        UseRandomize = grouping.First().UseRandomize,
                        ColumnCounts = grouping.First().ColumnCounts,
                        Moves = grouping.First().Moves,
                        WordColumns = grouping.First().WordColumns,
                        Categories = grouping.Select(rawLevelData => new CategoryConfigData
                        {
                            CategoryIndex = rawLevelData.CategoryIndex,
                            ItemType = rawLevelData.ItemType,
                            CategoryName = rawLevelData.CategoryName,
                            Words = rawLevelData.Words
                        }).ToList()
                    }).ToList();

                this._levelConfigData = levelConfigDataList.ToDictionary(rawLevelData => rawLevelData.Level);
            }
        }

        public LevelModel BuildLevelModel(int level)
        {
            LevelConfigData levelConfigData = this.GetLevelConfigData(level);
            if (levelConfigData == null)
                return null;

            // To do: modify the level model data structure to update generate new level model
            return new LevelModel();
        }

        private LevelConfigData GetLevelConfigData(int levelId)
        {
            return this._levelConfigData?.GetValueOrDefault(levelId);
        }

        protected override void ReleaseManagedResources()
        {
            this._levelConfigData?.Clear();
            this._levelConfigData = null;
        }
    }
}
