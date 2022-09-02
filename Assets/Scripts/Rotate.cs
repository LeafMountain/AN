using UnityEngine;

[ExecuteInEditMode]
public class Rotate : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speed = 90f;
    
    void Update()
    {
        transform.RotateAround(transform.position, axis, speed * Time.deltaTime);        
    }
}
