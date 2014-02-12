using UnityEngine;
using System.Collections;

public class scrPortal : MonoBehaviour
{
	public bool Open;

	// Use this for initialization
	void Start ()
	{
		if (Open == false)
		{
			renderer.material = new Material(renderer.material);
			renderer.material.SetColor("_TintColor", Color.red * 0.1f);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{

	}
}
