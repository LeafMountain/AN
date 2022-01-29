using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DG.Tweening;
using EventManager;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Actor
{
    public float aggroRange = 10f;
    public float speed = 2f;

    public Vector3 destination;
    public bool destinationReached;
    public DamageReciever damageReciever;

    public float aggroTimer;

    protected override void Start()
    {
        base.Start();
        SetDestination(transform.position);
        Events.AddListener(Flag.DamageRecieved, this, OnDamaged);
    }
    
    private void FixedUpdate()
    {
        if(GameManager.Instance.localPlayer == false) return;
        Vector3 playerPosition = GameManager.Instance.localPlayer.transform.position + Vector3.up;
        float playerDistance = Vector3.Distance(playerPosition, transform.position);
        if (aggroTimer > 0 || playerDistance < aggroRange)
        {
            OnPlayerWithinRange();
        }
        else
        {
            OnIdle();
        }

        destinationReached = (Vector3.Distance(transform.position, destination) < .1f);
        if(destinationReached == false)
        {
            transform.Translate((destination - transform.position).normalized * speed * Time.fixedDeltaTime);  
        }
    }

    protected virtual void OnIdle()
    {
        if (destinationReached)
        {
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
        if(aggroTimer <= 0f)
            StartAggroTimer();
        else
            aggroTimer = 5f;
        
        SetDestination(GameManager.Instance.localPlayer.transform.position + Vector3.up);
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
