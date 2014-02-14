using UnityEngine;
using System.Collections;

public class scrLever : MonoBehaviour
{
	public bool Activated;
	private Transform stick;

	// Use this for initialization
	void Start ()
	{
		stick = this.transform.FindChild ("Stick");
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Slide the lever depending on its state.
		if (Activated == true)
			stick.rotation = Quaternion.Lerp (stick.rotation, Quaternion.Euler (-30, 0, 0), 0.1f);
		else
			stick.rotation = Quaternion.Lerp (stick.rotation, Quaternion.Euler (30, 0, 0), 0.1f);

	}

	public void Interact(string name)
	{
		// Levers can only be pulled by Nine.
		if (name == "One_Nine")
			Activated = !Activated;
	}
}
