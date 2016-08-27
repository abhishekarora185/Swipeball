/*
 * Author: Abhishek Arora
 * This is the Behaviour script attached to the primary GameObject in the MainMenu level
 * */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Facebook.Unity;

public class MainMenuBehaviour : MonoBehaviour {


	// Render the cleaver with minimum required components at the center of the main menu as a decoration
	public GameObject cleaverDefinition;

	private GameObject cleaver;

	// Indicates that the game start animation is playing
	public bool gameStarted;

	// The value of scale that needs to be applied to the decorative cleaver for the current screen size
	private float objectScalingFactor;

	// Use this for initialization
	void Start () {
		this.objectScalingFactor = Screen.height / SwipeballConstants.Scaling.MenuHeightForOriginalSize;
		this.gameStarted = false;
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

	void Update()
	{
		// Update callback only handles the results of Facebook sync callbacks, since network requests are made on another thread that doesn't use the Unity API
		if (SaveDataHandler.GetLoadedSaveData().syncWithFacebook)
		{
			if (FacebookSession.canHideUnity)
			{
				this.HideUnity();
			}
			if (FacebookSession.canDisplayUsername)
			{
				this.DisplayUsername();
			}
			if (FacebookSession.canDisplayProfilePicture)
			{
				this.DisplayProfilePicture();
			}
			if (FacebookSession.canEnableLeaderboard)
			{
				this.EnableLeaderboard();
			}
		}
	}

	void OnApplicationQuit()
	{
		FacebookSession.TerminateNetworkThread();
	}

	private void TryFacebookLogin()
	{
		if (SaveDataHandler.GetLoadedSaveData().syncWithFacebook)
		{
			FacebookSession.InitializeOrResumeThread();
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
		if (FB.IsInitialized && FB.IsLoggedIn)
		{
			if (FacebookSession.user != null && FacebookSession.user.ContainsKey("picture"))
			{
				Image profilePicture = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.ProfilePicture).GetComponent<Image>();
				profilePicture.enabled = true;
				profilePicture.sprite = Sprite.Create((Texture2D)FacebookSession.user["picture"], new Rect(0, 0, SwipeballConstants.Effects.ProfilePictureSize, SwipeballConstants.Effects.ProfilePictureSize), new Vector2());
			}
			else
			{
				FacebookSession.GetProfilePicture(SwipeballConstants.FacebookConstants.LoggedInUserId);
			}
		}
	}

	// Whenever the main menu is loaded, the high score is synced (if the user is logged in to Facebook, of course)
	private void TrySyncHighScore()
	{
		if (SaveDataHandler.GetLoadedSaveData().syncWithFacebook && FB.IsInitialized && FB.IsLoggedIn)
		{
			FacebookSession.SyncHighScore();
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
			this.gameStarted = true;
			StartCoroutine(SwipeballAnimation.PlayGameStartAnimation(this.cleaver));
		} );

	}

	// Displays the word "Synced" on screen after(if) the high score is synced with Facebook
	public void PrintSyncedMessage()
	{
		StartCoroutine(SwipeballAnimation.PrintSyncedMessage());
	}

	// Player is ready!
	public void StartGame()
	{
		Application.LoadLevel(SwipeballConstants.LevelNames.Game);
	}

	// Once Facebook data has been successfully obtained, we can allow the user to open the leaderboard
	private void EnableLeaderboard()
	{
		FacebookSession.canEnableLeaderboard = false;

		GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.HighScore).GetComponent<Text>().text = SwipeballConstants.UIText.HighScore + SaveDataHandler.GetLoadedSaveData().highScore;
		this.PrintSyncedMessage();

		GameObject leaderboardButton = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Leaderboard);

		// No point rendering if the user has already pressed Play
		if (!this.gameStarted)
		{
			leaderboardButton.GetComponent<Button>().enabled = true;
			leaderboardButton.GetComponent<Text>().enabled = true;
			leaderboardButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				Application.LoadLevel(SwipeballConstants.LevelNames.Leaderboard);
			});
		}
	}

	// Pauses the game when Facebook authentication is in progress
	private void HideUnity()
	{
		FacebookSession.canHideUnity = false;
		if (!FacebookSession.isGameShown)
		{
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		}
		else
		{
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	// These two methods display information once the relevant Facebook data is obtained

	private void DisplayUsername()
	{
		FacebookSession.canDisplayUsername = false;
		GameObject greetingText = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Greeting);
		greetingText.GetComponent<Text>().text = FacebookSession.user["name"] + " ";
	}

	private void DisplayProfilePicture()
	{
		FacebookSession.canDisplayProfilePicture = false;

		Image profilePicture = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.ProfilePicture).GetComponent<Image>();
		profilePicture.enabled = true;
		profilePicture.sprite = Sprite.Create((Texture2D)FacebookSession.user["picture"], new Rect(0, 0, SwipeballConstants.Effects.ProfilePictureSize, SwipeballConstants.Effects.ProfilePictureSize), new Vector2());
	}

}
