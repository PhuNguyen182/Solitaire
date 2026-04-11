using ZLinq;
using System.Linq;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Level;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Models;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;
using DracoRuan.Foundation.DataFlow.LocalData.StaticDataControllers;
using DracoRuan.Foundation.DataFlow.DataProcessors;
using DracoRuan.Foundation.DataFlow.DataProviders;
using DracoRuan.Foundation.DataFlow.LocalData;
using DracoRuan.Foundation.DataFlow.MasterDataController;
using Extensions;
using UnityEngine.Pool;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers
{
    [StaticGameDataController(nameof(RawLevelConfigDataController))]
    public class RawLevelConfigDataController : StaticGameDataControllerWithRecord<RawLevelConfigData, RawLevelData,
        RawLevelDataClassMap>
    {
        private Dictionary<int, LevelConfigData> _levelConfigData;
        private CardCategoryConfigDataController _cardCategoryConfigDataController;
        private IMainDataManager _mainDataManager;

        protected override RawLevelConfigData SourceData { get; set; }

        protected override List<DataProcessSequence> DataProcessSequences => new()
        {
            new DataProcessSequence($"CSVs/{this.GetDataKey()}", DataSourceType.Resources)
        };

        public override void InjectDataManager(IMainDataManager mainDataManager)
        {
            base.InjectDataManager(mainDataManager);
            this._mainDataManager = mainDataManager;
        }

        protected override void OnDataInitialized()
        {
            this.SetupLevelConfigData();
        }

        private void SetupLevelConfigData()
        {
            using (ListPool<LevelConfigData>.Get(out List<LevelConfigData> levelConfigDataList))
            {
                levelConfigDataList = this.SourceData.Records.AsValueEnumerable()
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

        #region Creating Level Model

        public LevelModel BuildLevelModel(int level)
        {
            LevelConfigData levelConfigData = this.GetLevelConfigData(level);
            if (levelConfigData == null)
                return null;

            this._cardCategoryConfigDataController ??=
                this._mainDataManager.GetStaticDataController<CardCategoryConfigDataController>();
            LevelModel levelModel = new LevelModel
            {
                moveCount = levelConfigData.Moves,
                numberOfColumns = levelConfigData.WordColumns
            };

            List<CategoryData> availableCategories = BuildAvailableCardCategories(levelConfigData);
            levelModel.availableCategories = availableCategories;
            List<CardColumnModel> cardColumnModel = BuildCardColumnData(levelConfigData, availableCategories);
            levelModel.cardColumnModel = cardColumnModel;
            return levelModel;
        }

        private List<CategoryData> BuildAvailableCardCategories(LevelConfigData levelConfigData)
        {
            List<CategoryData> levelCategoryData = new List<CategoryData>();
            List<CategoryConfigData> cardCategory = levelConfigData.Categories;
            foreach (CategoryConfigData cardCategoryData in cardCategory)
            {
                int wordCount = cardCategoryData.Words.Count;
                CardContentType cardContentType = cardCategoryData.ItemType;
                List<CardModel> cardModels = new List<CardModel>
                {
                    new()
                    {
                        cardCategory = cardCategoryData.CategoryName,
                        cardContent = cardCategoryData.CategoryName,
                        contentType = cardContentType,
                        cardType = CardType.Foundation,
                    }
                };

                foreach (string cardWord in cardCategoryData.Words)
                {
                    cardModels.Add(new CardModel
                    {
                        cardCategory = cardCategoryData.CategoryName,
                        cardContent = cardWord,
                        contentType = cardContentType,
                        cardType = CardType.Normal,
                    });
                }

                levelCategoryData.Add(new CategoryData
                {
                    categoryName = cardCategoryData.CategoryName,
                    maxCardCount = wordCount,
                    cards = cardModels,
                });
            }

            return levelCategoryData;
        }

        private List<CardColumnModel> BuildCardColumnData(LevelConfigData levelConfigData,
            List<CategoryData> availableCategories)
        {
            List<CardModel> totalCardModels = new List<CardModel>();
            List<CardColumnModel> result = new List<CardColumnModel>();
            List<CardModel> cardInColumnModels = new List<CardModel>();

            int categoryCount = availableCategories.Count;
            for (int i = 0; i < categoryCount; i++)
                totalCardModels.AddRange(availableCategories[i].cards);

            List<int> columnCounts = levelConfigData.ColumnCounts;
            int columnCount = columnCounts.Count;

            int currentIndex = 0;
            bool useRandom = levelConfigData.UseRandomize;
            totalCardModels.Shuffle(!useRandom);

            for (int i = 0; i < columnCount; i++)
            {
                cardInColumnModels.Clear();
                int cardCountInColumn = columnCounts[i];
                if (currentIndex + cardCountInColumn <= totalCardModels.Count)
                {
                    List<CardModel> subList = totalCardModels.GetRange(currentIndex, cardCountInColumn);
                    cardInColumnModels.AddRange(subList);
                    currentIndex += cardCountInColumn;
                }

                result.Add(new CardColumnModel
                {
                    cardModel = cardInColumnModels
                });
            }

            return result;
        }
        
        #endregion

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
