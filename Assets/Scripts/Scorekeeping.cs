using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scorekeeping : MonoBehaviour {

    private static int score;

    // The number of frames after which the score is increased by 1
    private int scoreThreshold;

    // Counts frames upto the scoreThreshold
    private int scoreCounter;

	// Use this for initialization
	void Start () {
        // Place the scorekeeper at the bottom right corner of the screen
        Vector3 scorePosition = Camera.main.ViewportToScreenPoint(new Vector3(1.0f, 0.0f));
        scorePosition.z = 0.0f;
        this.gameObject.GetComponent<Text>().transform.position = scorePosition;

        score = 0;
        this.scoreThreshold = 500;
        this.scoreCounter = 0;
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePersistentScore();
	}

    // Constantly increase the score by 1 after a certain period as a survival reward
    private void UpdatePersistentScore()
    {
        this.scoreCounter = (this.scoreCounter + 1) % this.scoreThreshold;

        if(!BallBehaviour.isDead && this.scoreCounter == 0)
        {
            score++;
        }

        this.gameObject.GetComponent<Text>().text = score + string.Empty;
    }

    // Score increases triggered by other agents
    public static void IncreaseScore(int increasedScore)
    {
        // TODO: Play some awesome animation

        score += increasedScore;
    }
}
