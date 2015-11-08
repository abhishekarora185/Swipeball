using UnityEngine;
using System.Collections;

public class BallBehaviour : MonoBehaviour {

	// Controls the movement of the player's ball

	// Determines if the ball is dead or not
	public bool isDead;
	// The last position of the ball, needed for raycasting
	public Vector3 lastPosition;
	// The multiplier to the force that user input adds to the ball
	private float inputSensitivity;
	// The point on the map where the ball respawns
	private Vector3 respawnPoint;
	// The physics body of the ball
	private Rigidbody2D ballBody;
	// The start position of a swipe input
	private Vector2 initialPosition;
	// The number of frames for which the finger is held down during a swipe
	private int swipeframes;

	// Use this for initialization
	void Start () {
		this.gameObject.name = SwipeballConstants.GameObjectNames.Game.Ball;
		this.inputSensitivity = 2.0f;
		this.ballBody = this.gameObject.GetComponent<Rigidbody2D>();
		this.respawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
		this.respawnPoint.z = 0;
		this.isDead = false;

		this.gameObject.tag = SwipeballConstants.GameObjectNames.ObjectTags.ActiveEntityTag;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMovementFromUserInput();
		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().entityPositions.Add(this.gameObject.transform.position);
		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().KillBallIfOutOfBounds(this.gameObject);
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
			Vector2 dragDistance = finalPosition - this.initialPosition;

			// Limit the input force by the average of the width and height
			if(dragDistance.magnitude > (Screen.width + Screen.height) / 2)
			{
				dragDistance = (Screen.width + Screen.height) * dragDistance.normalized / 2;
			}

			Vector2 forceVector = this.inputSensitivity * dragDistance / 2;

			this.ballBody.velocity = Vector2.zero;
			this.ballBody.AddForce(forceVector, ForceMode2D.Force);

			PhysicsHacks.AddRetardingForce(this.ballBody);
		}
	}

}
