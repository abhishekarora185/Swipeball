﻿using UnityEngine;
using System.Collections;

public class BallBehaviour : MonoBehaviour {

	// Controls the movement of the player's ball

    // Determines if the ball is dead or not
    public static bool isDead;
	// The multiplier to the force that user input adds to the ball
	private float inputSensitivity;
	// The physics body of the ball
	private Rigidbody2D ballBody;
	// The start position of a swipe input
	private Vector2 initialPosition;
	// The number of frames for which the finger is held down during a swipe
	private int swipeframes;

	// Use this for initialization
	void Start () {
        this.gameObject.name = SwipeballConstants.EntityNames.Ball;
		this.inputSensitivity = 9.5f;
		this.ballBody = this.gameObject.GetComponent<Rigidbody2D>();
        isDead = false;

        // UI elements come first in the sorting layer, and then any game entities
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMovementFromUserInput();
		SpawnBehaviour.entityPositions.Add(this.gameObject.transform.position);
	}

	private void UpdateMovementFromUserInput()
	{
		// Movement updates through user input

		// When the user's finger embraces the screen
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.initialPosition = Input.mousePosition;
			this.swipeframes = 0;
		}

		// While the user caresses the screen gently
		if (Input.GetKey(KeyCode.Mouse0))
		{
			this.swipeframes++;
		}

		// When the user is done with the screen for now
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			Vector2 finalPosition = Input.mousePosition;
			Vector2 forceVector = this.inputSensitivity * (finalPosition - this.initialPosition) / this.swipeframes;

			this.ballBody.velocity = Vector2.zero;
			this.ballBody.AddForce(forceVector, ForceMode2D.Force);

            PhysicsHacks.AddRetardingForce(this.ballBody);
		}
	}

}