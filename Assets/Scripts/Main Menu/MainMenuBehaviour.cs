using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MainMenuBehaviour : MonoBehaviour {

	// Render the cleaver with minimum required components at the center of the main menu
	public GameObject cleaverDefinition;

	private GameObject cleaver;

	private int highScore;

	private bool soundEnabled;

	// The value of scale that needs to be applied to the decorative cleaver for the current screen size
	private float objectScalingFactor;

	// Use this for initialization
	void Start () {
		LoadHighScoreAndSoundSettings();

		this.objectScalingFactor = Screen.height / SwipeballConstants.Scaling.MenuHeightForOriginalSize;

		UIOperations.SetTextProperties();
		AddText();
		AddCleaverDecoration();
		SetButtonListeners();
	}

	private void AddText()
	{
		GameObject highScoreText = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.HighScore);
		highScoreText.GetComponent<Text>().text = SwipeballConstants.UIText.HighScore + this.highScore + string.Empty;
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

		GameObject soundButton = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Sound);
		if(this.soundEnabled)
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
			if(this.soundEnabled)
			{
				this.soundEnabled = false;
				InstructionsBehaviour.soundEnabled = false;
				soundButton.GetComponent<Text>().text = SwipeballConstants.UIText.Sound + SwipeballConstants.UIText.Off;
			}
			else
			{
				this.soundEnabled = true;
				InstructionsBehaviour.soundEnabled = true;
				soundButton.GetComponent<Text>().text = SwipeballConstants.UIText.Sound + SwipeballConstants.UIText.On;
			}
			SaveSoundSettings();
		});

		GameObject playButton = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.Play);
		playButton.GetComponent<Button>().onClick.AddListener(() => { 
			// Load the game after an encouraging animation
			StartCoroutine(SwipeballAnimation.PlayGameStartAnimation(this.cleaver, this.soundEnabled));
		} );
	}

	public void StartGame()
	{
		Application.LoadLevel(SwipeballConstants.LevelNames.Game);
	}

	public void LoadHighScoreAndSoundSettings()
	{
		this.highScore = 0;
		this.soundEnabled = false;
		InstructionsBehaviour.soundEnabled = false;

		BinaryFormatter bf = new BinaryFormatter();
		SaveData saveData = null;
		try
		{
			if (File.Exists(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName))
			{
				FileStream file = File.Open(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName, FileMode.Open);
				saveData = (SaveData)bf.Deserialize(file);
				this.highScore = saveData.highScore;
				this.soundEnabled = saveData.soundEnabled;
				InstructionsBehaviour.soundEnabled = saveData.soundEnabled;
				file.Close();
			}
		}
		catch (System.Exception e)
		{
			this.highScore = 0;
			this.soundEnabled = false;
			InstructionsBehaviour.soundEnabled = false;
		}
	}

	public void SaveSoundSettings()
	{
		// Save the sound settings to file
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName, FileMode.OpenOrCreate);

		SaveData saveData = new SaveData();
		saveData.highScore = this.highScore;
		saveData.soundEnabled = this.soundEnabled;

		bf.Serialize(file, saveData);

		file.Close();
	}
}
