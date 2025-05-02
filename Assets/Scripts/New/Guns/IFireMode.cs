using UnityEngine;

public interface IFireMode {
    public void Initialize(Gun gun);
    void Fire(Vector3 origin, Vector3 direction);
}