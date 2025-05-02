using UnityEngine;

public class PlayerInputHandlerUI : MonoBehaviour
{
    public InventoryUI inventoryUI;
    
    private void Awake() {
        inventoryUI = FindFirstObjectByType<InventoryUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            inventoryUI.Toggle();
    }
}