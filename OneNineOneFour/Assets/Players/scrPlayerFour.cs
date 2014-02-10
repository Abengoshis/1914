using UnityEngine;
using System.Collections;

public class scrPlayerFour : scrPlayer
{
	private bool piggybacking = false;
	private bool crawling = false;

	protected override void Update()
	{
		base.Update ();

		if (piggybacking == true)
		{
			// Face the same direction as the other sibling.
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, sibling.transform.rotation, 0.1f);

			// If interact or jump are pressed, stop piggybacking.
			if (interactPressed == true || jumpPressed == true)
			{
				// Reapply gravity.
				this.rigidbody.useGravity = true;

				if (jumpPressed == true)
					this.rigidbody.velocity += new Vector3(0, JumpSpeed, 0);

				piggybacking = false;
			}
		}
	}

	protected override void FixedUpdate()
	{
		// Temporarily store the walk and jump speed so they can be restored after modifying.
		float tempWalk = WalkSpeed;
		float tempJump = JumpSpeed;

		// If crawling, reduce the walk speed and disallow jumping.
		if (crawling == true)
		{
			WalkSpeed *= 0.5f;
			JumpSpeed = 0;
		}

		if (piggybacking == true)
		{
			WalkSpeed = 0;

			if (sibling.GetComponent<scrPlayer>().CanControl == true)
			{
				// Lerp this player's position to a position above the sibling.
				this.transform.position = sibling.transform.position + new Vector3(0, sibling.transform.localScale.y + this.transform.localScale.y, 0) + sibling.rigidbody.velocity * Time.fixedDeltaTime;
			}
			else
			{
				// Lerp this player's position to a position above the sibling.
				this.transform.position = Vector3.Lerp (this.transform.position, sibling.transform.position + new Vector3(0, sibling.transform.localScale.y + this.transform.localScale.y, 0), 0.1f);
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
		// Check to see if within interaction range with the sibling.
		if (other.transform.parent == sibling.transform)
		{
			if (onGround == true  && interactPressed == true)
			{
				// Remove velocity and gravity.
				this.rigidbody.useGravity = false;
				this.rigidbody.velocity = Vector3.zero;
				piggybacking = true;
			}
		}
	}
}
