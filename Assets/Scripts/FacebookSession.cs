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

	public static void ClearCache()
	{
		user = null;
		userFriends = null;
		leaderboardUserIdsSorted = null;
	}

	public static bool IsLeaderboardReady()
	{
		bool isReady = true;

		if (user != null && leaderboardUserIdsSorted != null && userFriends != null)
		{
			foreach (string userId in leaderboardUserIdsSorted)
			{
				if (!user.ContainsKey("id") || !user.ContainsKey("name") || !userFriends.ContainsKey(userId)
					|| userFriends[userId] == null
					|| (!userFriends[userId].ContainsKey("name") || !userFriends[userId].ContainsKey("score") || !userFriends[userId].ContainsKey("picture")))
				{
					isReady = false;
					break;
				}
			}
		}
		else
		{
			isReady = false;
		}

		return isReady;
	}

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
			GetProfilePicture(SwipeballConstants.FacebookConstants.LoggedInUserId);
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
			// Use only the first name to avoid overflow
			user["name"] = (fetchedUser["name"].ToString().IndexOf(" ") > -1)? fetchedUser["name"].ToString().Substring(0,fetchedUser["name"].ToString().IndexOf(" ")):fetchedUser["name"];
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

	public static void GetProfilePicture(string userId)
	{
		if (FB.IsLoggedIn)
		{
			// Use an inline callback as the userId context is required
			FB.API(userId + "/picture?type=square&height=" + SwipeballConstants.Effects.ProfilePictureSize + "&width=" + SwipeballConstants.Effects.ProfilePictureSize,
				HttpMethod.GET, 
				(IGraphResult result) => {
					if (result.Error == null)
					{
						if (userId == SwipeballConstants.FacebookConstants.LoggedInUserId)
						{
							// Profile picture of the logged in user
							user["picture"] = result.Texture;
							if (Application.loadedLevelName == SwipeballConstants.LevelNames.MainMenu)
							{
								Image profilePicture = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.ProfilePicture).GetComponent<Image>();
								profilePicture.enabled = true;
								profilePicture.sprite = Sprite.Create(result.Texture, new Rect(0, 0, SwipeballConstants.Effects.ProfilePictureSize, SwipeballConstants.Effects.ProfilePictureSize), new Vector2());
							}
						}
						else if (userId != string.Empty)
						{
							if (userFriends[userId] == null)
							{
								userFriends[userId] = new Dictionary<string, object>();
							}
							userFriends[userId]["picture"] = result.Texture;
						}
					}
					else
					{
						Debug.Log(result.Error);
					}
				});
		}
		else
		{
			Debug.Log("Could not retrieve Facebook profile picture because the user is not logged in.");
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

			FacebookSession.GetFriendScores();
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
			leaderboardUserIdsSorted = new List<string>();

			if (scores != null)
			{
				foreach (Dictionary<string, object> userScore in scores)
				{
					var thisUser = (Dictionary<string, object>)userScore["user"];
					string username = (thisUser["name"].ToString().IndexOf(" ") > -1) ? thisUser["name"].ToString().Substring(0, thisUser["name"].ToString().IndexOf(" ")) : thisUser["name"].ToString();
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

					GetProfilePicture(userid);

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
			GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<MainMenuBehaviour>().EnableLeaderboard();

			FacebookSession.GetFriendScores();
		}
	}

}
