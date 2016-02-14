using UnityEngine;
using System.Collections;

public class SwipeballAnimation {

	public static IEnumerator PlayGameStartAnimation(GameObject cleaver, bool soundEnabled)
	{
		// Make the cleaver turn green and rotate faster
		cleaver.GetComponent<Rigidbody2D>().angularVelocity = 20;
		cleaver.GetComponent<Light>().color = SwipeballConstants.Colors.Cleaver.HighPower;

		if(soundEnabled && GameObject.Find(SwipeballConstants.GameObjectNames.MainMenu.MenuEffects).GetComponent<AudioSource>() != null)
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

		ball.GetComponent<Light>().range /= SwipeballConstants.Effects.RespawnLightRangeMagnify;
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

}
