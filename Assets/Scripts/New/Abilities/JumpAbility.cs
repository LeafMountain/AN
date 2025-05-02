using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class JumpAbility : MonoBehaviour, IAbility
{
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;
    private bool requestJump;

    public bool IsActive { get; private set; }
    
    public static event Action OnJump;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Activate()
    {
        // Queue a jump (useful for input buffering)
        if (controller.isGrounded)
            requestJump = true;

        IsActive = true;
        OnJump?.Invoke();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Tick()
    {
        if (!IsActive) return;

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -1f; // Stick to ground

        if (requestJump)
        {
            verticalVelocity = jumpForce;
            requestJump = false;
        }

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
}