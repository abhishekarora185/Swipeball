using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Scorekeeping : MonoBehaviour {

	private int score;

	public static int highScore;

	// The number of frames after which the score is increased by 1
	private int scoreThreshold;

	// Counts frames upto the scoreThreshold
	private int scoreCounter;

	// Has the high score been beaten?
	private bool highScoreBeaten;

	// Number of frames for which "New High Score!" will be visible
	private int newHighScoreDisplayFrames;

	// Use this for initialization
	void Start () {
		this.score = 0;
		highScore = LoadHighScore();
		this.scoreThreshold = 500;
		this.scoreCounter = 0;
		this.highScoreBeaten = false;

		GameObject scorekeeperObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper);
		scorekeeperObject.GetComponent<Text>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePersistentScore();
		CheckAgainstHighScore();
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
		if (this.highScoreBeaten == false && this.score > highScore)
		{
			this.highScoreBeaten = true;
			this.newHighScoreDisplayFrames = 100;

			GameObject newHighScoreObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.NewHighScore);
			newHighScoreObject.GetComponent<Text>().fontSize = (int)(newHighScoreObject.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);
			newHighScoreObject.GetComponent<Text>().text = SwipeballConstants.UIText.NewHighScore;
			newHighScoreObject.GetComponent<Text>().enabled = true;
		}

		if (this.newHighScoreDisplayFrames > 0 || GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball) == null || GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead) 
		{
			this.newHighScoreDisplayFrames--;
			if (this.newHighScoreDisplayFrames == 0 || GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball) == null || GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead)
			{
				GameObject.Find(SwipeballConstants.GameObjectNames.Game.NewHighScore).GetComponent<Text>().enabled = false;
			}
		}
	}

	// Score increases triggered by other agents
	public void IncreaseScore(int increasedScore)
	{
		this.score += increasedScore;
	}

	public static int LoadHighScore()
	{
		highScore = 0;

		BinaryFormatter bf = new BinaryFormatter();
		try
		{
			FileStream file = File.Open(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName, FileMode.Open);
			SaveData saveData = (SaveData)bf.Deserialize(file);
			highScore = saveData.highScore;
			file.Close();
		}
		catch(System.Exception e)
		{

		}

		return highScore;
	}

	public void SaveHighScore()
	{
		if(this.score > highScore)
		{
			// Needed for GameOver to display the new high score
			highScore = this.score;

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName, FileMode.OpenOrCreate);

			SaveData saveData = new SaveData();
			saveData.highScore = this.score;

			bf.Serialize(file, saveData);
		}
	}
}
