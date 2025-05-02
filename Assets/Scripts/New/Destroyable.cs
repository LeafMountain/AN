using UnityEngine;
using UnityEngine.Events;

public class Destroyable : MonoBehaviour {
    public UnityEvent OnDestroy;
    
    public void Trigger() {
        Destroy(gameObject);
        OnDestroy?.Invoke();
    }
}
