using UnityEngine;
using System.Collections.Generic;

public class SpawnBehaviour : MonoBehaviour {

	// Decides when to spawn mines

	// Prefab references needed for spawning
    public GameObject ballDefinition;
	public GameObject mineDefinition;
	public GameObject cleaverDefinition;
	// The number of mines in play at a given point in time
	public static int minesOnField;
	// The number of entities in play apart from the mines and the walls
	public static int numberOfExtraEntities;
	// The upper limit on number of mines that can be on the field at a given point in time
	private int maxMinesOnField;
	// The total number of mines spawned in the current wave
	private int minesCreated;
	// The cutoff distance from other entities for deeming a spawn point worthy of being selected for the next spawn of a mine
	private float spawnThreshold;
	// The number of mines that need to be destroyed in the current wave to increase the max number of mines allowed
	private int waveCount;

	// Have each object update its location every frame so that the spawner knows where not to spawn mines
	public static List<Vector3> entityPositions;

	// Use this for initialization
	void Start () {
		int initialMaxNumberOfMines = 2;
		this.minesCreated = initialMaxNumberOfMines;
		this.maxMinesOnField = initialMaxNumberOfMines;
		// At the start of a game, extra entities = ball and cleaver
		numberOfExtraEntities = 2;
		minesOnField = 0;
        // Set the spawn threshold to half the diagonal of the level
		this.spawnThreshold = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)).magnitude;
		this.waveCount = 5;
		entityPositions = new List<Vector3>();

        Vector3 ballSpawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.7f, 0.5f));
        ballSpawnPosition.z = 0.0f;
        Vector3 cleaverSpawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.3f, 0.5f));
        cleaverSpawnPosition.z = 0.0f;

        // Spawn the ball and cleaver
        Instantiate(ballDefinition, ballSpawnPosition, Quaternion.identity);
        Instantiate(cleaverDefinition, cleaverSpawnPosition, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		CleanPositionList();
		AddMines();
	}

	private void AddMines()
	{
		while (minesOnField < maxMinesOnField)
		{
			// Choose to spawn a mine at any point along the boundary
			Vector3[] spawnPositions = {
				Camera.main.ViewportToWorldPoint(new Vector3(0.1f, Random.value)),
				Camera.main.ViewportToWorldPoint(new Vector3(0.9f, Random.value)),
				Camera.main.ViewportToWorldPoint(new Vector3(Random.value, 0.1f)),
				Camera.main.ViewportToWorldPoint(new Vector3(Random.value, 0.9f))
			};
			float[] spawnPositionScores = { 0, 0, 0, 0 };

			Vector3 finalSpawnPosition = Vector3.zero;
			float maxSpawnPoints = 0;

			// Give each of the spawn positions scores based on their proximity to the entities
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

			// Spawn position's z co-ordinate is defaulting to the camera's plane for some reason
			finalSpawnPosition.z = 0.0f;

			// Create a brand new instance of the mine
			GameObject newMine = mineDefinition;
			Instantiate(newMine, finalSpawnPosition, Quaternion.identity);
			minesOnField++;
			this.minesCreated++;
		}

		// Increasing difficulty as the game progresses by adding more mines
		if (this.minesCreated == this.waveCount + 1)
		{
			this.waveCount += 5;
			this.minesCreated = 0;
			this.maxMinesOnField++;
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
    public static void KillObject(GameObject deadObject)
    {
        if(deadObject.name == SwipeballConstants.EntityNames.Ball)
        {
            // Kill everything. Existence ceases to have meaning
            foreach(GameObject activeEntity in GameObject.FindGameObjectsWithTag(SwipeballConstants.EntityNames.ActiveEntityTag))
            {
                DestroyObject(activeEntity);
            }

            // Game over like a five-point palm exploding heart punch
            Time.timeScale = 0.0f;
        }
        else
        {
            DestroyObject(deadObject);
        }
    }

}
