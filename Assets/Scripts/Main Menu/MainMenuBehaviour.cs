using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour {

	// Render the cleaver with minimum required components at the center of the main menu
	public GameObject cleaverDefinition;

	private GameObject cleaver;

	private int highScore;

	// The value of scale that needs to be applied to the decorative cleaver for the current screen size
	private float objectScalingFactor;

	// Use this for initialization
	void Start () {
		this.highScore = Scorekeeping.LoadHighScore();
		this.objectScalingFactor = Screen.height / SwipeballConstants.Scaling.MenuHeightForOriginalSize;

		UIOperations.SetTextProperties();
		AddText();
		AddCleaverDecoration();
		SetButtonListeners();
	}

	private void AddText()
	{
		GameObject highScoreText = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.HighScore);
		highScoreText.GetComponent<Text>().text = SwipeballConstants.UIText.HighScore + this.highScore + string.Empty;
	}

	private void AddCleaverDecoration()
	{
		Vector3 cleaverPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
		cleaverPosition.z = 0.0f;

		cleaverDefinition.transform.localScale = new Vector3(this.objectScalingFactor, this.objectScalingFactor, 0);
		cleaverDefinition.GetComponent<Light>().range = this.objectScalingFactor;
		cleaverDefinition.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.NoPower;

		// Render the cleaver
		this.cleaver = (GameObject)Instantiate(cleaverDefinition, cleaverPosition, Quaternion.identity);

		// Some rudimentary decorative animation
		cleaver.GetComponent<Rigidbody2D>().angularVelocity = 5;
	}

	private void SetButtonListeners()
	{
		GameObject instructionsButton = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Instructions);
		instructionsButton.GetComponent<Text>().text = SwipeballConstants.UIText.Instructions;
		instructionsButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			Application.LoadLevel(SwipeballConstants.LevelNames.Instructions);
		});

		GameObject creditsButton = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Credits);
		creditsButton.GetComponent<Text>().text = SwipeballConstants.UIText.Credits;
		creditsButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			Application.LoadLevel(SwipeballConstants.LevelNames.Credits);
		});

		GameObject playButton = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Play);
		playButton.GetComponent<Button>().onClick.AddListener(() => { 
			// Load the game after an encouraging animation
			StartCoroutine(SwipeballAnimation.PlayGameStartAnimation(this.cleaver));
		} );
	}

	public void StartGame()
	{
		Application.LoadLevel(SwipeballConstants.LevelNames.Game);
	}
}
