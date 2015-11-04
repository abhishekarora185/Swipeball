using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOver : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        GameObject gameOverObject = GameObject.Find(SwipeballConstants.EntityNames.GameOver);
        GameObject playAgainObject = GameObject.Find(SwipeballConstants.EntityNames.PlayAgain);
        GameObject mainMenuObject = GameObject.Find(SwipeballConstants.EntityNames.MainMenu);
        GameObject highScoreObject = GameObject.Find(SwipeballConstants.EntityNames.HighScore);
        GameObject scoreObject = GameObject.Find(SwipeballConstants.EntityNames.Score);
        GameObject scorekeeperObject = GameObject.Find(SwipeballConstants.EntityNames.Scorekeeper);

        gameOverObject.GetComponent<Text>().enabled = false;
        playAgainObject.GetComponent<Text>().enabled = false;
        mainMenuObject.GetComponent<Text>().enabled = false;
        highScoreObject.GetComponent<Text>().enabled = false;
        scoreObject.GetComponent<Text>().enabled = false;
        scorekeeperObject.GetComponent<Text>().enabled = true;
    }

    public static void CreateGameOverMenu()
    {
        GameObject gameOverObject = GameObject.Find(SwipeballConstants.EntityNames.GameOver);
        GameObject playAgainObject = GameObject.Find(SwipeballConstants.EntityNames.PlayAgain);
        GameObject mainMenuObject = GameObject.Find(SwipeballConstants.EntityNames.MainMenu);
        GameObject highScoreObject = GameObject.Find(SwipeballConstants.EntityNames.HighScore);
        GameObject scoreObject = GameObject.Find(SwipeballConstants.EntityNames.Score);
        GameObject scorekeeperObject = GameObject.Find(SwipeballConstants.EntityNames.Scorekeeper);
        
        gameOverObject.GetComponent<Text>().text = SwipeballConstants.MenuText.GameOver;
        gameOverObject.GetComponent<Text>().enabled = true;
        
        playAgainObject.GetComponent<Button>().enabled = true;
        playAgainObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            // Restart the game when the button is pressed
            Application.LoadLevel(SwipeballConstants.LevelNames.Game);
        });
        playAgainObject.GetComponent<Text>().text = SwipeballConstants.MenuText.PlayAgain;
        playAgainObject.GetComponent<Text>().enabled = true;

        mainMenuObject.GetComponent<Button>().enabled = true;
        mainMenuObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            // Go to the main menu
            Application.LoadLevel(SwipeballConstants.LevelNames.MainMenu);
        });
        mainMenuObject.GetComponent<Text>().text = SwipeballConstants.MenuText.MainMenu;
        mainMenuObject.GetComponent<Text>().enabled = true;
        
        highScoreObject.GetComponent<Text>().text = SwipeballConstants.MenuText.HighScore + Scorekeeping.highScore;
        highScoreObject.GetComponent<Text>().enabled = true;
        
        scoreObject.GetComponent<Text>().text = SwipeballConstants.MenuText.Score + scorekeeperObject.GetComponent<Text>().text;
        scoreObject.GetComponent<Text>().enabled = true;

        scorekeeperObject.GetComponent<Text>().enabled = false;
    }

}
