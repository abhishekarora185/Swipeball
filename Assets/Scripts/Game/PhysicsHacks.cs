using UnityEngine;
using System.Collections;

public class PhysicsHacks {

	// All moving objects call this function to make sure they aren't moving too fast
	// This is a hack to prevent extremely fast objects from achieving escape velocity and breaking beyond the game's barriers
	public static void AddRetardingForce(Rigidbody2D extremelyFastObject)
	{
		if(extremelyFastObject.velocity.sqrMagnitude > SwipeballConstants.PhysicsHacks.SquareSpeedCap)
		{
			extremelyFastObject.AddForce((extremelyFastObject.velocity.magnitude - SwipeballConstants.PhysicsHacks.SpeedCap) * - extremelyFastObject.velocity.normalized);
		}
	}

}
