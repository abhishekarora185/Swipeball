/*
 * Author: Abhishek Arora
 * This is the Behaviour script attached to the Cleaver, which the Ball can employ to destroy Mines, in the Game level
 * */

using UnityEngine;
using System.Collections;

public class CleaverBehaviour : MonoBehaviour {

	// Handles cleaver behaviour

	// The "battery" level of the cleaver, on whose depletion it becomes sluggish and loses its capability to destroy mines
	public int powerLevel;
	// The initial mass of the cleaver
	private float initialMass;
	// audio clips to be played at different power levels
	private AudioClip lowPowerClip;
	private AudioClip mediumPowerClip;
	private AudioClip highPowerClip;
	
	// Use this for initialization
	void Start () {
		this.gameObject.name = SwipeballConstants.GameObjectNames.Game.Cleaver;
		this.powerLevel = SwipeballConstants.GameObjectQuantities.Cleaver.MaxPower;
		this.initialMass = this.gameObject.GetComponent<Rigidbody2D>().mass;
		this.lowPowerClip = (AudioClip)Resources.Load(SwipeballConstants.Effects.LowPowerSound);
		this.mediumPowerClip = (AudioClip)Resources.Load(SwipeballConstants.Effects.MediumPowerSound);
		this.highPowerClip = (AudioClip)Resources.Load(SwipeballConstants.Effects.HighPowerSound);

		this.gameObject.tag = SwipeballConstants.GameObjectNames.GameObjectTags.ActiveEntityTag;

		GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialBehaviour).GetComponent<TutorialBehaviour>().tutorialPlayQueue.Enqueue(SwipeballConstants.Tutorial.Cleaver);
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialBehaviour).GetComponent<TutorialBehaviour>().isTutorialPlaying)
		{
			UpdatePowerLevel();
			PhysicsHacks.AddRetardingForce(this.gameObject.GetComponent<Rigidbody2D>());
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().entityPositions.Add(this.gameObject.transform.position);
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().KillBallIfOutOfBounds(this.gameObject);
		}
	}

	private void UpdatePowerLevel()
	{
		// Drain power every cycle, and set the mass of the cleaver according to remaining power in order to make it sluggish
		if(this.powerLevel > 0)
		{
			this.powerLevel--;
			this.gameObject.GetComponent<Rigidbody2D>().mass = this.initialMass + ((SwipeballConstants.GameObjectQuantities.Cleaver.MaxPower - this.powerLevel) / SwipeballConstants.GameObjectQuantities.Cleaver.MaxPower) * SwipeballConstants.GameObjectQuantities.Cleaver.MaxAdditionalMass;
		}
		// High Power
		if(this.powerLevel > SwipeballConstants.GameObjectQuantities.Cleaver.MaxPower/2 && this.gameObject.GetComponent<Light>() != null)
		{
			this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.HighPower;
			if(this.gameObject.GetComponent<AudioSource>() != null)
			{
				this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.highPowerClip);
			}
		}
		// Medium Power
		else if (this.powerLevel > 0 && this.powerLevel <= SwipeballConstants.GameObjectQuantities.Cleaver.MaxPower / 2 && this.gameObject.GetComponent<Light>() != null)
		{
			this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.LowPower;
			if (this.gameObject.GetComponent<AudioSource>() != null)
			{
				this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.mediumPowerClip);
			}
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialBehaviour).GetComponent<TutorialBehaviour>().tutorialPlayQueue.Enqueue(SwipeballConstants.Tutorial.CleaverYellow);
		}
		// Low Power
		else if (this.powerLevel ==0 && this.gameObject.GetComponent<Light>() != null)
		{
			this.gameObject.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.NoPower;
			if (this.gameObject.GetComponent<AudioSource>() != null)
			{
				this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.lowPowerClip);
			}
			if (!GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead || GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().ballLives > 0)
			{
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialBehaviour).GetComponent<TutorialBehaviour>().tutorialPlayQueue.Enqueue(SwipeballConstants.Tutorial.CleaverRed);
			}
		}
	}

	// Unity callback for collision handling
	void OnCollisionEnter2D(Collision2D collision)
	{
		// Collisions with the walls
		if(collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.VerticalWalls || collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.HorizontalWalls)
		{
			// In order to make sure the cleaver never gets stuck to a wall, we will repel it with a force proportional to its mass whenever it comes in contact with one
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(SwipeballConstants.GameObjectQuantities.Cleaver.RepulsionSensitivity * this.gameObject.GetComponent<Rigidbody2D>().mass * (this.gameObject.transform.position - new Vector3 (collision.contacts[0].point.x, collision.contacts[0].point.y, 0.0f)).normalized);
		}
		// Collisions with the player's Ball
		if(collision.gameObject.name == SwipeballConstants.GameObjectNames.Game.Ball)
		{
			// Charge power according to the relative velocity of impact
			this.powerLevel = (this.powerLevel + (int)(SwipeballConstants.GameObjectQuantities.Cleaver.ChargeSensitivity * collision.relativeVelocity.magnitude));
			if(this.powerLevel > SwipeballConstants.GameObjectQuantities.Cleaver.MaxPower)
			{
				this.powerLevel = SwipeballConstants.GameObjectQuantities.Cleaver.MaxPower;
			}
			this.gameObject.GetComponent<Rigidbody2D>().mass = this.initialMass + ((SwipeballConstants.GameObjectQuantities.Cleaver.MaxPower - this.powerLevel) / SwipeballConstants.GameObjectQuantities.Cleaver.MaxPower) * SwipeballConstants.GameObjectQuantities.Cleaver.MaxAdditionalMass;
		}
	}
}
