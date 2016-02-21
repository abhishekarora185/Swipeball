using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LeaderboardBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIOperations.SetTextProperties();
		SetButtonListeners();
		FacebookSession.GetFriendScores();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void SetButtonListeners()
	{
		GameObject mainMenuButton = GameObject.Find(SwipeballConstants.GameObjectNames.Leaderboard.MainMenu);
		mainMenuButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			Application.LoadLevel(SwipeballConstants.LevelNames.MainMenu);
		});
	}
}
