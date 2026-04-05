using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;

namespace _Solitaire.Scripts.Gameplay.Level
{
    [Serializable]
    public class CategoryData
    {
        public int categoryId;
        public int maxCardCount;
        public List<CardModel> cards = new(); 
    }
}