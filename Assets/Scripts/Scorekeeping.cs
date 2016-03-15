using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class Scorekeeping : MonoBehaviour {

	public GameObject scoreIncrementDefinition;

	private int level;

	private int score;

	// The number of frames after which the score is increased by 1
	private int scoreThreshold;

	// Counts frames upto the scoreThreshold
	private int scoreCounter;

	// Number of frames for which "New High Score!" will be visible
	private int newHighScoreDisplayFrames;

	// Number of frames for which "Level <x>" will be visible
	private int levelUpDisplayFrames;

	// Number of frames for which "Lives : <x>" will be visible
	private int livesDisplayFrames;

	// A stack containing the user's friends' scores (if the leaderboard is ready) and the user's score
	// Used to display congratulatory messages as and when someone's high score is beaten
	private Stack<Dictionary<string, object>> leaderboardStack;

	// A sub-list of FacebookSession.leaderboardUserIdsSorted that replaces it after game over
	private List<string> newLeaderboardUserIdsSorted;

	// The clip to be played when the high score is attained
	private AudioClip highScoreClip;

	// This flag prevents misuse of the leaderboard stack if it is not successfully initialized
	// This happens when the user starts the game before the leaderboard is loaded
	private bool leaderboardStackInitialized;

	// Use this for initialization
	void Start () {
		this.score = 0;
		this.level = 1;

		this.scoreThreshold = 500;
		this.scoreCounter = 0;
		this.highScoreClip = (AudioClip) Resources.Load(SwipeballConstants.Effects.NewHighScoreSound);
		this.leaderboardStackInitialized = false;

		GameObject scorekeeperObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper);
		scorekeeperObject.GetComponent<Text>().enabled = true;

		PrepareLeaderboardStack();

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
		if (!GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialBehaviour).GetComponent<TutorialBehaviour>().isTutorialPlaying)
		{
			UpdatePersistentScore();
			CheckAgainstHighScore();
			DisableTemporaryText();
		}
	}

	// Initializes objects pertaining to leaderboard notifications
	private void PrepareLeaderboardStack()
	{
		this.leaderboardStack = new Stack<Dictionary<string, object>>();

		// Only try to add friends' scores if the leaderboard is ready
		if (SaveDataHandler.GetLoadedSaveData().syncWithFacebook && FacebookSession.IsLeaderboardReady())
		{
			// Push the details of all users above or equal to the user's score onto the stack
			this.newLeaderboardUserIdsSorted = FacebookSession.leaderboardUserIdsSorted;
			int userPositionInLeaderboard = 0;

			foreach (string userId in this.newLeaderboardUserIdsSorted)
			{
				// Create a new object to push that contains the user id
				Dictionary<string, object> userStackObject = FacebookSession.userFriends[userId];
				userStackObject["id"] = userId;

				leaderboardStack.Push(userStackObject);
				if (userId == FacebookSession.user["id"].ToString())
				{
					userPositionInLeaderboard = this.newLeaderboardUserIdsSorted.IndexOf(userId);
					break;
				}
			}

			this.newLeaderboardUserIdsSorted.RemoveAll(userId => this.newLeaderboardUserIdsSorted.IndexOf(userId) <= userPositionInLeaderboard);

			this.leaderboardStackInitialized = true;
		}
		else
		{
			// Push only the user's score into the stack; name will not be required, as the message is simply "New High Score!"
			Dictionary<string, object> userStackObject = new Dictionary<string, object>();
			userStackObject["score"] = SaveDataHandler.GetLoadedSaveData().highScore;

			this.leaderboardStack.Push(userStackObject);
		}
	}

	// Constantly increase the score by 1 after a certain period as a survival reward
	private void UpdatePersistentScore()
	{
		this.scoreCounter = (this.scoreCounter + 1) % this.scoreThreshold;

		if (GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball) != null && !GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<BallBehaviour>().isDead)
		{
			if (this.scoreCounter == 0)
			{
				this.IncreaseScore(SwipeballConstants.ScoreIncrements.Persistent, GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).transform.position);
			}
			this.gameObject.GetComponent<Text>().text = this.score + string.Empty;
		}

	}

	// Checks the current score against the high score to see if it's been beaten, and displays "New High Score!" if it is so
	private void CheckAgainstHighScore()
	{
		int dummyParseResult;

		if (this.leaderboardStack != null && this.leaderboardStack.Count > 0 
			&& this.leaderboardStack.Peek().ContainsKey("score") && System.Int32.TryParse(this.leaderboardStack.Peek()["score"].ToString(), out dummyParseResult) && this.score > System.Int32.Parse(this.leaderboardStack.Peek()["score"].ToString()))
		{
			this.newHighScoreDisplayFrames = 100;

			GameObject newHighScoreObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.NewHighScore);

			if (!FacebookSession.IsLeaderboardReady() || (SaveDataHandler.GetLoadedSaveData().syncWithFacebook && FacebookSession.IsLeaderboardReady() && this.leaderboardStackInitialized && this.leaderboardStack.Peek()["id"].ToString() == FacebookSession.user["id"].ToString()))
			{
				this.leaderboardStack.Pop();
				newHighScoreObject.GetComponent<Text>().text = SwipeballConstants.UIText.NewHighScore;
			}
			else if (SaveDataHandler.GetLoadedSaveData().syncWithFacebook && FacebookSession.IsLeaderboardReady() && this.leaderboardStackInitialized)
			{
				Dictionary<string, object> userData = this.leaderboardStack.Pop();
				// Push the user ID into its new position in the ordered leaderboard list
				this.newLeaderboardUserIdsSorted.Insert(0, userData["id"].ToString());
				// Display the name of your latest victim for glory's sake
				newHighScoreObject.GetComponent<Text>().text = SwipeballConstants.UIText.YouBeatSomeone + userData["name"];
			}
			newHighScoreObject.GetComponent<Text>().enabled = true;

			if(this.gameObject.GetComponent<AudioSource>() != null)
			{
				this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.highScoreClip);
			}
		}
	}

	public void LevelUp()
	{
		this.level++;
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

	// Score increments triggered by game objects; their positions are taken to determine where to display the score increment prefab
	public void IncreaseScore(int increasedScore, Vector3 gameObjectPosition)
	{
		this.score += increasedScore;
		GameObject scoreIncrement = Instantiate(this.scoreIncrementDefinition);
		scoreIncrement.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(gameObjectPosition);
		scoreIncrement.transform.SetParent(GameObject.Find(SwipeballConstants.GameObjectNames.Canvas).transform);
		scoreIncrement.transform.localScale = Vector3.one;
		scoreIncrement.GetComponent<Text>().text = increasedScore + string.Empty;
		scoreIncrement.GetComponent<Text>().color = SwipeballConstants.Colors.UI.ScoreIncrementText;
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

	// Store the changed (if you played well enough) leaderboard list in the Facebook cache, if applicable
	public void ReassembleLeaderboardList()
	{
		if (SaveDataHandler.GetLoadedSaveData().syncWithFacebook && FacebookSession.IsLeaderboardReady() && this.leaderboardStackInitialized)
		{
			// After the game is over, the list's next slot is where this game's score goes
			this.newLeaderboardUserIdsSorted.Insert(0, FacebookSession.user["id"].ToString());

			// Update the leaderboard with the new high score, if any
			if (this.score > SaveDataHandler.GetLoadedSaveData().highScore)
			{
				FacebookSession.userFriends[FacebookSession.user["id"].ToString()]["score"] = this.score + string.Empty;
			}

			// Now, pop the rest of the stack onto the front of this list, and store it
			while (this.leaderboardStack.Count > 0)
			{
				this.newLeaderboardUserIdsSorted.Insert(0, this.leaderboardStack.Pop()["id"].ToString());
			}

			FacebookSession.leaderboardUserIdsSorted = this.newLeaderboardUserIdsSorted;
		}
	}
}
