using UnityEngine;

public class PlayerDebug : MonoBehaviour {
    public Gun initialGun;
    
    private PlayerEquipment playerEquipment;
    
    void Start() {
        playerEquipment = GetComponent<PlayerEquipment>();
        playerEquipment.EquipGun(initialGun);
    }
}
