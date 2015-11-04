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
        this.isLethal = false;
		this.reorientCounter = 0;
		this.reorientationDelay = 100;
		this.reorientationSensitivity = 20.0f;
		this.repulsionSensitivity = 50.0f;
        this.explosionSensitivity = 5.0f;
        this.isDead = false;
        this.deathScore = 30;

        // UI elements come first in the sorting layer, and then any game entities
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

        if(this.gameObject.GetComponent<Light>() != null)
        {
            this.gameObject.GetComponent<Light>().color = Color.cyan;
        }
	}
	
	// Update is called once per frame
	void Update () {
		SpawnBehaviour.entityPositions.Add(this.gameObject.transform.position);
        PerformMineUpdates();
	}

    // Periodically reorients the mines in the direction of the player and does some animations
	private void PerformMineUpdates()
	{
		this.reorientCounter = (this.reorientCounter + 1) % this.reorientationDelay;
        
		if (!this.isDead && GameObject.Find(SwipeballConstants.EntityNames.Ball) != null && this.reorientCounter == 0)
		{
            // If this is the first time the mine is reorienting, turn the mine lethal
            if(this.isLethal == false)
            {
                if(this.gameObject.GetComponent<Light>() != null)
                {
                    this.gameObject.GetComponent<Light>().color = Color.red;
                    this.isLethal = true;
                }
            }
			// Reorient the mines in the direction of the ball
			Vector3 ballPosition = GameObject.Find(SwipeballConstants.EntityNames.Ball).GetComponent<Transform>().position;

			Vector3 directionVector = (ballPosition - this.gameObject.GetComponent<Transform>().position);

			this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(reorientationSensitivity * directionVector, ForceMode2D.Force);
		    PhysicsHacks.AddRetardingForce(this.gameObject.GetComponent<Rigidbody2D>());
        }
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// Detect collisions with either the player's ball or the cleaver
		if (this.isLethal && collision.gameObject.name == SwipeballConstants.EntityNames.Ball)
		{
			// Death animation
            BallBehaviour.isDead = true;
            CleaverBehaviour.powerLevel = 0;
            StartCoroutine(AnimationBehaviour.PlayDeathAnimation(collision.gameObject));
		}

		if (collision.gameObject.name == SwipeballConstants.EntityNames.Mine)
		{
			// Colliding mines repel each other (they stick together if this is not done, and mines shouldn't behave like jelly)

			collision.gameObject.GetComponent<Rigidbody2D>().AddForce(repulsionSensitivity * (collision.gameObject.transform.position - this.gameObject.transform.position));
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(repulsionSensitivity * (this.gameObject.transform.position - collision.gameObject.transform.position));
		}

		if (this.isLethal && collision.gameObject.name == SwipeballConstants.EntityNames.Cleaver && CleaverBehaviour.powerLevel > 0)
		{
            this.isDead = true;
            StartCoroutine(AnimationBehaviour.PlayDeathAnimation(this.gameObject));

            GameObject cleaver = collision.gameObject;
            GameObject ball = GameObject.Find(SwipeballConstants.EntityNames.Ball);

            // Diagonal of the game world
            float largestDistance = (Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0))).magnitude;

            // Apart from destroying the mine, give the cleaver and the player a little push from the "explosion" (neighbouring mines are resilient to such lethal force, just cause)
            cleaver.GetComponent<Rigidbody2D>().AddForce(
                this.explosionSensitivity * 
                ((largestDistance - (cleaver.transform.position - this.gameObject.transform.position).magnitude) / largestDistance) * 
                (cleaver.transform.position - this.gameObject.transform.position).normalized, ForceMode2D.Impulse
            );
            ball.GetComponent<Rigidbody2D>().AddForce(
                this.explosionSensitivity *
                ((largestDistance - (ball.transform.position - this.gameObject.transform.position).magnitude) / largestDistance) *
                (ball.transform.position - this.gameObject.transform.position).normalized, ForceMode2D.Impulse
            );

            // Let the spawner know it can add more mines
            SpawnBehaviour.minesOnField--;

            // Increase the score
            Scorekeeping.IncreaseScore(this.deathScore);
		}
	}

}
