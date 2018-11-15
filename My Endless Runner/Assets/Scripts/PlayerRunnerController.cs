using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunnerController : MonoBehaviour {

    #region Public Variables
    // How fast the player can move left and right
    public float horizontalAcceleration = 3f;
	public float maxWalkSpeed = 3f;
	public float maxRunSpeed = 5f;

    // How long and forceful can the player jump
    public float jumpForce = 5f;
    public float jumpControlTime = .2f;

	public float maxJumpSpeed = 5f;


	public enum AnimationState
	{
		Idle = 0,
		Walking = 1,
		Running = 2,
		Jumping = 3,
		Airborne = 4,
	}

	public AnimationState state = AnimationState.Idle;
    #endregion

    #region private Variables
    private Rigidbody2D rb;
	private Animator anim;

	private bool flipX = false;
    
    // Flag to determine if player is grounded
    private bool isGrounded = false;

    // Remaining time for jump acceleration
    private float jumpTimeRemaining;
    // Flag to determine if player is still holding down jump
    private bool stoppedJumping = false;

	private Vector3 scale;
    #endregion

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
		anim = transform.GetComponentInChildren<Animator>();
        jumpTimeRemaining = jumpControlTime;
		scale = transform.localScale;
    }
	
	// Update is called once per frame
	private void Update ()
	{
		#region Player Movement & Animation
		if (rb != null)
		{
			float horizontalSpeed = Input.GetAxis("Horizontal") * horizontalAcceleration;

			state = AnimationState.Idle;

			// Apply player velocity
			// Reset velocity to avoid sliding upon direction change
			if ((horizontalSpeed > 0 && rb.velocity.x < 0) || (horizontalSpeed < 0 && rb.velocity.x > 0))
			{
				rb.velocity = new Vector2(rb.velocity.x * 0.1f, rb.velocity.y);
			}
			rb.AddForce(new Vector2(horizontalSpeed, 0), ForceMode2D.Force);

			// Limit velocity (horizontal movement)
			float horMaxSpeed = (Input.GetButton("Walk")) ? maxWalkSpeed : maxRunSpeed;
			Vector2 newLimitedVelocity = rb.velocity;
			newLimitedVelocity.x = (Mathf.Abs(newLimitedVelocity.x) > horMaxSpeed) ? horMaxSpeed * Mathf.Sign(newLimitedVelocity.x) : newLimitedVelocity.x;
			

			if (newLimitedVelocity.x != 0)
			{
				if (Mathf.Abs(newLimitedVelocity.x) <= maxWalkSpeed)
				{
					state = AnimationState.Walking;
				}
				else
				{
					state = AnimationState.Running;
				}
			}

			// Control player jumping 
			if (isGrounded && Input.GetButton("Jump"))
			{
				stoppedJumping = false;
				isGrounded = false;
				rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

				state = AnimationState.Jumping;
			}
			else if (Input.GetButton("Jump") && !stoppedJumping && jumpTimeRemaining > 0)
			{
				// Air control with more powerful jump if the player holds the button longer
				//  and the player still has remainig jump time
				jumpTimeRemaining -= Time.deltaTime;
				rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

				state = AnimationState.Jumping;
			}
			else if (!isGrounded)
			{
				// Player is airborne so disable jumping
				jumpTimeRemaining = 0;
				stoppedJumping = true;

				state = AnimationState.Airborne;
			}

			// Flip character if moving negative direction
			// Check if the player is walking and if the sprite needs to be mirrored
			if (newLimitedVelocity.x > 0f && flipX)
			{
				flipX = false;
				transform.localScale = scale;
			}
			else if (newLimitedVelocity.x < 0f && !flipX)
			{
				flipX = true;
				transform.localScale = Vector3.Scale(scale, new Vector3(-1, 1, 1));
			}

			// Limit velocity (vertical movement)
			newLimitedVelocity.y = rb.velocity.y;
			newLimitedVelocity.y = (newLimitedVelocity.y > maxJumpSpeed) ? maxJumpSpeed : newLimitedVelocity.y;
			rb.velocity = newLimitedVelocity;
		}
		#endregion

		// Apply current state to animator
		anim.SetInteger("state", (int)state);
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.gameObject.CompareTag("Platform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5)
                {
                    isGrounded = true;
                    jumpTimeRemaining = jumpControlTime;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }
    }
}
