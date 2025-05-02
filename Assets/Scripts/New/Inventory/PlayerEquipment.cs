using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public Transform WeaponSlot;
    public Gun CurrentGun;

    public void EquipGun(Gun gunPrefab)
    {
        if (CurrentGun != null)
            Destroy(CurrentGun.gameObject);

        CurrentGun = Instantiate(gunPrefab, WeaponSlot);
        CurrentGun.transform.localPosition = default;
        CurrentGun.transform.localRotation = default;
    }
}