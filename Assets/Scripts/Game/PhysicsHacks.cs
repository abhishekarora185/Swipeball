/*
 * Author: Abhishek Arora
 * This is a helper class used to compensate for Unity's occasional inability to provide for the game's required physics behaviour
 * */

using UnityEngine;
using System.Collections.Generic;

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

	// Get the maximum force allowed to be applied to the ball
	// Used directly for input in Drag and Release mode
	public static float MaximumForce()
	{
		float maximumForce = (Screen.width + Screen.height) / 2 * SwipeballConstants.Input.DragAndReleaseInputSensitivity;
		return maximumForce;
	}

	// Get the maximum velocity the ball is allowed to have, calculated using v = u + at, where u = 0, a = MaximumForce() / mass of ball, t = physics engine compute time
	// Used directly for input in Drag and Follow mode
	public static float MaximumVelocity()
	{
		float maximumAcceleration = MaximumForce() / GameObject.Find(SwipeballConstants.GameObjectNames.Game.Ball).GetComponent<Rigidbody2D>().mass;
		float maximumVelocity = maximumAcceleration * Time.fixedDeltaTime;
		return maximumVelocity;
	}
}
