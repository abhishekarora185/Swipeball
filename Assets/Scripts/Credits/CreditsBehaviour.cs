﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsBehaviour : MonoBehaviour {

	private GameObject title;
	private GameObject paragraph;
	private GameObject mainMenu;

	// Use this for initialization
	void Start () {
		this.title = GameObject.Find(SwipeballConstants.GameObjectNames.InstructionsAndCredits.Title);
		this.paragraph = GameObject.Find(SwipeballConstants.GameObjectNames.InstructionsAndCredits.Paragraph);
		this.mainMenu = GameObject.Find(SwipeballConstants.GameObjectNames.InstructionsAndCredits.MainMenu);

		this.title.GetComponent<Text>().text = SwipeballConstants.UIText.Credits;
		this.paragraph.GetComponent<Text>().text = SwipeballConstants.UIText.CreditsText;
		this.mainMenu.GetComponent<Text>().text = SwipeballConstants.UIText.MainMenu;

		UIOperations.SetTextProperties();

		this.mainMenu.GetComponent<Button>().onClick.AddListener(() =>
		{
			Application.LoadLevel(SwipeballConstants.LevelNames.MainMenu);
		});
	}
	
}