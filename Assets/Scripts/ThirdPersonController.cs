using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif


/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : Player
    {
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private PlayerInput _playerInput;
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        protected override void Start()
        {
            base.Start();
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _playerInput = GetComponent<PlayerInput>();

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        protected override void Update()
        {
            base.Update();

            if (IsOwner)
            {
                _hasAnimator = TryGetComponent(out _animator);

                // InputFix();

                JumpAndGravity();
                GroundedCheck();
                Move();
                Fire();
                // Aim();
                FireAlt();
                Interact();
                ChangeWeapon();
            }

            CharacterRotation();
        }

        private void ChangeWeapon()
        {
            if (_input.one)
            {
                equipment.SetSlot(0);
                _input.one = false;
            }
            else if (_input.two)
            {
                equipment.SetSlot(1);
                _input.two = false;
            }
            else if (_input.three)
            {
                equipment.SetSlot(2);
                _input.three = false;
            }
            else if (_input.four)
            {
                equipment.SetSlot(3);
                _input.four = false;
            }
        }


        protected override void LateUpdate()
        {
            base.LateUpdate();

            CameraRotation();
            GunRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void CharacterRotation()
        {
            if (MouseHit.transform)
            {
                Vector3 lookPosition = MouseHit.point;
                lookPosition.y = transform.position.y;
                transform.rotation = Quaternion.LookRotation(lookPosition - transform.position);
            }
        }

        private void GunRotation()
        {
            if (equipment.HasWeapon() == false) return;

            Vector3 lookAtPoint = Vector3.zero;

            if (MouseHit.transform == null)
            {
                lookAtPoint = CameraController.Instance.camera.transform.forward * 50f;
            }
            // if (MouseHit.transform.TryGetComponent(out DamageReciever damageReciever))
            // {
            // weapon.transform.LookAt(MouseHit.transform);
            // }

            else
            {
                lookAtPoint = MouseHit.point;
            }

            equipment.AimAt(lookAtPoint);
        }

        private void InputFix()
        {
            _input.sprint = Keyboard.current.shiftKey.isPressed;

            float x = 0, y = 0;
            x += Keyboard.current.dKey.isPressed ? 1 : 0;
            x += Keyboard.current.aKey.isPressed ? -1 : 0;
            y += Keyboard.current.wKey.isPressed ? 1 : 0;
            y += Keyboard.current.sKey.isPressed ? -1 : 0;
            _input.move = new Vector2(x, y);
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                Vector3 moveAnimationVelocity = new(_input.move.x, 0f, _input.move.y);
                moveAnimationVelocity = transform.InverseTransformDirection(moveAnimationVelocity);
                moveAnimationVelocity *= targetSpeed;

                // _animator.SetFloat("HorizontalSpeed", moveAnimationVelocity.x);
                // _animator.SetFloat("Speed", moveAnimationVelocity.z);

                // _animator.SetFloat(_animIDSpeed, _animationBlend);
                // _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private bool isFiring;

        public float _amplitude = 20;
        public float _time = 1f;

        private bool aimMode;
        private float aimModeCooldown;

        private void ToggleAim(bool value)
        {
            AimModeCooldown();
        }

        private async void AimModeCooldown()
        {
            aimModeCooldown = Time.time + 2f;

            if (aimMode) return;
            aimMode = true;
            while (true)
            {
                await Task.Yield();
                if (aimModeCooldown < Time.time)
                    break;
            }

            aimMode = false;
        }

        [ServerRpc]
        private void ServerFire_ServerRpc() => Fire();

        private void Fire()
        {
            if (Mouse.current.leftButton.isPressed == false) return;

            if (IsServer == false)
            {
                ServerFire_ServerRpc();
                return;
            }

            AimModeCooldown();
            equipment.UseWeapon();
        }

        private void FireAlt()
        {
            if (Mouse.current.rightButton.isPressed == false)
            {
                equipment.StopAim();
                return;
            }

            AimModeCooldown();
            equipment.Aim(MouseHit.point);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (aimMode)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, equipment.weaponAttach.transform.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, equipment.weaponAttach.transform.rotation);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, equipment.weaponAttach.transform.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand,
                    equipment.weaponAttach.transform.rotation * Quaternion.Euler(Vector3.up * 90));

                animator.SetLookAtWeight(1f);
                animator.SetLookAtPosition(equipment.weaponAttach.transform.position + Vector3.up * .5f);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            }
        }

        private void Interact()
        {
            if (_input.interact == false) return;
            GetComponent<Interactor>().Interact();


            // if (lookTarget.TryGetComponent(out IInteractable interactable))
            // {
            //     Debug.Log(interactable.GetPrompt());
            //     interactable.Interact(this);
            // }

            _input.interact = false;
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }
    }
}