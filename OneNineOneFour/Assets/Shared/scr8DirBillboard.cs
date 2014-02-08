using UnityEngine;
using System.Collections;

/// <summary>
/// This script causes its object to face the main camera at all times, but
/// also takes into account the angle of the camera to the object. This allows
/// eight different frame sets to be used for each 45 degree angle, giving a
/// stylistic false 3D appearance.
/// 
/// Frames are stored in sets next to each other, extending down 8 frames (1 for each angle).
/// e.g. if there were 4 angles instead of 8 (for demonstration purposes).
/// ####@@@@@@&
/// ####@@@@@@&
/// ####@@@@@@&
///	####@@@@@@&
///	0   1     2 : FrameSet
///	4   6     1 : FramesPerSet
///	
///	Algorithm for current actual frame:		
///	x = (FramesPerSet[FrameSet - 1] + frame) * FrameWidth;
///	y = angle * FrameHeight;
/// </summary>
public class scr8DirBillboard : MonoBehaviour
{
	public Material SpriteSheet;	// The sprite sheet material to be used.
	public float Scale = 1;	// The size of the sprite quad in units.
	private GameObject sprite;	// The sprite which will be seen by the camera.
	private int frame = 0;	// The current frame of animation.
	private int angle = 0;	// The current angle frameset to use.
	private float frameTimer = 0;	// The frame timer that counts to the delay.
	public float FrameDelay = 0;	// The delay between animation frames in milliseconds.
	public int FrameWidth = 0;	// The width of each frame.
	public int FrameHeight = 0;	// The height of each frame.
	public int FrameSet = 0;	// The current set of frames and angles to use (8 frames down, n frames across).
	public int[] FramesPerSet;	// The number of frames in each set (of 8 angles).
	public bool Animated = true;	// Whether the sprite is animated.
	
	// Properties used to find the actual frame offset for the material.
	private float sheetWidth { get { return SpriteSheet.mainTexture.width; } }
	private float sheetHeight { get { return SpriteSheet.mainTexture.height; } }
	private float xOffset { get { return FrameSet > 0 ? (float)((FramesPerSet[FrameSet - 1] + frame) * FrameWidth) / sheetWidth : 0; } }	// Divide by sheet width.
	private float yOffset { get { return 1 - (float)((angle + 1) * FrameHeight) / sheetHeight; } }	// Divide by sheet height.
	
	// Use this for initialization
	private void Start ()
	{
		sprite = GameObject.CreatePrimitive(PrimitiveType.Quad);
		Destroy (sprite.collider);	// Remove the quad's collider.
		sprite.renderer.material = new Material(SpriteSheet);
		sprite.transform.localScale = new Vector3(Scale, Scale * (float)FrameHeight / FrameWidth, 1);	// Set the scale to Scale by Scale * FrameHeight / FrameWidth ratio so the texture isn't stretched.
		sprite.layer = LayerMask.NameToLayer("Ignore Raycast");
	}
	
	// Update is called once per frame
	private void Update ()
	{
		// If the sprite is animated, run animation logic.
		if (Animated == true)
		{
			frameTimer += Time.deltaTime;	// Increment the frameTimer by the time between updates.
			if (frameTimer >= FrameDelay)
			{
				frameTimer %= FrameDelay;	// Loop the frameTimer when it reaches the delay.	
				frame++;	// Increment the animation frame.
				if (frame > FramesPerSet[FrameSet])
					frame %= FramesPerSet[FrameSet];	// Loop the frame when it reaches the end of its set.
			}
		}
		
		// Find the direction from the camera to this object along a 2D plane.
		Vector3 direction = this.transform.position - Camera.main.transform.position;
		direction.y = 0;	// Ignore elevation.
				
		// Get the angle of the direction to two vectors which define the orientation of this object.
		float angleToForward = Vector3.Angle (direction, this.transform.forward);	// 0 = facing away (direction & forward are equal), 180 = facing towards (direction & forward are opposite).
		float angleToRight = Vector3.Angle (direction, this.transform.right);	// 0 = facing left (direction & right are equal), 180 = facing right (direction & right are opposite).
		
		// Set the frameset to the corresponding angle. With 8 angles, each frameset occurs at 45 degree increments with, so check if the angle is within +- 22.5 degrees of the extremes 0 and 180.
		if (angleToForward <= 22.5f)
		{
			angle = 4;	// Facing away.
		}
		else if (angleToForward <= 67.5f)
		{
			// Check right angle.	
			if (angleToRight > 90.0f)
				angle = 5;	// Facing away and to the right.
			else
				angle = 3;	// Facing away and to the left.
		}
		else if (angleToForward <= 112.5f)
		{
			if (angleToRight > 90)
				angle = 6;	// Facing right.
			else
				angle = 2;	// Facing left.			
		}
		else if (angleToForward <= 157.5f)
		{
			if (angleToRight > 90.0f)
				angle = 7;	// Facing towards and to the right.
			else
				angle = 1;	// Facing towards and to the left.
		}
		else // if (angleToForward <= 180)
		{
			angle = 0;	// Facing towards
		}
		
		//Debug.Log ("Forward: " + angleToForward);
		//Debug.Log ("Right  : " + angleToRight);
		//Debug.Log ("Angle  : " + angle);
		Debug.DrawRay(transform.position, transform.forward);
		
		// Move the sprite to this object's location.
		sprite.transform.position = this.transform.position;
		
		// Rotate the sprite to always face the main camera.
		sprite.transform.LookAt(Camera.main.transform.position);
		Quaternion lookRotation = sprite.transform.rotation;	// Get the rotation to a modifiable quaternion.
		lookRotation.eulerAngles = new Vector3(0, sprite.transform.rotation.eulerAngles.y + 180, 0);	// Remove pitch and yaw rotation components and rotate the yaw by 180 degrees to face the right way.
		sprite.transform.rotation = lookRotation;	// Set the rotation back, with the pitch and yaw nullified.
		
		// Set the material properties.
		sprite.renderer.material.mainTextureOffset = new Vector2(xOffset, yOffset);
	}
}
