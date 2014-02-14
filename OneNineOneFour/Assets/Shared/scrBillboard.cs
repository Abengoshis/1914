using UnityEngine;
using System.Collections;

/// <summary>
/// This script causes its object to face the main camera at all times.
/// 
/// Frames are stored in sets next to each other.
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
public class scrBillboard : MonoBehaviour
{
	public Material SpriteSheet;	// The sprite sheet material to be used.
	public float Scale = 1;	// The size of the sprite quad in units.
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
	private float xOffset { get { return FrameSet > 0 ? (float)((FramesPerSet[FrameSet - 1] + frame) * FrameWidth / (float)(FramesPerSet[FrameSet] * FrameWidth)) : frame * FrameWidth / (float)(FramesPerSet[FrameSet] * FrameWidth); } }	// Divide by sheet width.
	private float yOffset { get { return 1 - (float)((angle + 1) * FrameHeight) / sheetHeight; } }	// Divide by sheet height.
	
	// Use this for initialization
	private void Start ()
	{
		this.renderer.material = new Material(SpriteSheet);
		this.transform.localScale = new Vector3(Scale, Scale * (float)FrameHeight / FrameWidth, 1);	// Set the scale to Scale by Scale * FrameHeight / FrameWidth ratio so the texture isn't stretched.
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
		
		// Rotate the sprite to always face the main camera.
		this.transform.LookAt(Camera.main.transform.position);
		
		// Set the material properties.
		this.renderer.material.mainTextureOffset = new Vector2(xOffset, yOffset);
	}
}
