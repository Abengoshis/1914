using UnityEngine;
using System.Collections;

public class scrCamera : MonoBehaviour
{
	public GameObject Target;	// The target object to follow.
	private float translateLerp = 0.1f;	// The speed of the camera's lerp.
	private float rotateLerp = 0.2f;
	
	private float orbitPitch = 0;
	private float orbitYaw = 0;
	private float orbitDistance = 3;
	
	void Start ()
	{
	
	}
	
	void Update ()
	{		
		if (Target != null)
		{
			Vector3 direction = Target.transform.position - this.transform.position;
			
			// Cast a ray from the target to the camera and check for the first surface hit by the ray.
			Ray wallClipPreventer = new Ray(Target.transform.position, -direction); Debug.DrawRay(wallClipPreventer.origin, wallClipPreventer.direction);
			RaycastHit wallClip;
			Physics.Raycast(wallClipPreventer, out wallClip, orbitDistance);
			float correctDistance = wallClip.distance; // Set the correct distance of the camera, preventing wall clipping by potentially pushing it closer than its desired distance.
			
			this.transform.position = Vector3.Lerp(this.transform.position, Target.transform.position + new Vector3(Mathf.Sin (orbitYaw), Mathf.Sin (orbitPitch), Mathf.Cos(orbitYaw)) * correctDistance, translateLerp);
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.LookRotation(direction), rotateLerp);
		}
		else
		{
			
		}
		
	}
}
