using DG.Tweening;
using UnityEngine;

public class Shake : MonoBehaviour {
    public float duration = .1f;
    public Vector3 power = new(1, 1, 1);
    
    public void Trigger() {
        DOTween.Kill(transform, true);
        transform.DOPunchRotation(power, duration);
    }
}
