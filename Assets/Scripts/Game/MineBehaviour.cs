using UnityEngine;
using System.Collections;

public class MineBehaviour : MonoBehaviour {

	// Flag to check if the mine can kill the player/ be killed by the cleaver
	private bool isLethal;
	// The number of cycles since the last reorientation
	private int reorientCounter;
	// The number of cycles till the mines reorient themselves towards the ball
	// The first reorientation also turns the mine lethal
	private int reorientationDelay;
	// The multiplier to the force added to the mine for reorientation
	private float reorientationSensitivity;
	// The multiplier to the force with which two mines repel each other
	private float repulsionSensitivity;
	// The multiplier to the explosive force released during mine death
	private float explosionSensitivity;
	// Indicates whether or not the mine has been killed
	private bool isDead;
	// Should near misses be rewarded at this moment or not?
	private bool nearMissTriggered;
	// If this mine is bumped into another by the ball, the additional collision should be rewarded
	private bool bumped;

	// Use this for initialization
	void Start () {
		this.reorientationDelay = 100;
		this.reorientationSensitivity = 20.0f;
		this.repulsionSensitivity = 50.0f;
		this.explosionSensitivity = 3.0f;
		this.isDead = false;
		this.nearMissTriggered = false;
		this.bumped = false;
		this.DormantState();

		this.gameObject.tag = SwipeballConstants.GameObjectNames.ObjectTags.ActiveEntityTag;
	}
	
	// Update is called once per frame
	void Update () {
		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().entityPositions.Add(this.gameObject.transform.position);
		PhysicsHacks.AddRetardingForce(this.gameObject.GetComponent<Rigidbody2D>());
		PerformMineUpdates();
		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().KillBallIfOutOfBounds(this.gameObject);
		CheckForNearMiss();
	}

	// Periodically reorients the mines in the direction of the player and does some animations
	private void PerformMineUpdates()
	{
		this.reorientCounter = (this.reorientCounter + 1) % this.reorientationDelay;

		if (!this.isDead && GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball) != null && !GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead && this.reorientCounter == 0)
		{
			// If this is the first time the mine is reorienting, turn the mine lethal
			if(!this.isLethal)
			{
				if(this.gameObject.GetComponent<Light>() != null)
				{
					this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Mine.Hostile;
					this.isLethal = true;
				}
			}

			if (this.bumped)
			{
				// Revenge time
				this.bumped = false;
			}

			// Reorient the mines in the direction of the ball
			Vector3 ballPosition = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<Transform>().position;

			Vector3 directionVector = (ballPosition - this.gameObject.GetComponent<Transform>().position);

			this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(reorientationSensitivity * directionVector, ForceMode2D.Force);
			PhysicsHacks.AddRetardingForce(this.gameObject.GetComponent<Rigidbody2D>());
		}
	}

	private void CheckForNearMiss()
	{
		GameObject ball = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball);;
		if (!this.isDead && ball != null && !ball.GetComponent<BallBehaviour>().isDead)
		{
			float distance = (ball.GetComponent<Transform>().position - this.gameObject.GetComponent<Transform>().position).magnitude;
			float maxDistance = ball.GetComponent<Renderer>().bounds.size.x / 2 + this.gameObject.GetComponent<Renderer>().bounds.size.x;

			// If the ball is close by and it hasn't triggered a near miss with this mine, set the near miss flag and wait for the ball to move away
			// Only reward if the mine is in a lethal state
			if (!this.nearMissTriggered && this.isLethal && distance <= maxDistance)
			{
				// The ball has come close, so set the near miss flag and let the user know he's being watched
				this.nearMissTriggered = true;
				if (this.gameObject.GetComponent<Light>() != null)
				{
					this.gameObject.GetComponent<Light>().range *= SwipeballConstants.Effects.MineDisturbLightRangeMagnify;
				}
			}
			else if (this.nearMissTriggered && this.isLethal && distance > maxDistance)
			{
				// Reward the player for surviving the near miss and release the flag
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper).GetComponent<Scorekeeping>().IncreaseScore(SwipeballConstants.ScoreIncrements.MineNearMissed, this.gameObject.transform.position);
				this.nearMissTriggered = false;
				if (this.gameObject.GetComponent<Light>() != null)
				{
					this.gameObject.GetComponent<Light>().range /= SwipeballConstants.Effects.MineDisturbLightRangeMagnify;
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// Detect collisions with either the player's ball or the cleaver
		if (collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.Ball)
		{
			if (this.isLethal)
			{
				// Game over!
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().KillBall();
			}
			else
			{
				// Bump
				StartCoroutine(SwipeballAnimation.PlayMineBumpAnimation(this.gameObject));
				this.bumped = true;
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper).GetComponent<Scorekeeping>().IncreaseScore(SwipeballConstants.ScoreIncrements.MineBumped, this.gameObject.transform.position);
			}
		}

		if (collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.Mine)
		{
			// Colliding mines repel each other (they stick together if this is not done, and mines shouldn't behave like jelly)
			if (collision.gameObject.GetComponent<MineBehaviour>().bumped)
			{
				// Reward the player for a chain bump
				StartCoroutine(SwipeballAnimation.PlayMineBumpAnimation(this.gameObject));
				this.bumped = true;
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper).GetComponent<Scorekeeping>().IncreaseScore(SwipeballConstants.ScoreIncrements.MineBumped, this.gameObject.transform.position);
			}

			collision.gameObject.GetComponent<Rigidbody2D>().AddForce(repulsionSensitivity * (collision.gameObject.transform.position - this.gameObject.transform.position));
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(repulsionSensitivity * (this.gameObject.transform.position - collision.gameObject.transform.position));
		}

		if (this.isLethal && collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.Cleaver && GameObject.Find(SwipeballConstants.GameObjectNames.Game.Cleaver).GetComponent<CleaverBehaviour>().powerLevel > 0)
		{
			this.isDead = true;
			StartCoroutine(SwipeballAnimation.PlayDeathAnimation(this.gameObject));

			GameObject cleaver = collision.gameObject;
			GameObject ball = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball);

			// Diagonal of the game world
			float largestDistance = (Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0))).magnitude;

			// Apart from destroying the mine, give the cleaver and the player a little push from the "explosion" (neighbouring mines are resilient to such lethal force, just cause)
			cleaver.GetComponent<Rigidbody2D>().AddForce(
				this.explosionSensitivity *
				Mathf.Pow(((largestDistance - (cleaver.transform.position - this.gameObject.transform.position).magnitude) / largestDistance), 2) *
				(cleaver.transform.position - this.gameObject.transform.position).normalized, ForceMode2D.Impulse
			);

			if (ball != null && ball.GetComponent<Rigidbody2D>().IsAwake())
			{
				ball.GetComponent<Rigidbody2D>().AddForce(
					this.explosionSensitivity *
					((largestDistance - (ball.transform.position - this.gameObject.transform.position).magnitude) / largestDistance) *
					(ball.transform.position - this.gameObject.transform.position).normalized, ForceMode2D.Impulse
				);
			}

			// Let the spawner know it can add more mines
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().minesOnField--;

			// Increase the score
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper).GetComponent<Scorekeeping>().IncreaseScore(SwipeballConstants.ScoreIncrements.MineKilled, this.gameObject.transform.position);
		}
	}

	public void DormantState()
	{
		this.isLethal = false;
		this.nearMissTriggered = false;
		this.bumped = false;
		if (this.gameObject.GetComponent<Light>() != null)
		{
			this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Mine.Dormant;
		}
		this.reorientCounter = 0;
	}

}
