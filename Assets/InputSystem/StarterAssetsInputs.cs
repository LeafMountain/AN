using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool fire;
		public bool fireAlt;
		public bool interact;
		public bool drop;
		
		public bool one;
		public bool two;
		public bool three;
		public bool four;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value) => JumpInput(value.isPressed);
		public void OnSprint(InputValue value) => SprintInput(value.isPressed);
		public void OnFire(InputValue value) => FireInput(value.isPressed);
		public void OnFireAlt(InputValue value) => FireAltInput(value.isPressed);
		public void OnInteract(InputValue value) => InteractInput(value.isPressed);
		public void OnDrop(InputValue value) => DropInput(value.isPressed);
		public void OnOne(InputValue value) => OneInput(value.isPressed);
		public void OnTwo(InputValue value) => TwoInput(value.isPressed);
		public void OnThree(InputValue value) => ThreeInput(value.isPressed);
		public void OnFour(InputValue value) => fourInput(value.isPressed);

		// ------
		
		public void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;
		public void LookInput(Vector2 newLookDirection) => look = newLookDirection;
		public void JumpInput(bool newJumpState) => jump = newJumpState;
		public void SprintInput(bool newSprintState) => sprint = newSprintState;
		public void FireInput(bool newFireState) => fire = newFireState;
		public void FireAltInput(bool newFireState) => fireAlt = newFireState;
		public void InteractInput(bool newInputState) => interact = newInputState;
		public void DropInput(bool newInputState) => drop = newInputState;
		public void OneInput(bool newInputState) => one= newInputState;
		public void TwoInput(bool newInputState) => two= newInputState;
		public void ThreeInput(bool newInputState) => three= newInputState;
		public void fourInput(bool newInputState) => four= newInputState;

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			GameManager.LockCursor(cursorLocked);
		}
#endif

	}
	
}