using System.Collections;
using DG.Tweening;
using Mirror;
using UnityEngine;

public class Player : Actor
{
    [SerializeField] private DamageReciever damageReciever;

    public Gun gun;
    public Transform hand;

    public int supplies = 0;
    public int energy = 100;
    public float lootRange = 2f;
    public float lootSpeed = 5f;

    private Collider[] colliders = new Collider[10];
    public LayerMask actorMask;
    private Tweener lootTween;

    public override void OnStartServer()
    {
        base.OnStartServer();
        gun = Instantiate(GameManager.Instance.defaultGun, hand);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;
        NetworkServer.Spawn(gun.gameObject);
        StartCoroutine(EnergyTick());
    }

    public IEnumerator EnergyTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            energy -= 1;
        }
    }

    public void FixedUpdate()
    {
        if (isServer)
        {
            int hits = Physics.OverlapSphereNonAlloc(transform.position, lootRange, colliders, actorMask);
            for (int i = 0; i < hits; i++)
            {
                var actor = colliders[i].GetComponent<Actor>();
                var storeable = actor.GetComponent<Storeable>();
                float distance = Vector3.Distance(transform.position + Vector3.up, actor.transform.position);
                if (distance < 1f)
                {
                    storeable.PickUp(this);
                    NetworkServer.Destroy(storeable.gameObject);
                    Destroy(storeable.gameObject);
                }
                else
                {
                    actor.GetComponent<Rigidbody>().isKinematic = true;
                    storeable.transform.position += (transform.position + Vector3.up - actor.transform.position).normalized * lootSpeed * Time.fixedDeltaTime;
                }
            }
        }
    }
}