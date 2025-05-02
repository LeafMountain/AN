using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset inventoryItemTemplate;
    private VisualElement healthFill;
    private VisualElement inventoryGrid;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Health Bar
        healthFill = root.Q<VisualElement>("health-fill");

        // Inventory
        inventoryGrid = root.Q<VisualElement>("inventory-grid");
        // PopulateInventory();
    }

    public void UpdateHealth(float healthPercentage)
    {
        healthFill.style.width = new Length(healthPercentage * 100, LengthUnit.Percent);
    }

    private void PopulateInventory()
    {
        for (int i = 0; i < 16; i++) // Example: 16 slots
        {
            var item = inventoryItemTemplate.CloneTree();
            item.RegisterCallback<PointerDownEvent>(OnItemDragStart);
            item.RegisterCallback<PointerUpEvent>(OnItemDragEnd);
            inventoryGrid.Add(item);
        }
    }

    private void OnItemDragStart(PointerDownEvent evt)
    {
        var item = evt.target as VisualElement;
        item.style.position = Position.Absolute;
    }

    private void OnItemDragEnd(PointerUpEvent evt)
    {
        var item = evt.target as VisualElement;
        item.style.position = Position.Relative;
    }
}