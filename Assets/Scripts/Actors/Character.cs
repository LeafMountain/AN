using Core;
using UnityEngine;

public class Character : Actor
{
    public DamageReceiver damageReceiver;
    public Equipment equipment;
    public Animator animator;

    public SkinnedMeshRenderer leg;

    private Vector3 positionLastFrame;
    public Vector3 Velocity { get; private set; }

    public SkinnedMeshRenderer testPart;

    public UIActor ui;

    protected virtual void OnValidate()
    {
        equipment = GetComponent<Equipment>();
    }

    protected override void Start()
    {
        base.Start();

        if (animator)
        {
            animator.Rebind();
        }

        if (ui)
        {
            Vector3 position = GetCenterOfMass() + Vector3.up * 1.5f;
            Quaternion rotation = transform.rotation;
            ui = GameManager.Spawn(ui, position, rotation, transform);
            ui.Init(this);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected virtual void LateUpdate()
    {
        UpdateVelocity();
        UpdateAnimatorValues();

    }

    protected virtual void FixedUpdate()
    {
    }



    private void UpdateVelocity()
    {
        Vector3 position = transform.position;
        Velocity = position - positionLastFrame;
        positionLastFrame = position;
    }

    private void UpdateAnimatorValues()
    {
        if (animator == null) return;

        Vector3 localVelocity = transform.InverseTransformDirection(Velocity) / Time.deltaTime;
        animator.SetFloat("HorizontalSpeed", localVelocity.x);
        animator.SetFloat("Speed", localVelocity.z);
    }


}