using Newtonsoft.Json.Bson;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		public PlayerInput playerInput;

		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool attack;
        public bool attack2;
        public bool changeModeUp;
		public bool changeModeDown;
		public bool pause;
		public bool unPause;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnPause(InputValue value)
		{
			PauseInput(value.isPressed);
		}
		public void OnUnPause(InputValue value)
		{
			UnPauseInput(value.isPressed);
		}
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
		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}
		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnAttack(InputValue value)
		{
			AttackInput(value.isPressed);
		}
        public void OnAttack2(InputValue value)
        {
            Attack2Input(value.isPressed);
        }
        public void OnChangeModeUp(InputValue value)
		{
            ChangeModeUpInput(value.isPressed);
		}
        public void OnChangeModeDown(InputValue value)
        {
            ChangeModeDownInput(value.isPressed);
        }
#endif


		public void PauseInput(bool newPauseState)
		{
			pause = newPauseState;
		}
		public void UnPauseInput(bool newUnPauseState)
		{
			unPause = newUnPauseState;
		}
        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 
		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}
		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}
		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
        public void AttackInput(bool newAttackState)
        {
            attack = newAttackState;
        }
        public void Attack2Input(bool newAttackState)
        {
            attack2 = newAttackState;
        }
        public void ChangeModeUpInput(bool newAscendState)
		{
            changeModeUp = newAscendState;
		}
		public void ChangeModeDownInput(bool newDescendState)
		{
			changeModeDown = newDescendState;
		}
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		public void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
}