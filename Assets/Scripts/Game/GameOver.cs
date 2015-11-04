using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOver : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    public static void CreateGameOverMenu()
    {
        GameObject gameOverObject = GameObject.Find(SwipeballConstants.EntityNames.GameOver);
        GameObject playAgainObject = GameObject.Find(SwipeballConstants.EntityNames.PlayAgain);
        GameObject mainMenuObject = GameObject.Find(SwipeballConstants.EntityNames.MainMenu);
        GameObject highScoreObject = GameObject.Find(SwipeballConstants.EntityNames.HighScore);
        GameObject scoreObject = GameObject.Find(SwipeballConstants.EntityNames.Scorekeeper);

        Vector3 gameOverPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.75f));
        gameOverPosition.z = 0.0f;
        gameOverObject.GetComponent<Text>().transform.position = gameOverPosition;
        gameOverObject.GetComponent<Text>().text = SwipeballConstants.MenuText.GameOver;
        gameOverObject.GetComponent<Text>().enabled = true;

        Vector3 playAgainPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.25f, 0.25f));
        playAgainPosition.z = 0.0f;
        playAgainObject.GetComponent<Button>().transform.position = playAgainPosition;
        playAgainObject.GetComponent<Button>().enabled = true;
        playAgainObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            // Restart the game when the button is pressed
            Application.LoadLevel(SwipeballConstants.LevelNames.Game);
        });
        playAgainObject.GetComponent<Text>().text = SwipeballConstants.MenuText.PlayAgain;
        playAgainObject.GetComponent<Text>().enabled = true;

        Vector3 mainMenuPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.75f, 0.25f));
        mainMenuPosition.z = 0.0f;
        mainMenuObject.GetComponent<Button>().transform.position = mainMenuPosition;
        mainMenuObject.GetComponent<Button>().enabled = true;
        mainMenuObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            // Go to the main menu
            Application.LoadLevel(SwipeballConstants.LevelNames.MainMenu);
        });
        mainMenuObject.GetComponent<Text>().text = SwipeballConstants.MenuText.MainMenu;
        mainMenuObject.GetComponent<Text>().enabled = true;

        Vector3 highScorePosition = Camera.main.ViewportToScreenPoint(new Vector3(0.8f, 0.5f));
        highScorePosition.z = 0.0f;
        highScoreObject.GetComponent<Text>().transform.position = highScorePosition;
        highScoreObject.GetComponent<Text>().text = SwipeballConstants.MenuText.HighScore + Scorekeeping.highScore;
        highScoreObject.GetComponent<Text>().enabled = true;

        Vector3 scorePosition = Camera.main.ViewportToScreenPoint(new Vector3(0.2f, 0.5f));
        scorePosition.z = 0.0f;
        scoreObject.GetComponent<Text>().transform.position = scorePosition;
        scoreObject.GetComponent<Text>().text = SwipeballConstants.MenuText.Score + scoreObject.GetComponent<Text>().text;
        scoreObject.GetComponent<Text>().enabled = true;
    }

}
