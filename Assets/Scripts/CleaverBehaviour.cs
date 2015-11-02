using UnityEngine;
using System.Collections;

public class CleaverBehaviour : MonoBehaviour {

	// Handles cleaver behaviour

    // The "battery" level of the cleaver, on whose progressive depletion it becomes sluggish and loses its capability to destroy mines
    public static int powerLevel;
    // The maximum battery level of the cleaver
    public static int maxPower;
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
        this.gameObject.name = SwipeballConstants.EntityNames.Cleaver;
        maxPower = 5000;
        powerLevel = maxPower;
        this.initialMass = this.gameObject.GetComponent<Rigidbody2D>().mass;
        this.maxAdditionalMass = 3;
        this.chargeSensitivity = 20.0f;
		this.repulsionSensitivity = 50.0f;

        // UI elements come first in the sorting layer, and then any game entities
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePowerLevel();
		SpawnBehaviour.entityPositions.Add(this.gameObject.transform.position);
	}

    private void UpdatePowerLevel()
    {
        // Drain power every cycle, and set the mass of the cleaver according to remaining power in order to make it sluggish
        if(powerLevel > 0)
        {
            powerLevel--;
            this.gameObject.GetComponent<Rigidbody2D>().mass = this.initialMass + ((maxPower - powerLevel) / maxPower) * this.maxAdditionalMass;
        }

        if(powerLevel > maxPower/2 && this.gameObject.GetComponent<Light>() != null)
        {
            this.gameObject.GetComponent<Light>().color = Color.green;
        }
        else if (powerLevel > 0 && powerLevel <= maxPower / 2 && this.gameObject.GetComponent<Light>() != null)
        {
            this.gameObject.GetComponent<Light>().color = Color.yellow;
        }
        else if (powerLevel ==0 && this.gameObject.GetComponent<Light>() != null)
        {
            this.gameObject.GetComponent<Light>().color = Color.red;
        }
    }

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.name == SwipeballConstants.EntityNames.VerticalWalls || collision.gameObject.name == SwipeballConstants.EntityNames.HorizontalWalls)
		{
			// In order to make sure the cleaver never gets stuck to a wall, we will repel it with a force proportional to its mass whenever it comes in contact with one
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(this.repulsionSensitivity * this.gameObject.GetComponent<Rigidbody2D>().mass * (this.gameObject.transform.position - new Vector3 (collision.contacts[0].point.x, collision.contacts[0].point.y, 0.0f)));
		}
        
        if(collision.gameObject.name == SwipeballConstants.EntityNames.Ball)
        {
            // Charge power according to the relative velocity of impact
            powerLevel = (powerLevel + (int)(this.chargeSensitivity * collision.relativeVelocity.magnitude));
            if(powerLevel > maxPower)
            {
                powerLevel = maxPower;
            }
            this.gameObject.GetComponent<Rigidbody2D>().mass = this.initialMass + ((maxPower - powerLevel) / maxPower) * this.maxAdditionalMass;
        }
	}
}
