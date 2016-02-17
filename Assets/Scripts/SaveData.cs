using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


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

	private static SaveData LoadDataFromStorage()
	{
		BinaryFormatter bf = new BinaryFormatter();

		saveData = new SaveData();
		saveData.highScore = 0;
		saveData.soundEnabled = false;
		saveData.syncWithFacebook = false;

		try
		{
			if (File.Exists(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName))
			{
				FileStream file = File.Open(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName, FileMode.Open);
				saveData = (SaveData)bf.Deserialize(file);
				file.Close();
			}
		}
		catch (System.Exception e)
		{
			Debug.Log(e);
		}
		return saveData;
	}

	public static void SaveDataToStorage()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + SwipeballConstants.FileSystem.AppDataFileName, FileMode.OpenOrCreate);

		bf.Serialize(file, saveData);

		file.Close();
	}
}

[System.Serializable]
public class SaveData {

	public int highScore;

	public bool soundEnabled;

	public bool syncWithFacebook;

}
