using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunnerController : MonoBehaviour {

    #region Public Variables
    // Reference to GameManager to get GameSpeed
    public GameManager gm;
    // How fast the player can move left and right
    public float moveSpeed = 3f;
    // How long and forceful can the player jump
    public float jumpForce = 7f;
    public float jumpTime = 2f;
    #endregion

    #region private Variables
    private Rigidbody2D rb;
    
    // Flag to determine if player is grounded
    private bool isGrounded = false;

    // Remaining time for jump acceleration
    private float jumpTimeRemaining;
    // Flag to determine if player is still holding down jump
    private bool stoppedJumping = false;
    #endregion

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        jumpTimeRemaining = jumpTime;
    }
	
	// Update is called once per frame
	private void Update () {
        
    }

    private void FixedUpdate()
    {
        #region Player Movement
        if (rb != null)
        {
            // Move player left and right
            float horizontalSpeed = Input.GetAxis("Horizontal") * moveSpeed;
            float verticalSpeed = rb.velocity.y;

            // Control player jumping 
            // FIXME multiple jumping allowed sometimes???
            if (isGrounded)
            {
                if (Input.GetButton("Jump"))
                {
                    verticalSpeed = jumpForce;
                    stoppedJumping = false;
                }
            }
            // If player releases button, stop jumping
            else if (Input.GetButtonUp("Jump"))
            {
                jumpTimeRemaining = 0;
                stoppedJumping = true;
            }
            // Air control with more powerful jump if the player hold the button longer
            //  and the player still has remainig jump time
            else if (Input.GetButton("Jump") && !stoppedJumping && jumpTimeRemaining > 0)
            {
                verticalSpeed = jumpForce;
                jumpTimeRemaining -= Time.deltaTime;
            }

            // apply player velocity
            rb.velocity = new Vector2(gm.currentSpeed + horizontalSpeed, verticalSpeed);
        }
        #endregion
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
                    jumpTimeRemaining = jumpTime;
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
