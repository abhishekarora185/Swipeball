/*
 * Author: Abhishek Arora
 * This is the helper class that defines the save data model and exposes it to the rest of the game components for use and modification
 * */

using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;


public class SaveDataHandler
{
	private static SaveData saveData;

	public static SaveData GetLoadedSaveData()
	{
		if(saveData == null)
		{
			LoadDataFromStorage();
		}
		return saveData;
	}

	public static void SetHighScore(int highScore)
	{
		saveData.highScore = highScore;
		SaveDataToStorage();
	}

	public static void SetSoundEnabled(bool soundEnabled)
	{
		saveData.soundEnabled = soundEnabled;
		SaveDataToStorage();
	}

	public static void SetSyncWithFacebook(bool syncWithFacebook)
	{
		saveData.syncWithFacebook = syncWithFacebook;
		SaveDataToStorage();
	}

	public static void SetControlMode(SwipeballConstants.ControlMode controlMode)
	{
		saveData.controlMode = controlMode;
		SaveDataToStorage();
	}

	public static void AddViewedTutorial(SwipeballConstants.Tutorial tutorial)
	{
		saveData.viewedTutorials.Add(tutorial);
		SaveDataToStorage();
	}

	public static void ClearViewedTutorials()
	{
		saveData.viewedTutorials.Clear();
		SaveDataToStorage();
	}

	private static SaveData LoadDataFromStorage()
	{
		BinaryFormatter bf = new BinaryFormatter();

		try
		{
			if (File.Exists(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName))
			{
				FileStream file = File.Open(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName, FileMode.Open);
				saveData = (SaveData)bf.Deserialize(file);
				file.Close();
			}
			else
			{
				saveData = new SaveData();
				SaveDataToStorage();
			}
		}
		catch (System.Exception e)
		{
			Debug.Log("Error occurred during deserialize: " + e);
			saveData = new SaveData();
		}
		return saveData;
	}

	public static void SaveDataToStorage()
	{
		try
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName, FileMode.OpenOrCreate);

			bf.Serialize(file, saveData);

			file.Close();
		}
		catch (System.Exception e)
		{
			Debug.Log("Error occurred during serialize: " + e);
		}
	}
}

// Making save data serializable so as to store it conveniently
[Serializable]
public class SaveData {

	public int highScore;

	public bool soundEnabled;

	[OptionalField]
	public bool syncWithFacebook;

	[OptionalField]
	public SwipeballConstants.ControlMode controlMode;

	[OptionalField]
	public List<SwipeballConstants.Tutorial> viewedTutorials;

	public SaveData()
	{
		this.InitializeMembers();
	}

	private void InitializeMembers()
	{
		this.highScore = 0;
		this.soundEnabled = false;
		this.syncWithFacebook = false;
		this.controlMode = SwipeballConstants.ControlMode.DragAndRelease;
		this.viewedTutorials = new List<SwipeballConstants.Tutorial>();
	}

	// In case new save data properties have been added, the existing save file must be deserialized safely
	[OnDeserializing]
	private void SetValuesOnDeserialize(StreamingContext context)
	{
		this.InitializeMembers();
	}
}
