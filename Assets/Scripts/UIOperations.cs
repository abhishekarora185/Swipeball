using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIOperations {

	public static void SetTextProperties()
	{
		foreach (GameObject textObject in GameObject.FindGameObjectsWithTag(SwipeballConstants.GameObjectNames.ObjectTags.TextTag))
		{
			if (textObject.GetComponent<Text>() != null)
			{
				textObject.GetComponent<Text>().fontSize = (int)(textObject.GetComponent<Text>().fontSize * Screen.height / SwipeballConstants.Scaling.GameHeightForOriginalSize);

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
