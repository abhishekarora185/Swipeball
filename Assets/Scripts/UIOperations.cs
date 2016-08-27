/*
 * Author: Abhishek Arora
 * This is the helper class that scales text according to screen size
 * */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIOperations {

	public static void SetTextProperties()
	{
		foreach (GameObject textObject in GameObject.FindGameObjectsWithTag(SwipeballConstants.GameObjectNames.GameObjectTags.TextTag))
		{
			if (textObject.GetComponent<Text>() != null)
			{
				// UI scaling seems to be self-sufficient; add back the line below if further testing proves otherwise
				// textObject.GetComponent<Text>().fontSize = (int)(textObject.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);

				if(textObject.GetComponent<Button>() != null)
				{
					textObject.GetComponent<Text>().color = SwipeballConstants.Colors.UI.ButtonText;
				}
				else
				{
					textObject.GetComponent<Text>().color = SwipeballConstants.Colors.UI.NormalText;
				}
			}
		}
	}
}
