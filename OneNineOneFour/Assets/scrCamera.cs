using UnityEngine;
using System.Collections;

public class scrCamera : MonoBehaviour
{
	public GameObject Target;	// The target object to follow.
	private bool changingTarget = false;
	public float Distance = 3;
	private float pitch = 0;
	private float yaw = Mathf.PI;
	public float TranslationSmoothing = 1.0f;	// The lerp to use when translating the camera.
	public float RotationSmoothing = 0.5f;	// The lerp to use when rotating the camera.
	public Vector2 MouseSensitivity = new Vector2(0.5f, 0.5f);	// The sensitivity of the mouse input to move the camera.
	
	//public bool Tracking;	// Whether to track mouse motion (off for menus for example).

	public void ChangeTarget(GameObject target)
	{
		Target = target;
		changingTarget = true;
	}

	void Start ()
	{
		if (Target != null)
		{
			this.transform.position = Target.transform.position + new Vector3(Mathf.Sin (yaw), Mathf.Sin (pitch), Mathf.Cos(yaw)) * Distance;
			this.transform.LookAt(Target.transform.position);
		}
	}
	
	void FixedUpdate ()
	{		
		if (Target != null)
		{
			Vector3 direction = Target.transform.position - this.transform.position;
			
			// Cast a ray from the target to the camera and check for the first surface hit by the ray.
			Ray wallClipPreventer = new Ray(Target.transform.position, -direction);
			RaycastHit hit;
			float correctDistance;
			if (Physics.Raycast(wallClipPreventer, out hit, Distance, 1 << LayerMask.NameToLayer("Surface")))
				correctDistance = hit.distance - 0.2f; // Set the correct distance of the camera, preventing wall clipping by pushing it closer than its desired distance, plus 0.2 units so it is not inside the wall.
			else
				correctDistance = Distance;	// No clipping is observed so use the desired orbit distance.

			// Find the target position in orbit around the target.
			Vector3 desiredPosition = Target.transform.position + new Vector3(Mathf.Sin (yaw), Mathf.Sin (pitch), Mathf.Cos(yaw)) * correctDistance;

			// Smoothly translate the camera to its target position.
			this.transform.position = Vector3.Lerp(this.transform.position, desiredPosition, TranslationSmoothing);

			// Only allow rotation when not travelling to a new target.
			if (changingTarget == true)
		    {
				if (Vector3.Distance(this.transform.position, desiredPosition) < 0.1f)
					changingTarget = false;
			}
			else
			{
				// Change the distance and limit to min and max values. Get from both mouse and gamepad????.
				Distance -= Input.GetAxis("Mouse ScrollWheel");
				if (Distance > 10)
					Distance = 10;
				else if (Distance < 2)
					Distance = 2;

				// Get input from both mouse and gamepad.
				if (Input.GetAxis("Move Camera") != 0)
				{
					// Change the pitch and limit to +- Pi / 2 radians.
					pitch += Input.GetAxis("Mouse Y") * MouseSensitivity.y;
					
					// Change the yaw and wrap around 2 Pi radians.
					yaw += Input.GetAxis("Mouse X") * MouseSensitivity.x;
				}
				else
				{
					// Change the pitch and limit to +- Pi / 2 radians.
					pitch += Input.GetAxis("Pitch Camera") * MouseSensitivity.y * 0.2f;
					
					// Change the yaw and wrap around 2 Pi radians.
					yaw += Input.GetAxis("Yaw Camera") * MouseSensitivity.x * 0.3f;
				}

				if (pitch > Mathf.PI / 2)
					pitch = Mathf.PI / 2;
				else if (pitch < -Mathf.PI / 2)
					pitch = -Mathf.PI / 2;

				if (yaw > 2 * Mathf.PI)
					yaw = 0;
				else if (yaw < 0)
					yaw = 2 * Mathf.PI;

				// Smoothly rotate the camera to face its target object.
				this.transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.LookRotation(direction), RotationSmoothing);
			}
		}
	}
}
