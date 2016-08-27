/*
 * Author: Abhishek Arora
 * This is the Behaviour script attached to the Ball which the player controls in the Game level
 * */

using UnityEngine;
using System.Collections;

public class BallBehaviour : MonoBehaviour {

	// Determines if the ball is dead or not
	public bool isDead;
	// The physics body of the ball
	private Rigidbody2D ballBody;
	// The start position of a swipe input
	private Vector2 initialPosition;
	// Since the ball changes the range of its light while detecting input (as a minimal feedback mechanism), 
	// it is easy to get incorrect values from other sources manipulating it (like collisions)
	// Thus, store both ranges adjustable by input
	private float inputLightRangeOff;
	private float inputLightRangeOn;

	// Use this for initialization
	void Start () {
		this.gameObject.name = SwipeballConstants.GameObjectNames.Game.Ball;
		this.ballBody = this.gameObject.GetComponent<Rigidbody2D>();
		this.isDead = false;
		this.inputLightRangeOff = this.gameObject.GetComponent<Light>().range;
		this.inputLightRangeOn = this.gameObject.GetComponent<Light>().range * SwipeballConstants.Effects.BallMoveLightRangeMagnify;

		this.gameObject.tag = SwipeballConstants.GameObjectNames.GameObjectTags.ActiveEntityTag;

		GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialBehaviour).GetComponent<TutorialBehaviour>().tutorialPlayQueue.Enqueue(SwipeballConstants.Tutorial.Ball);
	}
	
	// Update is called once per frame
	void Update () {
		// If we're not in the middle of a tutorial, make things work like they're supposed to
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
			// Touch
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.initialPosition = Input.mousePosition;
			}

			// Drag
			if (Input.GetKey(KeyCode.Mouse0))
			{

			}

			// Release
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
			// Touch
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				this.ballBody.velocity = Vector3.zero;

				// A rudimentary animation that serves the purpose of input feedback too
				if (this.gameObject.GetComponent<Light>() != null)
				{
					this.gameObject.GetComponent<Light>().range = this.inputLightRangeOn;
				}
			}

			// Drag
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

			// Release
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
