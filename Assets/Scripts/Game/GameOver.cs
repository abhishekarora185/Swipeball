/*
 * Author: Abhishek Arora
 * The helper class that creates the Game Over menu in the Game level once the player is out of lives
 * */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOver {

	public static void CreateGameOverMenu()
	{
		GameObject gameOverObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.GameOver);
		GameObject playAgainObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.PlayAgain);
		GameObject mainMenuObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.MainMenu);
		GameObject highScoreObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.HighScore);
		GameObject scoreObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Score);
		GameObject scorekeeperObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Scorekeeper);
		
		gameOverObject.GetComponent<Text>().text = SwipeballConstants.UIText.GameOver;
		gameOverObject.GetComponent<Text>().enabled = true;
		
		playAgainObject.GetComponent<Button>().enabled = true;
		playAgainObject.GetComponent<Button>().onClick.AddListener(() =>
		{
			// Restart the game when the button is pressed
			Application.LoadLevel(SwipeballConstants.LevelNames.Game);
		});
		playAgainObject.GetComponent<Text>().text = SwipeballConstants.UIText.PlayAgain;
		playAgainObject.GetComponent<Text>().enabled = true;

		mainMenuObject.GetComponent<Button>().enabled = true;
		mainMenuObject.GetComponent<Button>().onClick.AddListener(() =>
		{
			// Go to the main menu
			Application.LoadLevel(SwipeballConstants.LevelNames.MainMenu);
		});
		mainMenuObject.GetComponent<Text>().text = SwipeballConstants.UIText.MainMenu;
		mainMenuObject.GetComponent<Text>().enabled = true;

		highScoreObject.GetComponent<Text>().text = SwipeballConstants.UIText.HighScore + SaveDataHandler.GetLoadedSaveData().highScore;
		highScoreObject.GetComponent<Text>().enabled = true;
		
		scoreObject.GetComponent<Text>().text = SwipeballConstants.UIText.Score + scorekeeperObject.GetComponent<Text>().text;
		scoreObject.GetComponent<Text>().enabled = true;

		scorekeeperObject.GetComponent<Text>().enabled = false;
	}

}
