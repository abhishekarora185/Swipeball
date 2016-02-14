using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine.UI;

public class FacebookSession {

	// Handles Facebook sessions and data transfer, and updates the UI with Facebook-relevant data

	public static string username = "";

	public static int highScore = 0;

	private static Dictionary<string, string> publishScoreDictionary;

	public static void InitializeFacebook()
	{
		if (!FB.IsInitialized)
		{
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		}
		else
		{
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
			ConnectToFacebookWithReadPermissions();
		}
	}

	private static void InitCallback()
	{
		if (FB.IsInitialized)
		{
			// Signal an app activation App Event
			FB.ActivateApp();
			ConnectToFacebookWithReadPermissions();
		}
		else
		{
			// Print a nice sorry message for the retard who tried to connect without internet
		}
	}

	private static void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
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

	public static void ConnectToFacebookWithReadPermissions()
	{
		var perms = new System.Collections.Generic.List<string>() { "public_profile", "user_friends", "user_games_activity" };
		FB.LogInWithReadPermissions(perms, ConnectWithReadPermissionsCallback);
	}

	public static void ConnectToFacebookWithPublishPermissions()
	{
		var perms = new System.Collections.Generic.List<string>() { "publish_actions" };
		FB.LogInWithPublishPermissions(perms, ConnectWithPublishPermissionsCallback);
	}

	private static void ConnectWithReadPermissionsCallback(ILoginResult result)
	{
		// Handle post-login for different levels depending on what information they will need

		if(Application.loadedLevelName == SwipeballConstants.LevelNames.MainMenu)
		{
			GetUsername();
			GetProfilePicture();
			GetHighScore();
		}
	}

	private static void ConnectWithPublishPermissionsCallback(ILoginResult result)
	{
		// Handle post-login for different levels depending on what information they will need to post

		if (Application.loadedLevelName == SwipeballConstants.LevelNames.MainMenu)
		{
			// Post high score
			FB.API("me/scores", HttpMethod.POST, ScorePublishCallback, publishScoreDictionary);
		}
	}

	public static void GetUsername()
	{
		if (FB.IsLoggedIn)
		{
			FB.API("me?fields=name", HttpMethod.GET, GetUsernameCallback);
		}
		else
		{
			Debug.Log("Could not retrieve Facebook username because the user is not logged in.");
		}
	}

	private static void GetUsernameCallback(IGraphResult result)
	{
		if (result.Error == null)
		{
			IDictionary user = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
			username = user["name"].ToString();

			if (Application.loadedLevelName == SwipeballConstants.LevelNames.MainMenu)
			{
				GameObject greetingText = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Greeting);
				greetingText.GetComponent<Text>().text = username + " ";
			}
		}
		else
		{
			Debug.Log(result.Error);
		}
	}

	public static void GetProfilePicture()
	{
		if (FB.IsLoggedIn)
		{
			float profilePictureSize = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<MainMenuBehaviour>().profilePictureSize;
			FB.API("me/picture?type=square&height=" + profilePictureSize + "&width=" + profilePictureSize, HttpMethod.GET, GetProfilePictureCallback);
		}
		else
		{
			Debug.Log("Could not retrieve Facebook profile picture because the user is not logged in.");
		}
	}

	private static void GetProfilePictureCallback(IGraphResult result)
	{
		if (result.Error == null)
		{
			if (Application.loadedLevelName == SwipeballConstants.LevelNames.MainMenu)
			{
				float profilePictureSize = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<MainMenuBehaviour>().profilePictureSize;

				Image profilePicture = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.ProfilePicture).GetComponent<Image>();
				profilePicture.enabled = true;
				profilePicture.sprite = Sprite.Create(result.Texture, new Rect(0, 0, profilePictureSize, profilePictureSize), new Vector2());
			}
		}
		else
		{
			Debug.Log(result.Error);
		}
	}

	public static void GetHighScore()
	{
		if (FB.IsLoggedIn)
		{
			FB.API("me/scores?fields=score,application", HttpMethod.GET, GetHighScoreCallback);
		}
		else
		{
			Debug.Log("Could not retrieve Facebook score because the user is not logged in.");
		}
	}

	private static void GetHighScoreCallback(IGraphResult result)
	{
		IDictionary data = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;

		IList scores = (IList)data["data"];
		IDictionary thisApp = null;

		if (scores != null)
		{
			foreach (IDictionary app in scores)
			{
				// Find the score associated with this app, if any

				if (app["application"] != null && (string)(((IDictionary)app["application"])["id"]) == FB.AppId)
				{
					thisApp = app;
					break;
				}
			}
		}

		var thisAppScore = System.Int32.Parse((string)thisApp["score"]);

		if (thisApp == null || SaveDataHandler.GetLoadedSaveData().highScore > thisAppScore)
		{
			// Push the new high score to Facebook
			publishScoreDictionary = new Dictionary<string, string>() { { "score", SaveDataHandler.GetLoadedSaveData().highScore + System.String.Empty } };

			ConnectToFacebookWithPublishPermissions();

		}
		else if (thisAppScore > SaveDataHandler.GetLoadedSaveData().highScore)
		{
			// Save the remote high score locally
			SaveDataHandler.SetHighScore(thisAppScore);
		}
	}

	private static void ScorePublishCallback(IGraphResult result)
	{
		Debug.Log("Score post successful!");
	}
	
}
