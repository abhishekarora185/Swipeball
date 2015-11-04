using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour {

	private int highScore;
	
	// The value of scale that needs to be applied to the decorative cleaver for the current screen size
	private float objectScalingFactor;

	// Render the cleaver with minimum required components at the center of the main menu
	public GameObject cleaverDefinition;

	private GameObject cleaver;

	// Use this for initialization
	void Start () {
		this.highScore = Scorekeeping.LoadHighScore();
		this.objectScalingFactor = Screen.height / SwipeballConstants.Scaling.MenuHeightForOriginalSize;

		ScaleText();
		AddText();
		AddCleaverDecoration();
		SetButtonListeners();
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

	private void AddText()
	{
		GameObject highScoreText = GameObject.Find(SwipeballConstants.EntityNames.HighScore);
		highScoreText.GetComponent<Text>().text = SwipeballConstants.MenuText.HighScore + this.highScore + string.Empty;
	}

	private void AddCleaverDecoration()
	{
		Vector3 cleaverPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
		cleaverPosition.z = 0.0f;

		cleaverDefinition.transform.localScale = new Vector3(this.objectScalingFactor, this.objectScalingFactor, 0);
		cleaverDefinition.GetComponent<Light>().range = this.objectScalingFactor;

		// Render the cleaver
		this.cleaver = (GameObject)Instantiate(cleaverDefinition, cleaverPosition, Quaternion.identity);

		// Some rudimentary decorative animation
		cleaver.GetComponent<Rigidbody2D>().angularVelocity = 5;
	}

	private void SetButtonListeners()
	{
		GameObject instructionsButton = GameObject.Find(SwipeballConstants.EntityNames.Instructions);
		instructionsButton.GetComponent<Text>().text = SwipeballConstants.MenuText.Instructions;
		instructionsButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			Application.LoadLevel(SwipeballConstants.LevelNames.Instructions);
		});

		GameObject creditsButton = GameObject.Find(SwipeballConstants.EntityNames.Credits);
		creditsButton.GetComponent<Text>().text = SwipeballConstants.MenuText.Credits;
		creditsButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			//Application.LoadLevel(SwipeballConstants.LevelNames.Credits);
		});

		GameObject playButton = GameObject.Find(SwipeballConstants.EntityNames.Play);
		playButton.GetComponent<Button>().onClick.AddListener(() => { 
			// Load the game
			StartCoroutine(AnimationBehaviour.PlayGameStartAnimation(this.cleaver));
		} );
	}
}
