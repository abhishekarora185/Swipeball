using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine.UI;

public class FacebookSession {

	// Handles Facebook sessions and data transfer, and updates the UI with Facebook-relevant data
	private static Dictionary<string, string> publishScoreDictionary;

	// Cache user/friends data so it can be accessed without having to query multiple times
	public static Dictionary<string, object> user;
	public static Dictionary<string, Dictionary<string, object>> userFriends;
	// Store user ids in order as retrieved from facebook
	public static List<string> leaderboardUserIdsSorted;

	// The user id of the last fetched profile picture
	// Global variable needed for caching the Texture since callback for profile picture retrieval does not have userid in context
	private static string lastRetrievedProfilePicture = string.Empty;

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
			float profilePictureSize = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<MainMenuBehaviour>().profilePictureSize;
			GetProfilePicture(SwipeballConstants.FacebookConstants.LoggedInUserId, profilePictureSize);
			GetHighScore();
		}
	}

	public static void GetUsername()
	{
		if (FB.IsLoggedIn)
		{
			FB.API(SwipeballConstants.FacebookConstants.LoggedInUserId + "?fields=name,id", HttpMethod.GET, GetUsernameCallback);
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
			if(user == null)
			{
				user = new Dictionary<string, object>();
			}

			Dictionary<string, object> fetchedUser = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;
			user["name"] = fetchedUser["name"];
			user["id"] = fetchedUser["id"];

			if (Application.loadedLevelName == SwipeballConstants.LevelNames.MainMenu)
			{
				GameObject greetingText = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Greeting);
				greetingText.GetComponent<Text>().text = user["name"] + " ";
			}
		}
		else
		{
			Debug.Log(result.Error);
		}
	}

	public static void GetProfilePicture(string userId, float profilePictureSize)
	{
		if (FB.IsLoggedIn)
		{
			lastRetrievedProfilePicture = userId;
			FB.API(userId + "/picture?type=square&height=" + profilePictureSize + "&width=" + profilePictureSize, HttpMethod.GET, GetProfilePictureCallback);
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
			if (lastRetrievedProfilePicture == SwipeballConstants.FacebookConstants.LoggedInUserId)
			{
				// Profile picture of the logged in user
				user["picture"] = result.Texture;
			}
			else if(lastRetrievedProfilePicture != string.Empty)
			{
				// Profile picture of a friend of the logged in user
				if(userFriends[lastRetrievedProfilePicture] == null)
				{
					userFriends[lastRetrievedProfilePicture] = new Dictionary<string, object>();
				}
				userFriends[lastRetrievedProfilePicture]["picture"] = result.Texture;
			}
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
			FB.API(SwipeballConstants.FacebookConstants.LoggedInUserId + "/scores?fields=score,application", HttpMethod.GET, GetHighScoreCallback);
		}
		else
		{
			Debug.Log("Could not retrieve Facebook score because the user is not logged in.");
		}
	}

	private static void GetHighScoreCallback(IGraphResult result)
	{
		var data = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;

		var scores = (List<object>)data["data"];
		Dictionary<string, object> thisApp = null;

		if (scores != null)
		{
			foreach (Dictionary<string, object> app in scores)
			{
				// Find the score associated with this app, if any
				if (app["application"] != null && (string)(((Dictionary<string, object>)app["application"])["id"]) == FB.AppId)
				{
					thisApp = app;
					break;
				}
			}
		}

		// Safe Parsing!
		int thisAppScore = 0;
		bool scoreParsed = false;

		if (thisApp != null && thisApp.ContainsKey("score"))
		{
			string scoreString = thisApp["score"].ToString();
			scoreParsed = System.Int32.TryParse(scoreString, out thisAppScore);
		}

		if (thisApp == null || (scoreParsed && SaveDataHandler.GetLoadedSaveData().highScore > thisAppScore))
		{
			// Push the new high score to Facebook
			publishScoreDictionary = new Dictionary<string, string>() { { "score", SaveDataHandler.GetLoadedSaveData().highScore + System.String.Empty } };

			ConnectToFacebookWithPublishPermissions();

		}
		else if (scoreParsed && thisAppScore >= SaveDataHandler.GetLoadedSaveData().highScore)
		{
			// Save the remote high score locally and display it, if necessary
			SaveDataHandler.SetHighScore(thisAppScore);

			if(Application.loadedLevelName == SwipeballConstants.LevelNames.MainMenu)
			{
				GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.HighScore).GetComponent<Text>().text = SwipeballConstants.UIText.HighScore + SaveDataHandler.GetLoadedSaveData().highScore;
				GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<MainMenuBehaviour>().PrintSyncedMessage();
				GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<MainMenuBehaviour>().EnableLeaderboard();
			}

		}
	}

	public static void GetFriendScores()
	{
		if (FB.IsLoggedIn)
		{
			FB.API(FB.AppId + "/scores", HttpMethod.GET, GetFriendScoresCallback);
		}
		else
		{
			Debug.Log("Could not retrieve friends' scores because the user is not logged in.");
		}
	}

	private static void GetFriendScoresCallback(IGraphResult result)
	{
		if (result.Error == null)
		{
			var data = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;
			var scores = (List<object>)data["data"];

			if (scores != null)
			{
				foreach (Dictionary<string, object> userScore in scores)
				{
					var thisUser = (Dictionary<string, object>)userScore["user"];
					string username = thisUser["name"].ToString();
					string userid = thisUser["id"].ToString();
					string score = userScore["score"].ToString();

					// Cache, cache, cache!
					if (userFriends == null)
					{
						userFriends = new Dictionary<string, Dictionary<string, object>>();
					}
					if (!userFriends.ContainsKey(userid) || userFriends[userid] == null)
					{
						userFriends[userid] = new Dictionary<string, object>();
					}
					userFriends[userid]["name"] = username;
					userFriends[userid]["score"] = score;

					if(leaderboardUserIdsSorted == null)
					{
						leaderboardUserIdsSorted = new List<string>();
					}
					leaderboardUserIdsSorted.Add(userid);
				}
			}
		}
		else
		{
			Debug.Log(result.Error);
		}
	}

	private static void ConnectWithPublishPermissionsCallback(ILoginResult result)
	{
		// Handle post-login for different levels depending on what information they will need to post

		if (Application.loadedLevelName == SwipeballConstants.LevelNames.MainMenu)
		{
			// Post high score
			FB.API(SwipeballConstants.FacebookConstants.LoggedInUserId + "/scores", HttpMethod.POST, ScorePublishCallback, publishScoreDictionary);
		}
	}

	private static void ScorePublishCallback(IGraphResult result)
	{
		if(Application.loadedLevelName == SwipeballConstants.LevelNames.MainMenu)
		{
			GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<MainMenuBehaviour>().PrintSyncedMessage();
		}
	}

}
