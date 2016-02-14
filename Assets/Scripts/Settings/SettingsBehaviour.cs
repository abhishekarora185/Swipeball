using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using Facebook.Unity;

public class SettingsBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIOperations.SetTextProperties();
		SetButtonListeners();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void SetButtonListeners()
	{
		GameObject mainMenuButton = GameObject.Find(SwipeballConstants.GameObjectNames.Settings.MainMenu);
		mainMenuButton.GetComponent<Text>().text = SwipeballConstants.UIText.MainMenu;
		mainMenuButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				Application.LoadLevel(SwipeballConstants.LevelNames.MainMenu);
			}
		);

		GameObject soundButton = GameObject.Find(SwipeballConstants.GameObjectNames.Settings.Sound);
		if (SaveDataHandler.GetLoadedSaveData().soundEnabled)
		{
			soundButton.GetComponent<Text>().text = SwipeballConstants.UIText.Sound + SwipeballConstants.UIText.On;
		}
		else
		{
			soundButton.GetComponent<Text>().text = SwipeballConstants.UIText.Sound + SwipeballConstants.UIText.Off;
		}
		soundButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			// Toggle sound on/off and save settings to file
			if (SaveDataHandler.GetLoadedSaveData().soundEnabled)
			{
				SaveDataHandler.SetSoundEnabled(false);
				soundButton.GetComponent<Text>().text = SwipeballConstants.UIText.Sound + SwipeballConstants.UIText.Off;
			}
			else
			{
				SaveDataHandler.SetSoundEnabled(true);
				soundButton.GetComponent<Text>().text = SwipeballConstants.UIText.Sound + SwipeballConstants.UIText.On;
			}
		});

		GameObject syncWithFacebookButton = GameObject.Find(SwipeballConstants.GameObjectNames.Settings.SyncWithFacebook);
		if (SaveDataHandler.GetLoadedSaveData().syncWithFacebook)
		{
			syncWithFacebookButton.GetComponent<Text>().text = SwipeballConstants.UIText.SyncWithFacebook + SwipeballConstants.UIText.On;
		}
		else
		{
			syncWithFacebookButton.GetComponent<Text>().text = SwipeballConstants.UIText.SyncWithFacebook + SwipeballConstants.UIText.Off;
		}
		syncWithFacebookButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			// Toggle sync with facebook on/off and save settings to file, while logging in/out if necessary
			if (SaveDataHandler.GetLoadedSaveData().syncWithFacebook)
			{
				SaveDataHandler.SetSyncWithFacebook(false);
				syncWithFacebookButton.GetComponent<Text>().text = SwipeballConstants.UIText.SyncWithFacebook + SwipeballConstants.UIText.Off;
				FB.LogOut();
			}
			else
			{
				SaveDataHandler.SetSyncWithFacebook(true);
				syncWithFacebookButton.GetComponent<Text>().text = SwipeballConstants.UIText.SyncWithFacebook + SwipeballConstants.UIText.On;
				if(!FB.IsInitialized)
				{
					FacebookSession.InitializeFacebook();
				}
				else if(!FB.IsLoggedIn)
				{
					FacebookSession.ConnectToFacebookWithReadPermissions();
				}
			}
		});
	}
}
