using System;
using System.Threading.Tasks;
using DG.Tweening;
using EventManager;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Actor
{
    public float aggroRange = 10f;
    public float speed = 2f;
    public float turnSpeed = 5f;
    public float stoppingRange = 2f;

    public Vector3 destination;
    public bool destinationReached;
    public DamageReciever damageReciever;
    public Gun gun;
    public Transform gunAttachPoint;

    public float aggroTimer;
    public Renderer stateLight;

    public Color idleColor;
    public Color aggroColor;
    public Color attackColor;

    protected override void Start()
    {
        base.Start();
        Events.AddListener(Flag.DamageRecieved, this, OnDamaged);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        SetDestination(transform.position);
        gun = Instantiate(GameManager.Instance.enemyGun, gunAttachPoint);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;
        NetworkServer.Spawn(gun.gameObject); 
    }

    [Server]
    private void FixedUpdate()
    {
        if (GameManager.Instance.localPlayer == false) return;
        Vector3 playerPosition = GameManager.Instance.localPlayer.transform.position + Vector3.up;
        float playerDistance = Vector3.Distance(playerPosition, transform.position);

        if (playerDistance < stoppingRange)
        {
            OnWithinStoppingRange();
        }
        else if (aggroTimer > 0 || playerDistance < aggroRange)
        {
            OnPlayerWithinRange();
        }
        else
        {
            OnIdle();
        }

        // Move
        destinationReached = (Vector3.Distance(transform.position, destination) < stoppingRange);
        if (destinationReached == false)
        {
            Vector3 direction = (destination - transform.position).normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.fixedDeltaTime * turnSpeed);
            transform.Translate(transform.forward * speed * Time.fixedDeltaTime);
        }
    }

    private void OnWithinStoppingRange()
    {
        // Charge up
        // Attack
        SetStateColor(attackColor);
        gun.Fire();
    }

    private void SetStateColor(Color color)
    {
        if(stateLight == null) return;
        for (var i = 0; i < stateLight.sharedMaterials.Length; i++)
        {
            stateLight.sharedMaterials[i].SetColor("_Emission", color);
        }
    }

    protected virtual void OnIdle()
    {
        if (destinationReached)
        {
            SetStateColor(idleColor);
            Vector3 insideUnitSphere = Random.insideUnitSphere;
            insideUnitSphere *= Random.Range(2f, 4f);
            insideUnitSphere += transform.position;
            insideUnitSphere.y = 1f;
            SetDestination(insideUnitSphere);
        }
    }

    public async void StartAggroTimer()
    {
        aggroTimer = 5f;
        while (aggroTimer > 0)
        {
            await Task.Yield();
            aggroTimer -= Time.deltaTime;
        }
    }

    protected virtual void OnPlayerWithinRange()
    {
        if (aggroTimer <= 0f)
            StartAggroTimer();
        else
            aggroTimer = 5f;

        SetDestination(GameManager.Instance.localPlayer.transform.position + Vector3.up);
        SetStateColor(aggroColor);
    }

    public void SetDestination(Vector3 destination)
    {
        destinationReached = false;
        this.destination = destination;
    }

    private void OnDamaged(object origin, EventArgs eventargs)
    {
        if (damageReciever.currentHealth > 0)
        {
            transform.DOPunchScale(Vector3.one * .1f, .2f);
        }
        else
        {
            transform.DOScale(Vector3.one * 2f, .1f).onComplete += () => Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}