using UnityEngine;
using System.Collections;

public class BallBehaviour : MonoBehaviour {

	// Controls the movement of the player's ball

	// Determines if the ball is dead or not
	public bool isDead;
	// The last position of the ball, needed for raycasting
	public Vector3 lastPosition;
	// The point on the map where the ball respawns
	private Vector3 respawnPoint;
	// The physics body of the ball
	private Rigidbody2D ballBody;
	// The start position of a swipe input
	private Vector2 initialPosition;
	// Since the ball changes light range while detecting input, it is easy to get incorrect values from other sources manipulating it
	// Thus, store both ranges adjustable by input
	private float inputLightRangeOff;
	private float inputLightRangeOn;

	// Use this for initialization
	void Start () {
		this.gameObject.name = SwipeballConstants.GameObjectNames.Game.Ball;
		this.ballBody = this.gameObject.GetComponent<Rigidbody2D>();
		this.respawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
		this.respawnPoint.z = 0;
		this.isDead = false;
		this.inputLightRangeOff = this.gameObject.GetComponent<Light>().range;
		this.inputLightRangeOn = this.gameObject.GetComponent<Light>().range * SwipeballConstants.Effects.BallMoveLightRangeMagnify;

		this.gameObject.tag = SwipeballConstants.GameObjectNames.ObjectTags.ActiveEntityTag;

		GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialBehaviour).GetComponent<TutorialBehaviour>().tutorialPlayQueue.Enqueue(SwipeballConstants.Tutorial.Ball);
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialBehaviour).GetComponent<TutorialBehaviour>().isTutorialPlaying)
		{
			UpdateMovementFromUserInput();
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().entityPositions.Add(this.gameObject.transform.position);
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().KillBallIfOutOfBounds(this.gameObject);
		}
	}

	private void UpdateMovementFromUserInput()
	{
		// Movement updates through user input, depending on the Control Mode setting

		if (SaveDataHandler.GetLoadedSaveData().controlMode == SwipeballConstants.ControlMode.DragAndRelease)
		{
			// When the user's finger embraces the screen
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.initialPosition = Input.mousePosition;
			}

			// While the user caresses the screen gently
			if (Input.GetKey(KeyCode.Mouse0))
			{

			}

			// When the user is done with the screen for now
			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				Vector2 finalPosition = Input.mousePosition;
				Vector2 dragDistance = finalPosition - this.initialPosition;

				Vector2 forceVector = SwipeballConstants.Input.DragAndReleaseInputSensitivity * dragDistance;

				// Limit the input force by the average of the width and height
				if (forceVector.magnitude > PhysicsHacks.MaximumForce())
				{
					forceVector = PhysicsHacks.MaximumForce() * forceVector.normalized;
				}

				this.ballBody.velocity = Vector2.zero;
				this.ballBody.AddForce(forceVector, ForceMode2D.Force);

				PhysicsHacks.AddRetardingForce(this.ballBody);
			}
		}
		else if (SaveDataHandler.GetLoadedSaveData().controlMode == SwipeballConstants.ControlMode.FollowSwipe)
		{

			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				this.ballBody.velocity = Vector3.zero;

				// A rudimentary animation
				if (this.gameObject.GetComponent<Light>() != null)
				{
					this.gameObject.GetComponent<Light>().range = this.inputLightRangeOn;
				}
			}

			if (Input.GetKey(KeyCode.Mouse0))
			{
				Vector2 finalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector2 dragDistance = finalPosition - this.initialPosition;

				Vector2 forceVector = SwipeballConstants.Input.FollowSwipeInputSensitivity * dragDistance;

				this.ballBody.AddForce(forceVector, ForceMode2D.Force);
				PhysicsHacks.AddRetardingForce(this.ballBody);

				if (this.ballBody.velocity.magnitude > PhysicsHacks.MaximumVelocity())
				{
					this.ballBody.velocity = PhysicsHacks.MaximumVelocity() * this.ballBody.velocity.normalized;
				}

				this.initialPosition = finalPosition;
			}

			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				if (this.gameObject.GetComponent<Light>() != null)
				{
					this.gameObject.GetComponent<Light>().range = this.inputLightRangeOff;
				}
			}
		}
	}

}
