using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public IInventory inventory;

    public Queue<CraftingRecipe> craftingQueue = new();
    public bool IsCrafting => currentTask != null;

    private Coroutine currentTask;

    public event System.Action<CraftingRecipe> OnCraftingStarted;
    public event System.Action<CraftingRecipe> OnCraftingCompleted;
    public event System.Action<CraftingRecipe> OnCraftingCanceled;

    public bool EnqueueRecipe(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe))
            return false;

        // Reserve ingredients up front
        foreach (var ing in recipe.ingredients)
            inventory.RemoveItem(ing.itemId, ing.amount);

        craftingQueue.Enqueue(recipe);

        if (currentTask == null)
            currentTask = StartCoroutine(CraftingLoop());

        return true;
    }

    public void CancelCurrentCraft()
    {
        if (currentTask != null)
        {
            StopCoroutine(currentTask);
            currentTask = null;

            if (craftingQueue.TryDequeue(out var canceled))
            {
                // Refund ingredients
                foreach (var ing in canceled.ingredients)
                    inventory.AddItem(ing.itemId, ing.amount);

                OnCraftingCanceled?.Invoke(canceled);
            }

            // Start next if any
            if (craftingQueue.Count > 0)
                currentTask = StartCoroutine(CraftingLoop());
        }
    }

    private IEnumerator CraftingLoop()
    {
        while (craftingQueue.Count > 0)
        {
            var recipe = craftingQueue.Dequeue();
            OnCraftingStarted?.Invoke(recipe);

            yield return new WaitForSeconds(recipe.craftTime);

            inventory.AddItem(recipe.resultItemId, recipe.resultAmount);
            OnCraftingCompleted?.Invoke(recipe);
        }

        currentTask = null;
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
        foreach (var ing in recipe.ingredients)
        {
            if (inventory.GetItemCount(ing.itemId) < ing.amount)
                return false;
        }
        return true;
    }
}
