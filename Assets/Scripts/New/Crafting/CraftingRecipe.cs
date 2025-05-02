using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [System.Serializable]
    public struct Ingredient
    {
        public string itemId;
        public int amount;
    }

    public string resultItemId;
    public int resultAmount = 1;
    public float craftTime = 0f; // In seconds

    public List<Ingredient> ingredients = new();
}