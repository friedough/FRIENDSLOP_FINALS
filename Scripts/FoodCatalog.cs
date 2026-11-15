using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Food Catalog", menuName = "Game/Food Catalog")]
public class FoodCatalog : ScriptableObject
{
    public List<FoodData> foods = new List<FoodData>();

    public FoodData GetRandomFood()
    {
        if (foods.Count == 0)
            return null;

        return foods[Random.Range(0, foods.Count)];
    }
}