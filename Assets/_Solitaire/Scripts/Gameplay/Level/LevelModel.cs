using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.Controller.DataController.Models;

namespace _Solitaire.Scripts.Gameplay.Level
{
    [Serializable]
    public class LevelModel
    {
        public int moveCount;
        public bool useRandomize;
        public LevelDifficulty difficulty;
        public List<CardColumnModel> cardColumnModel = new();
        public List<CategoryData> availableCategories = new();
        
        public int NumberOfColumns => this.cardColumnModel.Count;

        public void Clear()
        {
            this.moveCount = 0;
            this.useRandomize = false;
            this.difficulty = LevelDifficulty.None;
            this.cardColumnModel.Clear();
            this.availableCategories.Clear();
        }
    }
}
