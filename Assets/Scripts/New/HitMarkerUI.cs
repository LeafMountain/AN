using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class HitMarkerUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset hitMarkerTemplate;
    [SerializeField] private VisualTreeAsset killMarkerTemplate;

    private VisualElement root;

    private void Awake()
    {
        root = uiDocument.rootVisualElement;
        CombatEvents.OnHitConfirmed += ShowHitMarker;
        CombatEvents.OnTargetKilled += ShowKillMarker;
    }

    private void OnDestroy()
    {
        CombatEvents.OnHitConfirmed -= ShowHitMarker;
        CombatEvents.OnTargetKilled -= ShowKillMarker;
    }

    private void ShowHitMarker(Vector3 screenPos)
    {
        ShowMarker(hitMarkerTemplate);
    }

    private void ShowKillMarker(Vector3 screenPos)
    {
        ShowMarker(killMarkerTemplate);
    }

    private void ShowMarker(VisualTreeAsset template)
    {
        var marker = template.Instantiate();
        marker.AddToClassList("hit-marker");
        root.Add(marker);

        // Fade and remove after time
        marker.experimental.animation.Start(new StyleValues { opacity = 1f }, new StyleValues { opacity = 0f }, 300)
            .OnCompleted(() => root.Remove(marker));
    }
}