using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Models;

namespace _Solitaire.Scripts.Gameplay.Level
{
    [Serializable]
    public class LevelModel
    {
        public int moveCount;
        public LevelDifficulty difficulty;
        public List<CardColumnModel> cardColumnModel = new();
        public List<CategoryData> availableCategories = new();
        public int numberOfColumns;

        public void Clear()
        {
            this.moveCount = 0;
            this.difficulty = LevelDifficulty.None;
            this.numberOfColumns = 0;
            this.cardColumnModel.Clear();
            this.availableCategories.Clear();
        }
    }
}
