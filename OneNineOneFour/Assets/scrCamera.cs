using UnityEngine;
using System.Collections;

public class scrCamera : MonoBehaviour
{
	public GameObject Target;	// The target object to follow.
	public float Distance = 3;
	private float pitch = 0;
	private float yaw = Mathf.PI;
	public float TranslationSmoothing = 1.0f;	// The lerp to use when translating the camera.
	public float RotationSmoothing = 0.5f;	// The lerp to use when rotating the camera.
	public Vector2 MouseSensitivity = new Vector2(0.5f, 0.5f);	// The sensitivity of the mouse input to move the camera.
	
	//public bool Tracking;	// Whether to track mouse motion (off for menus for example).
	
	void Start ()
	{
	
	}
	
	void Update ()
	{		
		if (Target != null)
		{
			Vector3 direction = Target.transform.position - this.transform.position;
			
			// Cast a ray from the target to the camera and check for the first surface hit by the ray.
			Ray wallClipPreventer = new Ray(Target.transform.position, -direction);
			RaycastHit wallClip;
			float correctDistance;
			if (Physics.Raycast(wallClipPreventer, out wallClip, Distance))
				correctDistance = wallClip.distance - 0.2f; // Set the correct distance of the camera, preventing wall clipping by pushing it closer than its desired distance, plus 0.2 units so it is not inside the wall.
			else
				correctDistance = Distance;	// No clipping is observed so use the desired orbit distance.
			
			//Debug.DrawLine(wallClipPreventer.origin, wallClipPreventer.origin + wallClipPreventer.direction * Distance);
			
			// Smoothly translate the camera to its target position.
			this.transform.position = Vector3.Lerp(this.transform.position, Target.transform.position + new Vector3(Mathf.Sin (yaw), Mathf.Sin (pitch), Mathf.Cos(yaw)) * correctDistance, TranslationSmoothing);
			
			// Smoothly rotate the camera to face its target object.
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.LookRotation(direction), RotationSmoothing);
		}
		else
		{
			
		}
		
		// Change the distance and limit to min and max values.
		Distance -= Input.GetAxis("Mouse ScrollWheel");
		if (Distance > 10)
			Distance = 10;
		else if (Distance < 2)
			Distance = 2;
		
		// Change the pitch and limit to +- Pi / 2 radians.
		pitch += Input.GetAxis("Mouse Y") * MouseSensitivity.y;
		if (pitch > Mathf.PI / 2)
			pitch = Mathf.PI / 2;
		else if (pitch < -Mathf.PI / 2)
			pitch = -Mathf.PI / 2;
		
		// Change the yaw and wrap around 2 Pi radians.
		yaw -= Input.GetAxis("Mouse X") * MouseSensitivity.x;
		if (yaw > 2 * Mathf.PI)
			yaw = 0;
		else if (yaw < 0)
			yaw = 2 * Mathf.PI;
		
	}
}
