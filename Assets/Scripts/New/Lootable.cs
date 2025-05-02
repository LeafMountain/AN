using UnityEngine;

public class Lootable : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;

    public void Initialize(ItemData itemData, int amount) {
        this.itemData = itemData;
        this.amount = amount;

        var visual = Instantiate(itemData.prefab, transform);
        visual.transform.localPosition = default; 
        visual.transform.localRotation = default; 
    }
}
