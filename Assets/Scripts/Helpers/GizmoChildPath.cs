using UnityEngine;

public class GizmoChildPath : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++) {
            var a = transform.GetChild(i);
            var b = transform.GetChild((i + 1) % transform.childCount);
            Gizmos.DrawLine(a.position, b.position);
        }
    }
}
