using UnityEngine;
using System.Collections;

public class scrPlayerNine : scrPlayer
{
	public bool Climbing { get; private set; }
	private GameObject ladder;

	protected override void Update()
	{
		// Update status of flags.
		base.Update ();

		if (Climbing)
		{
			// If the player wants to get off the ladder, disable climbing and reenable gravity.
			if (interactPressed || jumpPressed)
				GetOffLadder();
		}
	}

	protected override void FixedUpdate()
	{
		// Temporarily store the walk and jump speed so they can be restored after modifying.
		float tempWalk = WalkSpeed;
		float tempJump = JumpSpeed;

		if (sibling.GetComponent<scrPlayerFour>().Piggybacking == true)
		{
			WalkSpeed *= 0.5f;
			JumpSpeed = 0;
		}
		else if (Climbing == true)
		{
			// Automatically get off ladders if trying to face away from them while also on the ground.
			if (onGround && Vector3.Angle(ladder.transform.forward, this.transform.forward) >= 45)
			{
				GetOffLadder();
			}
			else
			{
				// Lerp the position to centre on the ladder.
				this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(ladder.transform.position.x, this.transform.position.y, ladder.transform.position.z), 0.1f);

				// Always face the ladder.
				this.transform.rotation = ladder.transform.rotation;

				// Vertical input now makes the player move up and down.
				this.rigidbody.velocity = new Vector3(0, Input.GetAxis("Vertical") * WalkSpeed * 0.5f, 0);

				// Do not allow walking or jumping.
				WalkSpeed = 0;
				JumpSpeed = 0;
			}
		}

		// Update physics with the new walk and jump speed.
		base.FixedUpdate();

		// Restore the walk and jump speed for future updates.
		WalkSpeed = tempWalk;
		JumpSpeed = tempJump;
	}

	void OnTriggerStay(Collider other)
	{

		if (other.name == "Ladder")
		{
		// Get the angle to the player along a 2D plane.
		float angToPlayer = Vector3.Angle(other.transform.forward,
		                                  new Vector3(this.transform.position.x, 0, this.transform.position.z) - 
		                                  new Vector3(other.transform.position.x, 0, other.transform.position.z));
	
		// Get the relative angle the player is facing.
		float angFacing = Vector3.Angle (other.transform.forward, this.transform.forward);

			Debug.DrawLine(other.transform.position, other.transform.position + other.transform.forward);
			Debug.DrawLine (other.transform.position, other.transform.position + new Vector3(this.transform.position.x, 0, this.transform.position.z) - 
			                new Vector3(other.transform.position.x, 0, other.transform.position.z));
		
		Debug.Log (angToPlayer + "ANGTOPLAYER");
		Debug.Log (angFacing + "ANGFACING");
		}
		// Get on the ladder if not already climbing, four is not on your back, the other object is a ladder, and you are facing it +- 25.5 degrees.
		if (Climbing == false && sibling.GetComponent<scrPlayerFour>().Piggybacking == false && other.name == "Ladder")
		{
			// Get the angle to the player along a 2D plane.
			float angToPlayer = Vector3.Angle(other.transform.forward,
			                                  new Vector3(this.transform.position.x, 0, this.transform.position.z) - 
			                                  new Vector3(other.transform.position.x, 0, other.transform.position.z));

			// Get the relative angle the player is facing.
			float angFacing = Vector3.Angle (other.transform.forward, this.transform.forward);

			// If the directions are facing the opposite directions, climb the ladder.
			if (angToPlayer > 90 && angFacing < 90 ||
			    angToPlayer < 90 && angFacing > 90)
				GetOnLadder(other.gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (Climbing == true && other.name == "Ladder")
			GetOffLadder();
	}

	private void GetOnLadder(GameObject ladder)
	{
		this.ladder = ladder;
		Climbing = true;
		this.rigidbody.useGravity = false;
	}

	private void GetOffLadder()
	{
		this.ladder = null;
		Climbing = false;
		this.rigidbody.useGravity = true;
	}
}