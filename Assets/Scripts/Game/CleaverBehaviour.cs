using UnityEngine;
using System.Collections;

public class CleaverBehaviour : MonoBehaviour {

	// Handles cleaver behaviour

	// The "battery" level of the cleaver, on whose progressive depletion it becomes sluggish and loses its capability to destroy mines
	public int powerLevel;
	// The maximum battery level of the cleaver
	public int maxPower;
	// The last position of the cleaver, needed for raycasting
	public Vector3 lastPosition;
	// The initial mass of the cleaver
	private float initialMass;
	// The maximum mass that can be added to the cleaver through power drain
	private float maxAdditionalMass;
	// The multiplier to the power restored by an impact from the ball
	private float chargeSensitivity;
	// The multiplier to the force which prevents the cleaver from sticking to walls
	private float repulsionSensitivity;
	
	// Use this for initialization
	void Start () {
		this.gameObject.name = SwipeballConstants.GameObjectNames.Game.Cleaver;
		this.maxPower = 5000;
		this.powerLevel = maxPower;
		this.initialMass = this.gameObject.GetComponent<Rigidbody2D>().mass;
		this.maxAdditionalMass = 3;
		this.chargeSensitivity = 20.0f;
		this.repulsionSensitivity = 50.0f;
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePowerLevel();
		PhysicsHacks.AddRetardingForce(this.gameObject.GetComponent<Rigidbody2D>());
		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().entityPositions.Add(this.gameObject.transform.position);
		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().EndGameIfOutOfBounds(this.gameObject);
	}

	private void UpdatePowerLevel()
	{
		// Drain power every cycle, and set the mass of the cleaver according to remaining power in order to make it sluggish
		if(this.powerLevel > 0)
		{
			this.powerLevel--;
			this.gameObject.GetComponent<Rigidbody2D>().mass = this.initialMass + ((this.maxPower - this.powerLevel) / this.maxPower) * this.maxAdditionalMass;
		}

		if(this.powerLevel > this.maxPower/2 && this.gameObject.GetComponent<Light>() != null)
		{
            this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.HighPower;
		}
		else if (this.powerLevel > 0 && this.powerLevel <= this.maxPower / 2 && this.gameObject.GetComponent<Light>() != null)
		{
			this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.LowPower;
		}
		else if (this.powerLevel ==0 && this.gameObject.GetComponent<Light>() != null)
		{
			this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.NoPower;
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.VerticalWalls || collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.HorizontalWalls)
		{
			// In order to make sure the cleaver never gets stuck to a wall, we will repel it with a force proportional to its mass whenever it comes in contact with one
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(this.repulsionSensitivity * this.gameObject.GetComponent<Rigidbody2D>().mass * (this.gameObject.transform.position - new Vector3 (collision.contacts[0].point.x, collision.contacts[0].point.y, 0.0f)).normalized);
		}
		
		if(collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.Ball)
		{
			// Charge power according to the relative velocity of impact
			this.powerLevel = (this.powerLevel + (int)(this.chargeSensitivity * collision.relativeVelocity.magnitude));
			if(this.powerLevel > this.maxPower)
			{
				this.powerLevel = this.maxPower;
			}
			this.gameObject.GetComponent<Rigidbody2D>().mass = this.initialMass + ((this.maxPower - this.powerLevel) / this.maxPower) * this.maxAdditionalMass;
		}
	}
}
