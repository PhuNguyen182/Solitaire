using _Solitaire.Scripts.Gameplay.Controller.DataController.Models;
using DracoRuan.Foundation.DataFlow.DataProviders;
using DracoRuan.Foundation.DataFlow.LocalData;
using DracoRuan.Foundation.DataFlow.LocalData.DynamicDataControllers;
using DracoRuan.Foundation.DataFlow.SaveSystem;
using DracoRuan.Foundation.DataFlow.Serialization;

namespace _Solitaire.Scripts.Gameplay.Controller.DataController.Controllers
{
    [DynamicGameDataController(nameof(PlayerProgressionDataController))]
    public class PlayerProgressionDataController : DynamicGameDataController<PlayerProgressionData>
    {
        private int _cheatingLevel;
        
        protected override PlayerProgressionData SourceData { get; set; }
        protected override IDataSerializer<PlayerProgressionData> DataSerializer { get; set; }
        protected override IDataSaveService DataSaveService { get; set; }
        protected override SerializationType SerializationType => SerializationType.Json;
        protected override DataSourceType DataSourceType => DataSourceType.File;
        
        public bool IsCheatMode { get; set; }
        
        public override void Initialize()
        {
            
        }

        #region Level Progress Accessing
        
        public int GetCurrentPlayLevel()
        {
            int level = this.IsCheatMode ? this._cheatingLevel : this.SourceData.currentLevel;
            return level;
        }

        public void IncreaseLevel()
        {
            this.SourceData.currentLevel += 1;
            this.SaveData();
        }

        public void SetLevelPlay(int playLevel)
        {
            this.SourceData.currentLevel = playLevel;
            this.SaveData();
        }
        
        #endregion

        #region For Cheating
        
        public void SelectCheatingLevel(int level) => this._cheatingLevel = level;

        #endregion
    }
}
