using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Serialization;

public class LootNotificationUI : MonoBehaviour {
    // [SerializeField] private UIDocument uiDocument;
    // private VisualElement lootContainer;

    [SerializeField] private LootEntryUI lootEntryUIPrefab;

    private Dictionary<string, LootEntry> activeLootEntries = new();

    private void OnEnable() {
        PlayerInventory.OnItemAdded += ShowLoot;
    }

    private void OnDisable() {
        PlayerInventory.OnItemAdded -= ShowLoot;
    }

    public void ShowLoot(string itemName, int amount) {
        if (activeLootEntries.TryGetValue(itemName, out var entry)) {
            entry.Amount += amount;
            // entry.Label.text = $"+{entry.Amount} {itemName}";
            entry.LootEntryUI.Label.SetText($"+{entry.Amount} {itemName}");
            entry.ResetTimer();
        }
        else {
            // var label = new Label($"+{amount} {itemName}");
            // label.AddToClassList("loot-entry");
            // lootContainer.Add(label);
            var lootEntryUI = Instantiate(lootEntryUIPrefab, transform);
            lootEntryUI.Label.SetText($"+{amount} {itemName}");

            var lootEntry = new LootEntry {
                LootEntryUI = lootEntryUI,
                Amount = amount
            };
            activeLootEntries[itemName] = lootEntry;

            lootEntry.StartFadeOut(this, () => {
                Destroy(lootEntry.LootEntryUI.gameObject);
                activeLootEntries.Remove(itemName);
            });
        }
    }

    private class LootEntry {
        // public Label Label;
        public LootEntryUI LootEntryUI;
        public int Amount;
        private Coroutine fadeRoutine;
        private float duration = 3f;
        private float endTime;

        public void StartFadeOut(MonoBehaviour owner, System.Action onDone) {
            LootEntryUI.gameObject.SetActive(true);
            endTime = Time.time + duration;
            fadeRoutine = owner.StartCoroutine(FadeOutCoroutine(onDone));
        }

        public void ResetTimer() {
            if (fadeRoutine != null) {
                // Label.style.animationName = "none"; // reset animation
                // Label.style.animationName = "fade-out"; // retrigger
            }

            endTime = Time.time + duration;
        }

        private IEnumerator FadeOutCoroutine(System.Action onDone) {
            // yield return new WaitForSeconds(duration);
            while (endTime > Time.time) {
                yield return null;
            }

            onDone?.Invoke();
        }
    }
}