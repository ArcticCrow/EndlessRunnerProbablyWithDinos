using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	const string PLAYER_TAG = "Player";
	const string APOCALYPSE_TAG = "Apocalypse";

	[HideInInspector]
	public static GameManager Instance;

	#region Inspector visible variables
	[Header("Scene References")]
	public GameObject player;
	public GameObject apocalypse;
	public Camera mainCamera;

	[Header("Apocalypse Cloud Behaviour")]
	public float cloudStartVelocity = 1;
	public float cloudAcceleration = 0;
	public float cloudTopVelocity = 2;
	#endregion

	Rigidbody2D apoRB;

	// Use this for initialization
	void Start ()
	{
		// Check to see if an instance of GameManager 
		//  already exists
		CheckInstance();

		// Get and set all required game objects and 
		//  components from the active scene
		FindReferencesInScene();

		SetupApocalypse();
	}

	private void SetupApocalypse()
	{
		apoRB = apocalypse.GetComponent<Rigidbody2D>();
		apoRB.isKinematic = true;
		apoRB.velocity = new Vector2(cloudStartVelocity, 0);
	}

	private void FindReferencesInScene()
	{
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
			if (player == null)
			{
				throw new Exception("No player found in active scene!");
			}
		}
		if (apocalypse == null)
		{
			apocalypse = GameObject.FindGameObjectWithTag(APOCALYPSE_TAG);
			if (apocalypse == null)
			{
				throw new Exception("No apocalypse found in active scene!");
			}
		}
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
			if (mainCamera == null)
			{
				throw new Exception("No main camera found in active scene!");
			}
		}
	}

	private void CheckInstance()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{

	}

	void FixedUpdate()
	{
		apoRB.velocity += Vector2.right * (cloudAcceleration / Time.deltaTime);
		apoRB.velocity = Vector2.ClampMagnitude(apoRB.velocity, cloudTopVelocity);
		Debug.Log(apoRB.velocity);
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
