using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwipeballAnimation {

	public static IEnumerator PlayGameStartAnimation(GameObject cleaver)
	{
		// Make the cleaver turn green and rotate faster
		cleaver.GetComponent<Rigidbody2D>().angularVelocity = 20;
		cleaver.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.HighPower;

		if(SaveDataHandler.GetLoadedSaveData().soundEnabled && GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<AudioSource>() != null)
		{
			GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<AudioSource>().PlayOneShot(GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<AudioSource>().clip);
		}

		// Provides enough time for the above animation to play
		yield return new WaitForSeconds(SwipeballConstants.Effects.GameStartAnimationDuration);

		GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<MainMenuBehaviour>().StartGame();
	}

	public static IEnumerator PlayRespawnAnimation()
	{
		// Disable the object's tangible attributes so that it can explode and die in peace
		GameObject ball = GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball);
		ball.GetComponent<Light>().range *= SwipeballConstants.Effects.RespawnLightRangeMagnify;

		// Provides enough time for the above animation to play
		yield return new WaitForSeconds(ball.GetComponent<ParticleSystem>().duration);

		if (ball != null)
		{
			ball.GetComponent<Light>().range /= SwipeballConstants.Effects.RespawnLightRangeMagnify;
		}
	}

	public static IEnumerator PlayMineBumpAnimation(GameObject mine)
	{
		// The mine is confused
		if (mine.GetComponent<Light>() != null)
		{
			mine.GetComponent<Light>().range *= SwipeballConstants.Effects.MineDisturbLightRangeMagnify;
		}

		yield return new WaitForSeconds(SwipeballConstants.Effects.MineBumpAnimationDuration);

		// The mine is back to normal
		if (mine!=null && mine.GetComponent<Light>() != null)
		{
			mine.GetComponent<Light>().range /= SwipeballConstants.Effects.MineDisturbLightRangeMagnify;
		}
	}
	
	public static IEnumerator PlayDeathAnimation(GameObject deadObject)
	{
		// Disable the object's tangible attributes so that it can explode and die in peace
		deadObject.GetComponent<Rigidbody2D>().Sleep();
		deadObject.GetComponent<CircleCollider2D>().enabled = false;
		deadObject.GetComponent<SpriteRenderer>().enabled = false;

		if(deadObject.GetComponent<Light>() != null)
		{
			deadObject.GetComponent<Light>().intensity = 0.0f;
		}
		if (deadObject.GetComponent<ParticleSystem>() != null)
		{
			deadObject.GetComponent<ParticleSystem>().Play();
		}
		if (deadObject.GetComponent<AudioSource>() != null && SaveDataHandler.GetLoadedSaveData().soundEnabled)
		{
			deadObject.GetComponent<AudioSource>().PlayOneShot(deadObject.GetComponent<AudioSource>().clip);
		}

		// Provides enough time for the above animation to play
		yield return new WaitForSeconds(deadObject.GetComponent<ParticleSystem>().duration);

		GameObject.Find(SwipeballConstants.GameObjectNames.Game.Spawner).GetComponent<SpawnBehaviour>().KillObject(deadObject);
	}

	public static IEnumerator PrintSyncedMessage()
	{
		// This is only called by main menu, so it is assumed that the required objects exist
		string originalText = GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.HighScore).GetComponent<Text>().text;
		GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.HighScore).GetComponent<Text>().text = SwipeballConstants.UIText.Synced;

		yield return new WaitForSeconds(SwipeballConstants.Effects.SyncedMessageDuration);

		GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.HighScore).GetComponent<Text>().text = originalText;
	}

}
