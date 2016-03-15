using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TutorialBehaviour : MonoBehaviour {

	// Stores the Tutorials to be played, in order
	public Queue<SwipeballConstants.Tutorial> tutorialPlayQueue;

	// If a tutorial is playing now or not
	public bool isTutorialPlaying;
	// The tutorial playing now
	private SwipeballConstants.Tutorial tutorialObject;

	// The universal Tutorial UI
	private GameObject tutorialText;

	void Start () {
		this.InitializeTutorialElements();
	}

	void Update() {
		if (this.tutorialPlayQueue.Count > 0 && !this.isTutorialPlaying)
		{
			this.tutorialObject = this.tutorialPlayQueue.Dequeue();
			this.PlayTutorial();
		}

		if (this.isTutorialPlaying && Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.tutorialText.GetComponent<Text>().text = string.Empty;

			if (SwipeballConstants.GameObjectNames.Game.PrimaryGameObjectNameForTutorial.ContainsKey(this.tutorialObject))
			{
				GameObject primaryGameObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.PrimaryGameObjectNameForTutorial[this.tutorialObject]);

				if (primaryGameObject != null && primaryGameObject.GetComponent<Light>() != null)
				{
					primaryGameObject.GetComponent<Light>().range /= SwipeballConstants.Effects.TutorialLightRangeMagnify;
				}
			}

			// Add the viewed tutorial to the list of viewed tutorials in save data
			SaveDataHandler.AddViewedTutorial(this.tutorialObject);

			this.isTutorialPlaying = false;

			Time.timeScale = 1;
		}
	}

	private void InitializeTutorialElements()
	{
		this.tutorialPlayQueue = new Queue<SwipeballConstants.Tutorial>();

		this.tutorialText = GameObject.Find(SwipeballConstants.GameObjectNames.Game.TutorialText);
		this.tutorialText.GetComponent<Text>().text = string.Empty;

		this.isTutorialPlaying = false;
	}

	private void PlayTutorial()
	{
		if (SaveDataHandler.GetLoadedSaveData().viewedTutorials.Contains(this.tutorialObject))
		{
			// Don't play the tutorial again
		}
		else
		{
			this.isTutorialPlaying = true;
		
			Time.timeScale = 0;

			if (SwipeballConstants.UIText.TutorialText.ContainsKey(this.tutorialObject))
			{
				this.tutorialText.GetComponent<Text>().text = SwipeballConstants.UIText.TutorialText[this.tutorialObject];
			}

			// Light up the relevant game object
			if (SwipeballConstants.GameObjectNames.Game.PrimaryGameObjectNameForTutorial.ContainsKey(this.tutorialObject))
			{
				GameObject primaryGameObject = GameObject.Find(SwipeballConstants.GameObjectNames.Game.PrimaryGameObjectNameForTutorial[this.tutorialObject]);

				if (primaryGameObject != null && primaryGameObject.GetComponent<Light>() != null)
				{
					primaryGameObject.GetComponent<Light>().range *= SwipeballConstants.Effects.TutorialLightRangeMagnify;
				}
			}
		}
	}

}
