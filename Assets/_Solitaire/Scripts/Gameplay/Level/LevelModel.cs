using System;
using System.Collections.Generic;

namespace _Solitaire.Scripts.Gameplay.Level
{
    [Serializable]
    public class LevelModel
    {
        public int moveCount;
        public bool useDynamicCardData;
        public List<CardColumnModel> cardColumnModel = new();
        public List<string> availableCategories = new();
        
        public int NumberOfColumns => this.cardColumnModel.Count;

        public void Clear()
        {
            this.moveCount = 0;
            this.useDynamicCardData = false;
            this.cardColumnModel.Clear();
            this.availableCategories.Clear();
        }
    }
}
