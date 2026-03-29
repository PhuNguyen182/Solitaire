using System;
using System.Collections.Generic;
using _Solitaire.Scripts.Gameplay.GameEntity.VisualCard;

namespace _Solitaire.Scripts.Gameplay.Level
{
    [Serializable]
    public class CardColumnModel
    {
        public List<CardModel> cardModel = new();
    }
}