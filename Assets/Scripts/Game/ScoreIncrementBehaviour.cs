using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreIncrementBehaviour : MonoBehaviour {


	// Use this for initialization
	void Start () {
		// Destroy the displayed score after a given amount of time
		GameObject.Destroy(this.gameObject, SwipeballConstants.ScoreIncrements.GameObjectAliveFrames);

		// Fade out the game object
		this.gameObject.GetComponent<Text>().CrossFadeAlpha(0.0f, SwipeballConstants.ScoreIncrements.GameObjectAliveFrames, false);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
