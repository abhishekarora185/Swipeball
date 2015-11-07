using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstructionsBehaviour : MonoBehaviour {

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
		UIOperations.SetTextProperties();
		BeforeSelection();
	}

	private void ArrangeButtons()
	{
		// Resize the sprites, text and buttons and place them
		this.title = GameObject.Find(SwipeballConstants.GameObjectNames.Instructions.Title);
		this.paragraph = GameObject.Find(SwipeballConstants.GameObjectNames.Instructions.Paragraph);
		this.mainMenu = GameObject.Find(SwipeballConstants.GameObjectNames.Instructions.MainMenu);
		this.ballButton = GameObject.Find(SwipeballConstants.GameObjectNames.Instructions.BallButton);
		this.cleaverButton = GameObject.Find(SwipeballConstants.GameObjectNames.Instructions.CleaverButton);
		this.mineButton = GameObject.Find(SwipeballConstants.GameObjectNames.Instructions.MineButton);

		Vector3 spriteAndButtonScale = new Vector3(Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize, Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize, 1.0f);

		this.ballDefinition.transform.localScale = spriteAndButtonScale;
		this.cleaverDefinition.transform.localScale = spriteAndButtonScale;
		this.mineDefinition.transform.localScale = spriteAndButtonScale;

		this.ballDefinition.GetComponent<Light>().range = Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize;
		this.cleaverDefinition.GetComponent<Light>().range = Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize;
		this.mineDefinition.GetComponent<Light>().range = Screen.height / SwipeballConstants.Scaling.InstructionsHeightForOriginalSize;

		this.title.GetComponent<Text>().fontSize = (int)(title.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);
		this.paragraph.GetComponent<Text>().fontSize = (int)(paragraph.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);
		this.mainMenu.GetComponent<Text>().fontSize = (int)(mainMenu.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);

		Vector3 ballPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.166f, 0.2f));
		ballPosition.z = 0.0f;
		Vector3 cleaverPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.498f, 0.2f));
		cleaverPosition.z = 0.0f;
		Vector3 minePosition = Camera.main.ViewportToWorldPoint(new Vector3(0.833f, 0.2f));
		minePosition.z = 0.0f;
		
		this.mainMenu.GetComponent<Text>().text = SwipeballConstants.UIText.MainMenu;

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

	private void BeforeSelection()
	{
		//Replace the text with Standard Selection text and continue to act like a boring hag till prompted
		this.title.GetComponent<Text>().text = SwipeballConstants.UIText.GeneralInstructions.Title;
		this.paragraph.GetComponent<Text>().text = SwipeballConstants.UIText.GeneralInstructions.Paragraph;
	}

	private void BallSelection(GameObject ball)
	{
		// Replace the text with Ball Selection text and start the particle system animation
		this.title.GetComponent<Text>().text = SwipeballConstants.UIText.BallInstructions.Title;
		this.paragraph.GetComponent<Text>().text = SwipeballConstants.UIText.BallInstructions.Paragraph;

		ball.GetComponent<ParticleSystem>().Play();
	}

	private void CleaverSelection(GameObject cleaver)
	{
		// Replace the text with Cleaver Selection text and start the rotation and colour change animation
		this.title.GetComponent<Text>().text = SwipeballConstants.UIText.CleaverInstructions.Title;
		this.paragraph.GetComponent<Text>().text = SwipeballConstants.UIText.CleaverInstructions.Paragraph;

		cleaver.GetComponent<Rigidbody2D>().angularVelocity = 10;
		cleaver.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.HighPower;
	}

	private void MineSelection(GameObject mine)
	{
		// Replace the text with Mine Selection text and start the particle system animation
		this.title.GetComponent<Text>().text = SwipeballConstants.UIText.MineInstructions.Title;
		this.paragraph.GetComponent<Text>().text = SwipeballConstants.UIText.MineInstructions.Paragraph;

		mine.GetComponent<Light>().color = SwipeballConstants.Colors.Mine.Hostile;
		mine.GetComponent<ParticleSystem>().Play();
	}

	private void StopAllAnimations(GameObject ball, GameObject cleaver, GameObject mine)
	{
		ball.GetComponent<ParticleSystem>().Stop();
		cleaver.GetComponent<Rigidbody2D>().angularVelocity = 0;
		cleaver.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.NoPower;
		mine.GetComponent<ParticleSystem>().Stop();
		mine.GetComponent<Light>().color = SwipeballConstants.Colors.Mine.Dormant;
	}
}
