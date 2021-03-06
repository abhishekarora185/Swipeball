﻿/*
 * Author: Abhishek Arora
 * This is the Behaviour script attached to the Master of all spawning and death of the Ball, the Cleaver and Mines
 * */

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpawnBehaviour : MonoBehaviour {

	// Decides when to spawn mines

	// Prefab references needed for spawning
	public GameObject ballDefinition;
	public GameObject mineDefinition;
	public GameObject cleaverDefinition;

	// The number of mines in play at a given point in time
	public int minesOnField;
	// Have each object update its location every frame so that the spawner knows where not to spawn mines
	public List<Vector3> entityPositions;

	// The value of scale that needs to be applied to the ball, cleaver and mines for the current screen size
	private float objectScalingFactor;
	// The number of entities in play apart from the mines and the walls
	private int numberOfExtraEntities;
	// The number of extra lives the ball currently has
	public int ballLives;
	// The position on the field where the ball spawns
	private Vector3 ballSpawnPosition;
	// The upper limit on the number of mines that can be on the field at a given point in time
	private int maxMinesOnField;
	// The total number of mines spawned in the current wave
	private int minesCreated;
	// The cutoff distance from other entities for deeming a spawn point worthy of being selected for the next spawn of a mine
	private float spawnThreshold;
	// The number of mines that need to be destroyed in the current wave to increase the max number of mines allowed
	private int waveCount;
	// The number of waves that need to be cleared for one life to be awarded
	private int rewardThreshold;
	// The number of waves cleared since the last reward
	private int wavesClearedSinceLastReward;

	// Use this for initialization
	void Start () {

		// Force high framerate for iOS
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) 
		{
			Application.targetFrameRate = SwipeballConstants.MobileFrameRate;
		}

		this.objectScalingFactor = Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize;

		this.ballLives = 0;
		int initialMaxNumberOfMines = 2;
		this.minesCreated = initialMaxNumberOfMines;
		this.maxMinesOnField = initialMaxNumberOfMines;
		// At the start of a game, extra entities = ball and cleaver
		numberOfExtraEntities = 2;
		minesOnField = 0;
		// Set the spawn threshold to half the diagonal of the level
		this.spawnThreshold = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)).magnitude;
		this.waveCount = 6;
		this.rewardThreshold = 2;
		this.wavesClearedSinceLastReward = 0;
		this.entityPositions = new List<Vector3>();

		UIOperations.SetTextProperties();
		ConfigureAndSpawnBallAndCleaver();
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialBehaviour).GetComponent<TutorialBehaviour>().isTutorialPlaying)
		{
			CleanPositionList();
			ConfigureAndSpawnMines();
		}
	}

	void OnApplicationQuit()
	{
		FacebookSession.TerminateNetworkThread();
	}

	private void ConfigureAndSpawnBallAndCleaver()
	{
		this.ballSpawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.7f, 0.5f));
		this.ballSpawnPosition.z = 0.0f;
		Vector3 cleaverSpawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.3f, 0.5f));
		cleaverSpawnPosition.z = 0.0f;

		// Scale all objects according to screen size
		ballDefinition.transform.localScale = new Vector3(this.objectScalingFactor, this.objectScalingFactor, 0.0f);
		cleaverDefinition.transform.localScale = new Vector3(this.objectScalingFactor/SwipeballConstants.Scaling.CleaverScaleMultiplier, this.objectScalingFactor/SwipeballConstants.Scaling.CleaverScaleMultiplier, 0.0f);
		mineDefinition.transform.localScale = new Vector3(this.objectScalingFactor, this.objectScalingFactor, 0.0f);
		ballDefinition.GetComponent<Light>().range = this.objectScalingFactor;
		ballDefinition.GetComponent<Light>().intensity = SwipeballConstants.Effects.LightIntensity;
		cleaverDefinition.GetComponent<Light>().range = this.objectScalingFactor * SwipeballConstants.Scaling.CleaverScaleMultiplier / 2;
		cleaverDefinition.GetComponent<Light>().intensity = this.objectScalingFactor * SwipeballConstants.Scaling.CleaverScaleMultiplier / 2;
		cleaverDefinition.GetComponent<Light>().intensity = SwipeballConstants.Effects.LightIntensity;
		mineDefinition.GetComponent<Light>().range = this.objectScalingFactor;
		mineDefinition.GetComponent<Light>().intensity = SwipeballConstants.Effects.LightIntensity;

		// Spawn the ball and cleaver
		Instantiate(ballDefinition, this.ballSpawnPosition, Quaternion.identity);
		Instantiate(cleaverDefinition, cleaverSpawnPosition, Quaternion.identity);

		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper).GetComponent<Scorekeeping>().DisplayLives();
	}

	private void ConfigureAndSpawnMines()
	{
		while (minesOnField < maxMinesOnField)
		{
			// Choose to spawn a mine at any point along the boundary of the play area
			Vector3[] spawnPositions = {
				Camera.main.ViewportToWorldPoint(new Vector3(0.1f, Random.value)),
				Camera.main.ViewportToWorldPoint(new Vector3(0.9f, Random.value)),
				Camera.main.ViewportToWorldPoint(new Vector3(Random.value, 0.1f)),
				Camera.main.ViewportToWorldPoint(new Vector3(Random.value, 0.9f))
			};
			float[] spawnPositionScores = { 0, 0, 0, 0 };

			Vector3 finalSpawnPosition = Vector3.zero;
			float maxSpawnPoints = 0;

			// Give each of the spawn positions scores based on their proximity to the entities (ball, cleaver, other mines)
			foreach (Vector3 entityPosition in entityPositions)
			{
				for (int position = 0; position < spawnPositions.Length; position++)
				{
					float distance = (spawnPositions[position] - entityPosition).magnitude;
					if (distance > this.spawnThreshold)
					{
						// This spawn point will not conflict with this entity's position
						spawnPositionScores[position] += distance;
					}
				}
			}

			for (int position = 0; position < spawnPositions.Length; position++)
			{
				if (spawnPositionScores[position] >= maxSpawnPoints)
				{
					maxSpawnPoints = spawnPositionScores[position];
					finalSpawnPosition = spawnPositions[position];
				}
			}

			// Spawn position's z co-ordinate is defaulting to the camera's plane for some reason, so set it back to the plane of the play area
			finalSpawnPosition.z = 0.0f;

			// Create a brand new instance of the mine
			GameObject newMine = mineDefinition;
			Instantiate(newMine, finalSpawnPosition, Quaternion.identity);
			minesOnField++;
			this.minesCreated++;
		}

		// Increasing difficulty as the game progresses by adding more mines
		if (this.minesCreated == this.waveCount)
		{
			this.waveCount += 5;
			this.minesCreated = 0;
			this.maxMinesOnField++;
			this.wavesClearedSinceLastReward++;

			GameObject scorekeeper = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper);
			scorekeeper.GetComponent<Scorekeeping>().LevelUp();
			scorekeeper.GetComponent<Scorekeeping>().DisplayLevel();
		}

		if (this.wavesClearedSinceLastReward == this.rewardThreshold)
		{
			// Once every few levels, add a life
			this.ballLives++;
			this.wavesClearedSinceLastReward = 0;

			if(this.rewardThreshold > 1)
			{
				// The game gets ridiculously hard later on, so start giving out lives more often!
				this.rewardThreshold--;
			}

			GameObject scorekeeper = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper);
			scorekeeper.GetComponent<Scorekeeping>().DisplayLives();
		}
	}

	// Pop older entries, and restrict the size of this list to the number of mines + number of extra entities
	private void CleanPositionList()
	{
		if(entityPositions.Count > this.maxMinesOnField + numberOfExtraEntities)
		{
			entityPositions.RemoveRange(0, entityPositions.Count - (this.maxMinesOnField + numberOfExtraEntities));
		}
	}

	// Handles post-death animation processing of killed entities
	public void KillObject(GameObject deadObject)
	{
		if(deadObject.name == SwipeballConstants.GameObjectNames.Game.Ball)
		{
			if (this.ballLives <= 0)
			{
				// Kill everything. Existence ceases to have meaning
				foreach (GameObject activeEntity in GameObject.FindGameObjectsWithTag(SwipeballConstants.GameObjectNames.GameObjectTags.ActiveEntityTag))
				{
					DestroyObject(activeEntity);
				}

				// Game over like a five-point palm exploding heart punch
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper).GetComponent<Scorekeeping>().ReassembleLeaderboardList();
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper).GetComponent<Scorekeeping>().SaveHighScore();
				GameOver.CreateGameOverMenu();
			}
			else
			{
				DestroyObject(deadObject);

				// Render all mines dormant
				foreach (GameObject activeEntity in GameObject.FindGameObjectsWithTag(SwipeballConstants.GameObjectNames.GameObjectTags.ActiveEntityTag))
				{
					if (activeEntity.name == SwipeballConstants.GameObjectNames.Game.Mine && activeEntity.GetComponent<MineBehaviour>() != null)
					{
						activeEntity.GetComponent<MineBehaviour>().DormantState();
					}
				}

				this.ballLives--;

				// Respawn the ball
				Instantiate(ballDefinition, this.ballSpawnPosition, Quaternion.identity);
				StartCoroutine(SwipeballAnimation.PlayRespawnAnimation());
				
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper).GetComponent<Scorekeeping>().DisplayLives();
			}
		}
		else
		{
			DestroyObject(deadObject);
		}
	}

	// Called by mines, the cleaver and the ball, this checks if the given entity is outside of the viewport
	// (which would mean that the laws of physX have been compromised), and ends the game if so, since there is no point in living on
	// As there have been no such untoward incidents with the current applied variables (various experimentally-determined numerical values), this should not be regularly triggered
	// And just serves as a worst-case precaution, a graceful degradation of sorts
	public void KillBallIfOutOfBounds(GameObject entity)
	{
		Vector3 entityPosition = entity.transform.position;
		Camera mainCamera = Camera.main;
		if (entityPosition.x > mainCamera.ViewportToWorldPoint(new Vector3(1, 1)).x || 
			entityPosition.y > mainCamera.ViewportToWorldPoint(new Vector3(1, 1)).y || 
			entityPosition.x < mainCamera.ViewportToWorldPoint(new Vector3(0, 0)).x || 
			entityPosition.y < mainCamera.ViewportToWorldPoint(new Vector3(0, 0)).y)
		{
			if(entity.name == SwipeballConstants.GameObjectNames.Game.Ball)
			{
				// If the ball is out of bounds, don't do anything; if the user has lives to spare, he will continue playing
			}
			else
			{
				// If the cleaver or a mine is out of bounds, the game is not recoverable, and must be ended immediately
				this.ballLives = 0;
			}
			KillBall();
		}
	}

	// End the game if the laws of physX have been violated or if the Ball has collided with a Mine
	public void KillBall()
	{
		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead = true;
		if (this.ballLives == 0)
		{
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.Cleaver).GetComponent<CleaverBehaviour>().powerLevel = 0;
		}
		StartCoroutine(SwipeballAnimation.PlayDeathAnimation(GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball)));
	}

}
