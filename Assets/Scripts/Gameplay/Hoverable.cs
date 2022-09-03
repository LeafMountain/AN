using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Hoverable : MonoBehaviour
{
    public Rigidbody rigidbody;

    public float hoverHeight;
    public float recoverForce = 1f;

    public float stability = 0.3f;
    public float speed = 2.0f;

    private void OnValidate()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, hoverHeight))
        {
            var angularVelocity = rigidbody.angularVelocity;
            Vector3 predictedUp = Quaternion.AngleAxis(angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed, angularVelocity) * transform.up;
            Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
            rigidbody.AddTorque(torqueVector * (speed * speed));
            rigidbody.AddRelativeForce(Vector3.up * (recoverForce * (rigidbody.mass * Mathf.Abs(Physics.gravity.y))));
        }
    }
}