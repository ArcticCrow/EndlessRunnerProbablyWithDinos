using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    #region Public Variables
    // Reference to the player game object
    public GameObject player;
    // Reference to the camera that is supposed to follow the player
    public GameObject mainCamera;

    // What is the base game speed
    public float startingSpeed = 100f;
    // By how much should the speed increase
    public float acceleration = 1.01f;
    // How often should the speed increase
    public float accelerationInterval = 2f;
    // How fast is the game currently running
    public float currentSpeed = 0;
    #endregion

    #region Private Variables
    // How long until the speed increases
    private float nextIncrease = 0;
    #endregion

    // Use this for initialization
    void Start () {
        if (mainCamera == null)
        {
            mainCamera = Camera.main.gameObject;
        }
        // Reset the increase timer
        nextIncrease = accelerationInterval;

        // Set the base running speed
        currentSpeed = startingSpeed;
	}
	
	// Update is called once per frame
	void Update () {

        // Check if player is still in bounds
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 playerHalfScale = new Vector2(player.transform.localScale.x, player.transform.localScale.y);
        Bounds camBounds = OrthographicBounds(mainCamera.GetComponent<Camera>());
        //Debug.Log(camBounds);

        if (playerPos.x + playerHalfScale.x < camBounds.min.x || playerPos.x - playerHalfScale.x > camBounds.max.x
            || playerPos.y + playerHalfScale.y < camBounds.min.y || playerPos.y - playerHalfScale.y > camBounds.max.y)
        {
            Debug.Log("Player is outside of bounds!");
            // TODO game over
        }

        // calculate camera offset
        float horCamOffset = Mathf.Lerp(0, currentSpeed * Time.deltaTime, Time.time);
        mainCamera.transform.position += new Vector3 (horCamOffset, 0, 0);
	}

    private void FixedUpdate()
    {
        // Everytime the the interval has elapsed, increase the current game speed
        //  and reset the timer
        nextIncrease -= Time.fixedDeltaTime;
        if (nextIncrease <= 0)
        {
            currentSpeed *= acceleration;           // accelerate
            nextIncrease += accelerationInterval;   // reset timer
        }
        //Debug.Log("next increase in '" + nextIncrease + "'; current speed is " + currentSpeed);
    }

    // Calculating camera bounds
    private Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
}
