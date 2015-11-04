using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstructionsSelection : MonoBehaviour {

    public GameObject ballDefinition;
    public GameObject cleaverDefinition;
    public GameObject mineDefinition;

    private GameObject title;
    private GameObject paragraph;
    private GameObject mainMenu;
    private GameObject ballButton;
    private GameObject cleaverButton;
    private GameObject mineButton;

	// Use this for initialization
	void Start () {
        ArrangeButtons();
        StandardSelection();
	}

    private void ArrangeButtons()
    {
        // Resize the sprites, text and buttons and place them
        this.title = GameObject.Find(SwipeballConstants.EntityNames.Title);
        this.paragraph = GameObject.Find(SwipeballConstants.EntityNames.Paragraph);
        this.mainMenu= GameObject.Find(SwipeballConstants.EntityNames.MainMenu);
        this.ballButton = GameObject.Find(SwipeballConstants.EntityNames.BallButton);
        this.cleaverButton = GameObject.Find(SwipeballConstants.EntityNames.CleaverButton);
        this.mineButton = GameObject.Find(SwipeballConstants.EntityNames.MineButton);

        Vector3 spriteAndButtonScale = new Vector3(Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize, Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize, 1.0f);

        this.ballDefinition.transform.localScale = spriteAndButtonScale;
        this.cleaverDefinition.transform.localScale = spriteAndButtonScale;
        this.mineDefinition.transform.localScale = spriteAndButtonScale;

        this.ballDefinition.GetComponent<Light>().range = Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize;
        this.cleaverDefinition.GetComponent<Light>().range = Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize;
        this.mineDefinition.GetComponent<Light>().range = Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize;

        this.ballButton.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 3, Screen.height / 4);
        this.cleaverButton.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 3, Screen.height / 4);
        this.mineButton.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 3, Screen.height / 4);
        this.ballButton.GetComponent<Text>().fontSize = (int)(ballButton.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);
        this.cleaverButton.GetComponent<Text>().fontSize = (int)(cleaverButton.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);
        this.mineButton.GetComponent<Text>().fontSize = (int)(mineButton.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);

        this.title.GetComponent<Text>().fontSize = (int)(title.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);
        this.paragraph.GetComponent<Text>().fontSize = (int)(paragraph.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);
        this.mainMenu.GetComponent<Text>().fontSize = (int)(mainMenu.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);

        Vector3 ballPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.166f, 0.2f));
        ballPosition.z = 0.0f;
        Vector3 cleaverPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.498f, 0.2f));
        cleaverPosition.z = 0.0f;
        Vector3 minePosition = Camera.main.ViewportToWorldPoint(new Vector3(0.833f, 0.2f));
        minePosition.z = 0.0f;

        Vector3 ballButtonPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.166f, 0.2f));
        Vector3 cleaverButtonPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.498f, 0.2f));
        Vector3 mineButtonPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.833f, 0.2f));

        Vector3 titlePosition = Camera.main.ViewportToScreenPoint(new Vector3(0.1f, 0.9f));
        Vector3 paragraphPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.1f, 0.8f));
        Vector3 mainMenuPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.9f, 0.9f));

        this.ballButton.transform.position = ballButtonPosition;
        this.cleaverButton.transform.position = cleaverButtonPosition;
        this.mineButton.transform.position = mineButtonPosition;
        
        this.title.transform.position = titlePosition;
        this.paragraph.transform.position = paragraphPosition;
        this.mainMenu.transform.position = mainMenuPosition;

        this.mainMenu.GetComponent<Text>().text = SwipeballConstants.MenuText.MainMenu;

        GameObject ball = (GameObject)Instantiate(ballDefinition, ballPosition, Quaternion.identity);
        GameObject cleaver = (GameObject)Instantiate(cleaverDefinition, cleaverPosition, Quaternion.identity);
        GameObject mine = (GameObject)Instantiate(mineDefinition, minePosition, Quaternion.identity);

        StopAllAnimations(ball, cleaver, mine);

        this.ballButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            StopAllAnimations(ball, cleaver, mine);
            BallSelection(ball);
        });
        this.cleaverButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            StopAllAnimations(ball, cleaver, mine);
            CleaverSelection(cleaver);
        });
        this.mineButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            StopAllAnimations(ball, cleaver, mine);
            MineSelection(mine);
        });
        this.mainMenu.GetComponent<Button>().onClick.AddListener(() =>
        {
            Application.LoadLevel(SwipeballConstants.LevelNames.MainMenu);
        });
    }

    private void StandardSelection()
    {
        //Replace the text with Standard Selection text and continue to act like a boring hag till prompted
        this.title.GetComponent<Text>().text = TextConstants.StandardSelection.Title;
        this.paragraph.GetComponent<Text>().text = TextConstants.StandardSelection.Paragraph;
    }

    private void BallSelection(GameObject ball)
    {
        // Replace the text with Ball Selection text and start the particle system animation
        this.title.GetComponent<Text>().text = TextConstants.BallSelection.Title;
        this.paragraph.GetComponent<Text>().text = TextConstants.BallSelection.Paragraph;

        ball.GetComponent<ParticleSystem>().Play();
    }

    private void CleaverSelection(GameObject cleaver)
    {
        // Replace the text with Cleaver Selection text and start the rotation and colour change animation
        this.title.GetComponent<Text>().text = TextConstants.CleaverSelection.Title;
        this.paragraph.GetComponent<Text>().text = TextConstants.CleaverSelection.Paragraph;

        cleaver.GetComponent<Rigidbody2D>().angularVelocity = 10;
        cleaver.GetComponent<Light>().color = Color.green;
    }

    private void MineSelection(GameObject mine)
    {
        // Replace the text with Mine Selection text and start the particle system animation
        this.title.GetComponent<Text>().text = TextConstants.MineSelection.Title;
        this.paragraph.GetComponent<Text>().text = TextConstants.MineSelection.Paragraph;

        mine.GetComponent<Light>().color = Color.red;
        mine.GetComponent<ParticleSystem>().Play();
    }

    private void StopAllAnimations(GameObject ball, GameObject cleaver, GameObject mine)
    {
        ball.GetComponent<ParticleSystem>().Stop();
        cleaver.GetComponent<Rigidbody2D>().angularVelocity = 0;
        cleaver.GetComponent<Light>().color = Color.red;
        mine.GetComponent<ParticleSystem>().Stop();
        mine.GetComponent<Light>().color = Color.cyan;
    }
}
