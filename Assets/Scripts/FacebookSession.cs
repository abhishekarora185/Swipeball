using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using System.Threading;

public class FacebookSession {

	// The parallel thread that handles network requests to the graph API so as to not block UI activity
	private static Thread facebookSyncThread = null;
	private static bool shouldThreadDoWork;
	private static bool shouldThreadTerminate;

	// Handles Facebook sessions and data transfer, and updates the UI with Facebook-relevant data
	private static Dictionary<string, string> publishScoreDictionary;

	// Cache user/friends data so it can be accessed without having to query multiple times
	public static Dictionary<string, object> user;
	public static Dictionary<string, Dictionary<string, object>> userFriends;
	// Store user ids in order as retrieved from facebook
	public static List<string> leaderboardUserIdsSorted;

	// bool values to indicate to a consumer that certain data is available for consumption
	// It is the consumer's responsibility to set these back to false after use in case they keep checking these values
	public static bool canHideUnity = false;
	public static bool isGameShown;
	public static bool canDisplayUsername = false;
	public static bool canDisplayProfilePicture = false;
	public static bool canEnableLeaderboard = false;

	public static void InitializeOrResumeThread()
	{
		shouldThreadTerminate = false;
		shouldThreadDoWork = true;
		if (facebookSyncThread == null)
		{
			facebookSyncThread = new Thread(ThreadLoop);
			facebookSyncThread.Start();
		}
	}

	public static void ClearCache()
	{
		user = null;
		userFriends = null;
		leaderboardUserIdsSorted = null;
	}

	public static void TerminateNetworkThread()
	{
		shouldThreadTerminate = true;
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

	private static void ThreadLoop()
	{
		while (!shouldThreadTerminate && !shouldThreadDoWork) ;

		if (!shouldThreadTerminate)
		{
			if (!FB.IsInitialized)
			{
				InitializeFacebook();
			}
			else if (!FB.IsLoggedIn)
			{
				ConnectToFacebookWithReadPermissions();
			}

			shouldThreadDoWork = false;
			ThreadLoop();
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

	private static void OnHideUnity(bool isGameShownFlag)
	{
		canHideUnity = true;
		isGameShown = isGameShownFlag;
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

		GetUsername();
		GetProfilePicture(SwipeballConstants.FacebookConstants.LoggedInUserId);
		GetHighScore();
	}

	public static void GetUsername()
	{
		if (FB.IsLoggedIn)
		{
			FB.API(SwipeballConstants.FacebookConstants.LoggedInUserId + "?fields=name,id", HttpMethod.GET, GetUsernameCallback);
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

			canDisplayUsername = true;
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
							canDisplayProfilePicture = true;
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
				});
		}
	}

	public static void GetHighScore()
	{
		if (FB.IsLoggedIn)
		{
			FB.API(SwipeballConstants.FacebookConstants.LoggedInUserId + "/scores?fields=score,application", HttpMethod.GET, GetHighScoreCallback);
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

			canEnableLeaderboard = true;

			FacebookSession.GetFriendScores();
		}
	}

	public static void GetFriendScores()
	{
		if (FB.IsLoggedIn)
		{
			FB.API(FB.AppId + "/scores", HttpMethod.GET, GetFriendScoresCallback);
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
	}

	private static void ConnectWithPublishPermissionsCallback(ILoginResult result)
	{
		// Handle post-login for different levels depending on what information they will need to post

		// Post high score
		FB.API(SwipeballConstants.FacebookConstants.LoggedInUserId + "/scores", HttpMethod.POST, ScorePublishCallback, publishScoreDictionary);
	}

	private static void ScorePublishCallback(IGraphResult result)
	{
		canEnableLeaderboard = true;
		FacebookSession.GetFriendScores();
	}

}
