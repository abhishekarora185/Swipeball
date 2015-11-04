using UnityEngine;
using System.Collections;

public class AnimationBehaviour : MonoBehaviour {

    public static IEnumerator PlayDeathAnimation(GameObject deadObject)
    {
        // Disable the object's tangible attributes so that it can explode and die in peace
        deadObject.GetComponent<Rigidbody2D>().Sleep();
        deadObject.GetComponent<CircleCollider2D>().enabled = false;
        deadObject.GetComponent<SpriteRenderer>().enabled = false;

        deadObject.GetComponent<ParticleSystem>().Play();

        while (deadObject != null && deadObject.GetComponent<Light>().intensity > 0)
        {
            deadObject.GetComponent<Light>().intensity -= SwipeballConstants.Effects.DeathLightIntensityFade;
        }

        // Provides enough time for the above animation to play
        yield return new WaitForSeconds(deadObject.GetComponent<ParticleSystem>().duration);

        SpawnBehaviour.KillObject(deadObject);
    }

}
