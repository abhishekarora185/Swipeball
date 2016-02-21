using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Facebook.Unity;

public class MainMenuBehaviour : MonoBehaviour {

	// Vary the size of the profile picture depending on the scale of the UI
	public float profilePictureSize;

	// Render the cleaver with minimum required components at the center of the main menu
	public GameObject cleaverDefinition;

	private GameObject cleaver;

	// The value of scale that needs to be applied to the decorative cleaver for the current screen size
	private float objectScalingFactor;

	// Use this for initialization
	void Start () {
		this.objectScalingFactor = Screen.height / SwipeballConstants.Scaling.MenuHeightForOriginalSize;
		// A square shape must be maintained for this object in the Unity Editor
		this.profilePictureSize = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.ProfilePicture).GetComponent<Image>().rectTransform.rect.width;

		UIOperations.SetTextProperties();
		GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Leaderboard).GetComponent<Button>().enabled = false;
		GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Leaderboard).GetComponent<Text>().enabled = false;
		TryFacebookLogin();
		AddText();
		AddProfilePicture();
		TrySyncHighScore();
		AddCleaverDecoration();
		SetButtonListeners();
	}

	private void TryFacebookLogin()
	{
		if (SaveDataHandler.GetLoadedSaveData().syncWithFacebook)
		{
			if (!FB.IsInitialized)
			{
				FacebookSession.InitializeFacebook();
			}
			else if (!FB.IsLoggedIn)
			{
				FacebookSession.ConnectToFacebookWithReadPermissions();
			}
		}
	}

	private void AddText()
	{
		GameObject highScoreText = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.HighScore);
		highScoreText.GetComponent<Text>().text = SwipeballConstants.UIText.HighScore + SaveDataHandler.GetLoadedSaveData().highScore;

		GameObject greetingText = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Greeting);
		greetingText.GetComponent<Text>().text = SwipeballConstants.UIText.OfflineName;
		
		// Put the user's name in the greeting
		if(FB.IsInitialized && FB.IsLoggedIn)
		{
			if (FacebookSession.user != null && FacebookSession.user.ContainsKey("name"))
			{
				greetingText.GetComponent<Text>().text = FacebookSession.user["name"].ToString();
			}
			else
			{
				FacebookSession.GetUsername();
			}
		}
	}

	private void AddProfilePicture()
	{
		// This will be shown if the user successfully logs in to Facebook
		GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.ProfilePicture).GetComponent<Image>().enabled = false;
		if (FB.IsInitialized && FB.IsLoggedIn && (FacebookSession.user == null || !FacebookSession.user.ContainsKey("picture")))
		{
			FacebookSession.GetProfilePicture(SwipeballConstants.FacebookConstants.LoggedInUserId, this.profilePictureSize);
		}
		else if(FacebookSession.user != null && FacebookSession.user.ContainsKey("picture"))
		{
			Image profilePicture = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.ProfilePicture).GetComponent<Image>();
			profilePicture.enabled = true;
			profilePicture.sprite = Sprite.Create((Texture2D)FacebookSession.user["picture"], new Rect(0, 0, profilePictureSize, profilePictureSize), new Vector2());
		}
	}

	private void TrySyncHighScore()
	{
		if (FB.IsInitialized && FB.IsLoggedIn)
		{
			FacebookSession.GetHighScore();
		}
	}

	private void AddCleaverDecoration()
	{
		Vector3 cleaverPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
		cleaverPosition.z = 0.0f;

		cleaverDefinition.transform.localScale = new Vector3(this.objectScalingFactor/SwipeballConstants.Scaling.CleaverScaleMultiplier, this.objectScalingFactor/SwipeballConstants.Scaling.CleaverScaleMultiplier, 0);
		cleaverDefinition.GetComponent<Light>().range = this.objectScalingFactor * SwipeballConstants.Scaling.CleaverScaleMultiplier / 2;
		cleaverDefinition.GetComponent<Light>().intensity = this.objectScalingFactor * SwipeballConstants.Scaling.CleaverScaleMultiplier / 2;
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

		GameObject settingsButton = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Settings);
		settingsButton.GetComponent<Text>().text = SwipeballConstants.UIText.Settings;
		settingsButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			Application.LoadLevel(SwipeballConstants.LevelNames.Settings);
		});

		GameObject playButton = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Play);
		playButton.GetComponent<Button>().onClick.AddListener(() => { 
			// Load the game after an encouraging animation
			StartCoroutine(SwipeballAnimation.PlayGameStartAnimation(this.cleaver, SaveDataHandler.GetLoadedSaveData().soundEnabled));
		} );

	}

	public void PrintSyncedMessage()
	{
		StartCoroutine(SwipeballAnimation.PrintSyncedMessage());
	}

	public void EnableLeaderboard()
	{
		GameObject leaderboardButton = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Leaderboard);
		leaderboardButton.GetComponent<Button>().enabled = true;
		leaderboardButton.GetComponent<Text>().enabled = true;
		leaderboardButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			Application.LoadLevel(SwipeballConstants.LevelNames.Leaderboard);
		} );
	}

	public void StartGame()
	{
		Application.LoadLevel(SwipeballConstants.LevelNames.Game);
	}

}
