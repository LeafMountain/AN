using System.Collections.Generic;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class InventoryUI : MonoBehaviour {
    private IInventory inventory;
    [SerializeField] private InventoryEntryUI inventoryEntryUIPrefab;

    private bool isVisible;
    private List<InventoryEntryUI> inventoryEntries = new();

    private void Awake() {
        inventory = FindFirstObjectByType<PlayerInventory>();
        gameObject.SetActive(false);
    }

    private void OnEnable() {
        PlayerInventory.OnItemAdded += PlayerInventoryOnOnItemAdded;
    }

    private void OnDisable() {
        PlayerInventory.OnItemAdded -= PlayerInventoryOnOnItemAdded;
    }

    private void PlayerInventoryOnOnItemAdded(string itemId, int quantity) {
        Refresh();
    }

    public void Toggle() {
        isVisible = !isVisible;
        gameObject.SetActive(isVisible);
        if (isVisible) Refresh();

        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isVisible;
    }

    private void Refresh() {
        for (var i = inventoryEntries.Count - 1; i >= 0; i--) {
            Destroy(inventoryEntries[i].gameObject);
            inventoryEntries.RemoveAt(i);
        }

        foreach (var item in inventory.GetInventory()) {
            var inventoryEntryUI = Instantiate(inventoryEntryUIPrefab, transform);
            inventoryEntries.Add(inventoryEntryUI);
            inventoryEntryUI.Label.SetText($"{item.itemID} x{item.quantity}");
            inventoryEntryUI.DropButton.onClick.RemoveAllListeners();
            inventoryEntryUI.DropButton.onClick.AddListener(() => {
                inventory.RemoveItem(item.itemID, 1); // Or however you drop
                Refresh();
            });
            inventoryEntryUI.gameObject.SetActive(true);
        }
    }
}