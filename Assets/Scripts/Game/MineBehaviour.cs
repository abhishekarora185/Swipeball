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
	// The points obtained on destroying a mine
	private int deathScore;

	// Use this for initialization
	void Start () {
		this.reorientationDelay = 100;
		this.reorientationSensitivity = 20.0f;
		this.repulsionSensitivity = 50.0f;
		this.explosionSensitivity = 3.0f;
		this.isDead = false;
		this.deathScore = 30;
		this.DormantState();

		this.gameObject.tag = SwipeballConstants.GameObjectNames.ObjectTags.ActiveEntityTag;
	}
	
	// Update is called once per frame
	void Update () {
		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().entityPositions.Add(this.gameObject.transform.position);
		PhysicsHacks.AddRetardingForce(this.gameObject.GetComponent<Rigidbody2D>());
		PerformMineUpdates();
		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().KillBallIfOutOfBounds(this.gameObject);
	}

	// Periodically reorients the mines in the direction of the player and does some animations
	private void PerformMineUpdates()
	{
		this.reorientCounter = (this.reorientCounter + 1) % this.reorientationDelay;

		if (!this.isDead && GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball) != null && !GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead && this.reorientCounter == 0)
		{
			// If this is the first time the mine is reorienting, turn the mine lethal
			if(this.isLethal == false)
			{
				if(this.gameObject.GetComponent<Light>() != null)
				{
					this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Mine.Hostile;
					this.isLethal = true;
				}
			}
			// Reorient the mines in the direction of the ball
			Vector3 ballPosition = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<Transform>().position;

			Vector3 directionVector = (ballPosition - this.gameObject.GetComponent<Transform>().position);

			this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(reorientationSensitivity * directionVector, ForceMode2D.Force);
			PhysicsHacks.AddRetardingForce(this.gameObject.GetComponent<Rigidbody2D>());
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// Detect collisions with either the player's ball or the cleaver
		if (this.isLethal && collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.Ball)
		{
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().RespawnOrDie();
		}

		if (collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.Mine)
		{
			// Colliding mines repel each other (they stick together if this is not done, and mines shouldn't behave like jelly)

			collision.gameObject.GetComponent<Rigidbody2D>().AddForce(repulsionSensitivity * (collision.gameObject.transform.position - this.gameObject.transform.position));
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(repulsionSensitivity * (this.gameObject.transform.position - collision.gameObject.transform.position));
		}

		if (this.isLethal && collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.Cleaver && GameObject.Find(SwipeballConstants.GameObjectNames.Game.Cleaver).GetComponent<CleaverBehaviour>().powerLevel > 0)
		{
			this.isDead = true;
			StartCoroutine(SwipeballAnimation.PlayDeathAnimation(this.gameObject));

			GameObject cleaver = collision.gameObject;
			GameObject ball = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball);

			if (ball != null)
			{
				// Diagonal of the game world
				float largestDistance = (Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0))).magnitude;

				// Apart from destroying the mine, give the cleaver and the player a little push from the "explosion" (neighbouring mines are resilient to such lethal force, just cause)
				cleaver.GetComponent<Rigidbody2D>().AddForce(
					this.explosionSensitivity *
					Mathf.Pow(((largestDistance - (cleaver.transform.position - this.gameObject.transform.position).magnitude) / largestDistance), 2) *
					(cleaver.transform.position - this.gameObject.transform.position).normalized, ForceMode2D.Impulse
				);

				if (ball.GetComponent<Rigidbody2D>().IsAwake())
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
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper).GetComponent<Scorekeeping>().IncreaseScore(this.deathScore);
			}
		}
	}

	public void DormantState()
	{
		this.isLethal = false;
		if (this.gameObject.GetComponent<Light>() != null)
		{
			this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Mine.Dormant;
		}
		this.reorientCounter = 0;
	}

}
