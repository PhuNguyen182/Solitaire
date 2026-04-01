using UnityEngine;
using System.Collections.Generic;

namespace Extensions
{
    public static class CollectionExtension
    {
        #region Array
        
        public static void Shuffle<T>(this T[] array)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            for (int i = 0; i < array.Length - 1; i++)
            {
                int randomIndex = Random.Range(i + 1, array.Length);
                (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
            }
        }
        
        public static T GetLast<T>(this List<T> list) => list[^1];

        public static T GetRandom<T>(this List<T> list)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            return list[Random.Range(0, list.Count)];
        }
        
        #endregion

        #region List

        public static void Shuffle<T>(this List<T> list)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            for (int i = 0; i < list.Count - 1; i++)
            {
                int randomIndex = Random.Range(i + 1, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
        
        public static T GetLast<T>(this T[] array) => array[^1];

        public static T GetRandom<T>(this T[] array)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            return array[Random.Range(0, array.Length)];
        }
        
        #endregion
    }
}
