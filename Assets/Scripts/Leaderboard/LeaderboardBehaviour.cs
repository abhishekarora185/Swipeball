using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardBehaviour : MonoBehaviour {

	// List item prefab for the list
	public GameObject listItemDefinition;

	private bool leaderboardLoaded = false;

	// Use this for initialization
	void Start () {
		UIOperations.SetTextProperties();
		SetButtonListeners();
	}
	
	// Update is called once per frame
	void Update () {
		if(!this.leaderboardLoaded && FacebookSession.IsLeaderboardReady())
		{
			this.leaderboardLoaded = true;
			PopulateList();
		}
	}

	private void SetButtonListeners()
	{
		GameObject mainMenuButton = GameObject.Find(SwipeballConstants.GameObjectNames.Leaderboard.MainMenu);
		mainMenuButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			Application.LoadLevel(SwipeballConstants.LevelNames.MainMenu);
		});
	}

	public void PopulateList()
	{
		GameObject userList = GameObject.Find(SwipeballConstants.GameObjectNames.Leaderboard.UserList);

		foreach (string userId in FacebookSession.leaderboardUserIdsSorted)
		{
			bool isLoggedInUser = false;
			// If this list item corresponds to the logged in user, change the text color to highlight it
			if (FacebookSession.user != null && userId == FacebookSession.user["id"].ToString())
			{
				isLoggedInUser = true;
			}

			GameObject listItem = Instantiate(listItemDefinition);

			listItem.transform.SetParent(userList.transform);
			listItem.transform.localScale = Vector3.one;

			SetListItemComponents(listItem, userId, isLoggedInUser);
		}
	}

	private void SetListItemComponents(GameObject listItem, string userId, bool isLoggedInUser)
	{
		Dictionary<string, object> userData = FacebookSession.userFriends[userId];

		// Two text objects, one image object
		foreach (Text textObject in listItem.GetComponentsInChildren<Text>())
		{
			// Match names of GameObjects with their respective user details
			if (textObject.gameObject.name.Contains(SwipeballConstants.GameObjectNames.Leaderboard.Name) && userData.ContainsKey("name"))
			{
				textObject.text = userData["name"].ToString();
			}
			else if (textObject.gameObject.name.Contains(SwipeballConstants.GameObjectNames.Leaderboard.Score) && userData.ContainsKey("score"))
			{
				textObject.text = userData["score"].ToString();
			}

			if (isLoggedInUser)
			{
				textObject.color = Color.yellow;
			}
		}

		Texture2D profilePicture = (Texture2D) userData["picture"];

		if (profilePicture != null)
		{
			foreach (Image imageObject in listItem.GetComponentsInChildren<Image>())
			{
				// Match names of GameObjects with their respective user details
				if (imageObject.gameObject.name.Contains(SwipeballConstants.GameObjectNames.Leaderboard.Picture))
				{
					listItem.GetComponentInChildren<Image>().sprite = Sprite.Create(profilePicture, new Rect(0, 0, SwipeballConstants.Effects.ProfilePictureSize, SwipeballConstants.Effects.ProfilePictureSize), new Vector2());
				}
			}
		}
	}
}
