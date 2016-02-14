using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Scorekeeping : MonoBehaviour {

	private int score;

	public int level;

	// The number of frames after which the score is increased by 1
	private int scoreThreshold;

	// Counts frames upto the scoreThreshold
	private int scoreCounter;

	// Has the high score been beaten?
	private bool highScoreBeaten;

	// Number of frames for which "New High Score!" will be visible
	private int newHighScoreDisplayFrames;

	// Number of frames for which "Level <x>" will be visible
	private int levelUpDisplayFrames;

	// Number of frames for which "Lives : <x>" will be visible
	private int livesDisplayFrames;

	// The clip to be played when the high score is attained
	private AudioClip highScoreClip;

	// Use this for initialization
	void Start () {
		this.score = 0;
		this.level = 1;
		
		this.scoreThreshold = 500;
		this.scoreCounter = 0;
		this.highScoreBeaten = false;
		this.highScoreClip = (AudioClip) Resources.Load(SwipeballConstants.Effects.NewHighScoreSound);

		GameObject scorekeeperObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper);
		scorekeeperObject.GetComponent<Text>().enabled = true;

		GameObject musicObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Music);

		if(SaveDataHandler.GetLoadedSaveData().soundEnabled && musicObject.GetComponent<AudioSource>() != null)
		{
			musicObject.GetComponent<AudioSource>().enabled = true;
		}
		else if (musicObject.GetComponent<AudioSource>() != null)
		{
			musicObject.GetComponent<AudioSource>().enabled = false;
		}

		DisplayLevel();
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePersistentScore();
		CheckAgainstHighScore();
		DisableTemporaryText();
	}

	// Constantly increase the score by 1 after a certain period as a survival reward
	private void UpdatePersistentScore()
	{
		this.scoreCounter = (this.scoreCounter + 1) % this.scoreThreshold;

		if (GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball) != null && !GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead)
		{
			if (this.scoreCounter == 0)
			{
				this.score++;
			}
			this.gameObject.GetComponent<Text>().text = this.score + string.Empty;
		}

	}

	// Checks the current score against the high score to see if it's been beaten, and displays "New High Score!" if it is so
	private void CheckAgainstHighScore()
	{
		if (this.highScoreBeaten == false && this.score > SaveDataHandler.GetLoadedSaveData().highScore)
		{
			this.highScoreBeaten = true;
			this.newHighScoreDisplayFrames = 100;

			GameObject newHighScoreObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.NewHighScore);
			newHighScoreObject.GetComponent<Text>().text = SwipeballConstants.UIText.NewHighScore;
			newHighScoreObject.GetComponent<Text>().enabled = true;

			if(this.gameObject.GetComponent<AudioSource>() != null)
			{
				this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.highScoreClip);
			}
		}
	}

	public void DisplayLevel()
	{
		this.levelUpDisplayFrames = 100;

		GameObject levelUpObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.LevelUp);
		levelUpObject.GetComponent<Text>().text = SwipeballConstants.UIText.Level + this.level;
		levelUpObject.GetComponent<Text>().enabled = true;
	}

	public void DisplayLives()
	{
		if (GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner) != null)
		{
			this.livesDisplayFrames = 500;
			GameObject livesObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Lives);
			livesObject.GetComponent<Text>().text = SwipeballConstants.UIText.Lives + GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().ballLives;
			livesObject.GetComponent<Text>().enabled = true;
		}
	}

	// Disables text that comes up during achievements after a given interval
	private void DisableTemporaryText()
	{
		if (this.newHighScoreDisplayFrames > 0)
		{
			this.newHighScoreDisplayFrames--;
		}
		if (this.levelUpDisplayFrames > 0)
		{
			this.levelUpDisplayFrames--;
		}
		if (this.livesDisplayFrames > 0)
		{
			this.livesDisplayFrames--;
		}

		if (this.newHighScoreDisplayFrames == 0 || (GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball) != null && GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead))
		{
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.NewHighScore).GetComponent<Text>().enabled = false;
		}
		if (this.levelUpDisplayFrames == 0 || (GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball) != null && GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead))
		{
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.LevelUp).GetComponent<Text>().enabled = false;
		}
		if (this.livesDisplayFrames == 0 || (GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball) != null && GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead))
		{
			GameObject.Find(SwipeballConstants.GameObjectNames.Game.Lives).GetComponent<Text>().enabled = false;
		}
	}

	// Score increases triggered by other agents
	public void IncreaseScore(int increasedScore)
	{
		this.score += increasedScore;
	}

	public void SaveHighScore()
	{
		int highScore = SaveDataHandler.GetLoadedSaveData().highScore;
		if(this.score > highScore)
		{
			// Needed for GameOver to display the new high score
			highScore = this.score;

			SaveDataHandler.SetHighScore(highScore);
		}
	}
}
