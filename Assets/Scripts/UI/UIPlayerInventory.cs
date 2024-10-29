using InventorySystem;
using StatefulUI.Runtime.Core;

namespace UI {
    public class UIPlayerInventory {
        public StatefulComponent view;

        public void Init(StatefulComponent view) {
            this.view = view;

            Player localPlayer = GameManager.Players.Get(0);
            GameManager.ItemManager.AddCallback(localPlayer.GetComponent<PlayerInventory>().InventoryHandle, OnPlayerInventoryUpdated);
            FullyUpdatePlayerInventory();
            ToggleInventory(false);
        }

        public void ToggleInventory() {
            // if (view.TryGetContainer((int)ContainerRole.PlayerInventory, out ContainerReference playerInventoryContainer)) {
            //     playerInventoryContainer.Container.gameObject.SetActive(!playerInventoryContainer.Container.gameObject.activeSelf);
            // }
        }

        public void ToggleInventory(bool enabled) {
            // if (view.TryGetContainer((int)ContainerRole.PlayerInventory, out ContainerReference playerInventoryContainer)) {
            //     playerInventoryContainer.Container.gameObject.SetActive(enabled);
            // }
        }

        void OnPlayerInventoryUpdated(object sender, InventoryEventArgs e) {
            FullyUpdatePlayerInventory();
        }

        void FullyUpdatePlayerInventory() {
            // if (view.TryGetContainer((int)ContainerRole.PlayerInventory, out var playerInventoryContainer)) {
            //     InventoryHandle playerInventoryHandle = GameManager.Players.Get(0).GetComponent<PlayerInventory>().InventoryHandle;
            //     List<ItemHandle> items = GameManager.ItemManager.GetItems(playerInventoryHandle);
            //     playerInventoryContainer.Container.FillWithItems(items, (childView, handle) => {
            //         childView.transform.localPosition = default;
            //         childView.transform.localRotation = default;
            //         ObjectReference attachPoint = childView.GetObject(ObjectRole.GameObjectAttach);
            //
            //         int childCount = attachPoint.Object.transform.childCount;
            //         for (int i = 0; i < childCount; i++) {
            //             GameManager.Spawner.Despawn(attachPoint.Object.transform.GetChild(i).gameObject);
            //         }
            //
            //         Item? item = GameManager.ItemManager.GetItem(handle);
            //         if (item.HasValue) {
            //             ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
            //             itemData.graphics.InstantiateAsync(attachPoint.Object.transform.position, attachPoint.Object.transform.rotation, attachPoint.Object.transform).Completed += x => {
            //                 x.Result.transform.localPosition = default;
            //                 x.Result.transform.localRotation = default;
            //                 x.Result.transform.localScale = Vector3.one * 100;
            //                 x.Result.layer = LayerMask.NameToLayer("UI");
            //                 // (childView.transform as RectTransform).rect.height
            //             };
            //         }
            //     });
            // }
        }
    }
}