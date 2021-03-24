using System;
using UnityEngine;
using InControl;


namespace ManArmy
{
	/// <summary>
	/// Handles the player input, and feeds it to the other components.
	/// </summary>
	public class PlayerInputHandler : PlayerBehavior
	{        
		private void Update()
		{
			if(Player.ViewLocked.Is(false))
			{
                var InputDevice = InputManager.ActiveDevice;

                // Movement.
                Vector2 moveInput = new Vector2(
                    InputDevice.LeftStickX.Value,
                    InputDevice.LeftStickY.Value
                );

				Player.MovementInput.Set(moveInput);

				// Look.
				Player.LookInput.Set(new Vector2(
                    InputDevice.RightStickX.Value,
                    InputDevice.RightStickY.Value
                ));

                // Jump.
                if (InputDevice.Action4.WasPressed)
                    Player.Jump.TryStart();

                // Attack.
                if (InputDevice.RightBumper.WasPressed)
                    Player.Attack.TryStart();

                // Roll.
                if (InputDevice.Action1.WasPressed)
                    Player.Roll.TryStart();

                // Lock on.
                if (InputDevice.RightStickButton.WasPressed)
                {
                    if (!Player.LockOn.Active)
                        Player.LockOn.TryStart();
                    else
                        Player.LockOn.ForceStop();
                }
            }
			else
			{
				// Movement.
				Player.MovementInput.Set(Vector2.zero);

				// Look.
				Player.LookInput.Set(Vector2.zero);
			}
		}
	}
}
