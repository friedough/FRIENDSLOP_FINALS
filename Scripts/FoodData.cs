using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Game/Food Item")]
public class FoodData : ScriptableObject
{
    [Header("Food Information")]
    public string foodName;
    public GameObject foodPrefab;

    [TextArea]
    public string description;
}