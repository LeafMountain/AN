public class GameManager : NetworkBehaviour 
{
    private static GameManager gameManager;
    public static GameManager Instance => gameManager ? gameManager : gameManager = FindObjectOfType<GameManager>(true);

    public Camera characterCamera;
    public CinemachineVirtualCamera characterVirtualCamera;
    public GameObject defaultGun;
    public Player localPlayer;
    
    private void Start()
    {
        gameManager = this;
        Events.AddListener(Flag.DamageRecieved, OnDamageRecieved);
        Events.AddListener(Flag.Storeable, OnStoreableUpdated);
    }

    private void OnStoreableUpdated(object origin, EventArgs eventargs)
    {
        var storeableArgs = eventargs as Storeable.StoreableEventArgs;

        if (NetworkManager.Singleton.IsServer)
        {
            switch (storeableArgs.storeable.itemType)
            {
                case Storeable.ItemType.Supply:
                    localPlayer.supplies += 1;
                    break;
                
                case Storeable.ItemType.Battery:
                    localPlayer.energy += 1;
                    break;
                
                case Storeable.ItemType.None:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            storeableArgs.storeable.GetComponent<NetworkObject>().Despawn();
        }
    }

    private void OnDamageRecieved(object origin, EventArgs eventargs)
    {
        var damageArgs = eventargs as DamageRecievedArgs;
        if (damageArgs.destroyed == false)
        {
            if (damageArgs.instigator == localPlayer)
            {
                characterVirtualCamera.Shake(.2f, 4f);
            }
        }
        if (damageArgs.destroyed)
        {
            characterVirtualCamera.Shake(.7f, 7f);
        }
    }

    public static GameObject Spawn(GameObject original, Transform parent = null)
    {
        return Instantiate(original, parent);
    }
    
    public static GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return Instantiate(original, position, rotation, parent);
    }
    
    public static T Spawn<T>(T original) where T : MonoBehaviour
    {
        return Instantiate(original);
    }
    
    public static T Spawn<T>(T original, Vector3 position, Quaternion rotation, Transform parent = null) where T : MonoBehaviour
    {
        return Instantiate(original, position, rotation, parent);
    }
}