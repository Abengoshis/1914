using UnityEngine;
using System.Collections;

public class scrPlayer : MonoBehaviour
{
	private static bool switching = false;	// Prevents constant switching between characters.
	
	public bool CanControl = false;	// Whether this specific player can be controlled.
	private GameObject sibling;	// The other sibling.

	// Use this for initialization
	void Start ()
	{
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
	void Update ()
	{
		if (CanControl == true)
		{

			if (Input.GetAxis ("Switch") > 0)
			{
				// This prevents constant switching between players while the Switch axis is down.
				if (switching == false)
				{
					CanControl = false;	// Stop manual control.
					sibling.GetComponent<scrPlayer>().CanControl = true;	// Pass manual control to the other sibling.
					Camera.main.GetComponent<scrCamera>().Target = sibling;	// Target the camera to the other sibling.
					switching = true;	// Switching is taking place.

					Debug.Log ("Switched to " + sibling.name);
				}
			}
			else
			{
				switching = false;	// Switching is no longer happening.
			}




		}
		else
		{

		}
	}
}
