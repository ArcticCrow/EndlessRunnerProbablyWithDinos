using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

	#region Public Variables
	[Tooltip("Should the game object look for a game object with the player tag automatically?")]
	public bool followPlayer = false;
	[Tooltip("The target transform the game object will follow. You may leave this empty if 'followPlayer' is ticked.")]
	public Transform target;

	[Header("Position Offsetting")]
	public bool calculateOffsetAtStart = true;
	public bool ignoreX, ignoreY, ignoreZ;
	[Tooltip("The positional offset towards the followed object. You may leave this empty if 'calcualteOffsetAtStart' is ticked.")]
	public Vector3 offset;
	[Tooltip("The distance that the gameobject should look ahead of the target in a direction when following.")]
	public Vector2 lookAheadDistance = Vector2.zero;

	[Header("Smoothing")]
	[Tooltip("How fast should the game object snap to the targets position?")]
	public float smoothingTime = 0.05f;
	[Tooltip("How fast should the game object snap to the targets position?")]
	public float maxTransitionSpeed = 1f;

	[Tooltip("Should the game object snap back to the targets position at a different speed?")]
	public bool useDifferentReturnSpeed = false;
	public float returnTime = 0.01f;
	#endregion


	#region Private Variables
	private Vector3 lastPos;
	private Vector3 velocity = Vector3.zero;
	private Vector3 lookAheadOffset;
	#endregion


	// Use this for initialization
	void Start () {
		if (followPlayer)
		{
			target = GameObject.FindGameObjectWithTag("Player").transform;
		}
		if (target == null)
		{
			throw new System.Exception("No target to follow was given or found!");
		}

		if (calculateOffsetAtStart)
		{
			offset = transform.position - target.position;
			lastPos = target.position;

			if (ignoreX)
			{
				offset.x = 0;
			}
			if (ignoreY)
			{
				offset.y = 0;
			}
			if (ignoreZ)
			{
				offset.z = 0;
			}
		}

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (target != null)
		{
			Vector3 targetPosition = target.position + offset;
			if (!useDifferentReturnSpeed || lastPos != target.position)
			{
				// Calculate the look ahead offset (ignoring negative y values to avoid wonkiness)
				// TODO smooth look ahead value to avoid massive leaps in position
				lookAheadOffset = Vector3.Scale((target.position - lastPos), new Vector3(1, 1, 0));
				lookAheadOffset.x *= lookAheadDistance.x;
				lookAheadOffset.y *= (lookAheadOffset.y < 0) ? 0 : lookAheadDistance.y;
				targetPosition += lookAheadOffset;

				// Smoothly reposition to the new position using smoothing time and max speed
				transform.position = Vector3.SmoothDamp(transform.position, targetPosition,
					ref velocity, smoothingTime, maxTransitionSpeed);
			}
			else
			{
				// Smoothly reposition to the new position using return time
				transform.position = Vector3.SmoothDamp(transform.position, targetPosition,
					ref velocity, returnTime);
			}

			// Store targets last position for testing in next cycle
			lastPos = target.position;
		}
	}
}
