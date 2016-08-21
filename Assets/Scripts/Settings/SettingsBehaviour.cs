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

	void OnApplicationQuit()
	{
		FacebookSession.TerminateNetworkThread();
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
				FacebookSession.ClearCache();
				FacebookSession.TerminateNetworkThread();
			}
			else
			{
				SaveDataHandler.SetSyncWithFacebook(true);
				syncWithFacebookButton.GetComponent<Text>().text = SwipeballConstants.UIText.SyncWithFacebook + SwipeballConstants.UIText.On;
				FacebookSession.InitializeOrResumeThread();
			}
		});

		GameObject controlModeButton = GameObject.Find(SwipeballConstants.GameObjectNames.Settings.ControlMode);
		controlModeButton.GetComponent<Text>().text = SwipeballConstants.UIText.ControlModes + SwipeballConstants.UIText.ControlModeDisplayName[SaveDataHandler.GetLoadedSaveData().controlMode];
		controlModeButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			// Toggle control mode and save settings to file
			if (SaveDataHandler.GetLoadedSaveData().controlMode == SwipeballConstants.ControlMode.DragAndRelease)
			{
				SaveDataHandler.SetControlMode(SwipeballConstants.ControlMode.FollowSwipe);
			}
			else if (SaveDataHandler.GetLoadedSaveData().controlMode == SwipeballConstants.ControlMode.FollowSwipe)
			{
				SaveDataHandler.SetControlMode(SwipeballConstants.ControlMode.DragAndRelease);
			}
			controlModeButton.GetComponent<Text>().text = SwipeballConstants.UIText.ControlModes + SwipeballConstants.UIText.ControlModeDisplayName[SaveDataHandler.GetLoadedSaveData().controlMode];
		});

		GameObject resetTutorialsButton = GameObject.Find(SwipeballConstants.GameObjectNames.Settings.ResetTutorials);
		resetTutorialsButton.SetActive(false);
		if (SaveDataHandler.GetLoadedSaveData().viewedTutorials.Count > 0)
		{
			resetTutorialsButton.GetComponent<Text>().text = SwipeballConstants.UIText.ResetTutorials;
			resetTutorialsButton.GetComponent<Button>().onClick.AddListener(() => {
				SaveDataHandler.ClearViewedTutorials();
				resetTutorialsButton.SetActive(false);
			});
			resetTutorialsButton.SetActive(true);
		}
	}
}
