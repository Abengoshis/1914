using UnityEngine;
using System.Collections;

public class scrPlayer : MonoBehaviour
{
	private static bool interacting = false;	// Prevents constant interacting.
	private static bool switching = false;	// Prevents constant switching between characters.

	public float WalkSpeed;	// The speed to apply when walking.
	public float JumpSpeed;	// The speed to apply when jump is pressed.
	private bool jumping = false;	// Prevents constant jumping.
	
	public bool CanControl = false;	// Whether this specific player can be controlled.
	public bool Waiting = false;	// Whether this specific player has been signalled to wait.
	protected GameObject sibling;	// The other sibling.

	protected bool interactPressed { get; private set; }
	protected bool jumpPressed { get; private set; }
	protected bool switchPressed { get; private set; }
	protected bool onGround { get; private set; }

	// Use this for initialization
	void Start ()
	{
		Application.targetFrameRate = 60;

		// Disable the renderer when not in the editor.
		renderer.enabled = false;

		// Depending on this sibling's name, find the other sibling.
		if (this.name == "One_Nine")
			sibling = GameObject.Find ("One_Four");
		else if (this.name == "One_Four")
			sibling = GameObject.Find ("One_Nine");

		Debug.Log (sibling.name);
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		if (CanControl == true)
		{
			HandleInteract();
			HandleSwitch();
		}
	}

	protected virtual void FixedUpdate()
	{
		// Stop the rigidbody from sleeping.
		this.rigidbody.WakeUp();

		// Reset the velocity.
		this.rigidbody.velocity = new Vector3(0, this.rigidbody.velocity.y, 0);

		// Create a ray pointing down.
		Ray floorCheck = new Ray(this.transform.position, Vector3.down);
		
		// Check if the player is on (slightly above for more lenient input) the floor.
		if (Physics.Raycast(floorCheck, this.transform.localScale.y + 0.1f, 1 << LayerMask.NameToLayer("Default")))
			onGround = true;
		else
			onGround = false;

		if (CanControl)
		{
			HandleJump();
			HandleMove();
		}
		else
		{
			HandleFollow();
		}
	}

	void HandleInteract()
	{
		interactPressed = false;
		if (Input.GetAxis ("Interact") != 0)
		{
			// This prevents constant interacting while the Interact axis is down.
			if (interacting == false)
			{
				interacting = true;
				interactPressed = true;
			}
		}
		else
		{
			// Interact is no longer held so allow it to be activated again on the next press.
			interacting = false;
		}
	}

	void HandleSwitch()
	{
		switchPressed = false;
		if (Input.GetAxis ("Switch") != 0)
		{
			// This prevents constant switching between players while the Switch axis is down.
			if (switching == false)
			{
				// Stop manual control.
				CanControl = false;

				// Pass manual control to the other sibling.
				sibling.GetComponent<scrPlayer>().CanControl = true;
				
				// Target the camera to the other sibling.
				Camera.main.GetComponent<scrCamera>().ChangeTarget(sibling);

				switching = true;
				switchPressed = true;
			}
		}
		else
		{
			// Switch is no longer held so allow it to be activated again on the next press.
			switching = false;
		}
	}

	void HandleJump ()
	{
		jumpPressed = false;
		if (Input.GetAxis ("Jump") != 0)
		{
			if (jumping == false)
			{
				// Only jump if on the ground.
				if (onGround == true)
				{
					this.rigidbody.velocity += new Vector3(0, JumpSpeed, 0);
					jumping = true;
				}

				jumpPressed = true;
			}
		}
		else
		{
			// Jumping is no longer held so allow it to be activated again on the next press.
			jumping = false;
		}
	}

	void HandleMove ()
	{
		Vector3 moveDirection = Vector3.zero;

		// Forwards/Backwards
		if (Input.GetAxis("Vertical") != 0)
		{
			// Get the camera's forward vector.
			Vector3 forward = Camera.main.transform.forward;
			forward.y = 0;	// Remove the vertical value to only use horizontal components.
		
			// Add this direction to the move direction, with its sign depending on the sign of the vertical input.
			moveDirection += forward * Input.GetAxis("Vertical");
		}

		// Strafing
		if (Input.GetAxis("Horizontal") != 0)
		{
			// Get the camera's right vector.
			Vector3 right = Camera.main.transform.right;
			right.y = 0;	// Remove the vertical value to only use horizontal components.
			
			// Add this direction to the move direction, with its sign depending on the sign of the vertical input.
			moveDirection += right * Input.GetAxis("Horizontal");
		}

		// Face the desired direction.
		this.transform.LookAt(this.transform.position + moveDirection);

		// Set the horizontal components of the velocity to the move direction's horizontal components.
		this.rigidbody.velocity += new Vector3(moveDirection.x * WalkSpeed, 0, moveDirection.z * WalkSpeed);
	}

	void HandleFollow ()
	{
		// If this player isn't waiting, follow the other player.
		if (Waiting == false)
		{

		}
	}
}
