using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour {

    private int highScore;
    
    // The value of scale that needs to be applied to the decorative cleaver for the current screen size
    private float objectScalingFactor;

    // Render the cleaver with minimum required components at the center of the main menu
    public GameObject cleaverDefinition;

	// Use this for initialization
	void Start () {
        this.highScore = Scorekeeping.LoadHighScore();
        this.objectScalingFactor = Screen.height / SwipeballConstants.Scaling.MenuHeightForOriginalSize;

        ScaleText();
        AddTextAndButtons();
        AddCleaverDecoration();
        SetButtonListeners();
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void ScaleText()
    {
        foreach (GameObject textObject in GameObject.FindGameObjectsWithTag(SwipeballConstants.EntityNames.TextTag))
        {
            if (textObject.GetComponent<Text>() != null)
            {
                textObject.GetComponent<Text>().fontSize = (int)(textObject.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);
            }
        }
    }

    private void AddTextAndButtons()
    {
        GameObject instructionsButton = GameObject.Find(SwipeballConstants.EntityNames.Instructions);
        instructionsButton.GetComponent<Text>().text = SwipeballConstants.MenuText.Instructions;
        Vector3 instructionsButtonPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.1f, 0.9f));
        instructionsButtonPosition.z = 0.0f;
        instructionsButton.transform.position = instructionsButtonPosition;
        instructionsButton.GetComponent<Button>().onClick.AddListener(() => {
            Application.LoadLevel(SwipeballConstants.LevelNames.Instructions);
        });

        GameObject highScoreText = GameObject.Find(SwipeballConstants.EntityNames.HighScore);
        highScoreText.GetComponent<Text>().text = SwipeballConstants.MenuText.HighScore + this.highScore + string.Empty;
        Vector3 highScoreTextPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.9f, 0.9f));
        highScoreTextPosition.z = 0.0f;
        highScoreText.transform.position = highScoreTextPosition;
    }

    private void AddCleaverDecoration()
    {
        Vector3 cleaverPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
        cleaverPosition.z = 0.0f;

        cleaverDefinition.transform.localScale = new Vector3(this.objectScalingFactor, this.objectScalingFactor, 0);
        cleaverDefinition.GetComponent<Light>().range = this.objectScalingFactor;

        // Render the cleaver
        GameObject cleaver = (GameObject)Instantiate(cleaverDefinition, cleaverPosition, Quaternion.identity);

        // Some rudimentary decorative animation
        cleaver.GetComponent<Rigidbody2D>().angularVelocity = 10;
    }

    private void SetButtonListeners()
    {
        GameObject playButton = GameObject.Find(SwipeballConstants.EntityNames.Play);
        playButton.GetComponent<Button>().onClick.AddListener(() => { 
            // Load the game
            Application.LoadLevel(SwipeballConstants.LevelNames.Game);
        } );
    }
}
