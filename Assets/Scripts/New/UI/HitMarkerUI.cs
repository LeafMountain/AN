using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HitMarkerUI : MonoBehaviour
{
    // [SerializeField] private UIDocument uiDocument;
    // [SerializeField] private VisualTreeAsset hitMarkerTemplate;
    // [SerializeField] private VisualTreeAsset killMarkerTemplate;

    // private VisualElement root;
    
    [SerializeField] private Image hitMarker;
    [SerializeField] private Image killMarker;

    private void Awake()
    {
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
        ShowMarker(hitMarker);
    }

    private void ShowKillMarker(Vector3 screenPos)
    {
        ShowMarker(killMarker);
    }

    private void ShowMarker(Image template)
    {
        // var marker = template.Instantiate();
        // marker.AddToClassList("hit-marker");
        // root.Add(marker);

        // Fade and remove after time
        // marker.experimental.animation.Start(new StyleValues { opacity = 1f }, new StyleValues { opacity = 0f }, 300)
            // .OnCompleted(() => root.Remove(marker));
            
        template.gameObject.SetActive(true);
        hitMarker.DOKill();
        hitMarker.DOFade(0, 0.3f).From(1f).OnComplete(() =>
        {
            template.gameObject.SetActive(false);
            template.color = new Color(template.color.r, template.color.g, template.color.b, 1f); // Reset alpha
        });
    }
}