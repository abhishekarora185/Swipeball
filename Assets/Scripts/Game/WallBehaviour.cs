/*
 * Author: Abhishek Arora
 * This is the Behaviour script attached to the Walls, that keep everything within the field of play
 * */

using UnityEngine;
using System.Collections;

public class WallBehaviour : MonoBehaviour {

	// Sets the position and size of the boundaries based on screen size

	// Use this for initialization
	void Start () {
		GameObject verticalWalls = GameObject.Find(SwipeballConstants.GameObjectNames.Game.VerticalWalls);
		GameObject horizontalWalls = GameObject.Find(SwipeballConstants.GameObjectNames.Game.HorizontalWalls);

		// Add colliders for all sides of the bounding box
		EdgeCollider2D leftWall = verticalWalls.AddComponent<EdgeCollider2D>();
		EdgeCollider2D rightWall = verticalWalls.AddComponent<EdgeCollider2D>();
		EdgeCollider2D bottomWall = horizontalWalls.AddComponent<EdgeCollider2D>();
		EdgeCollider2D topWall = horizontalWalls.AddComponent<EdgeCollider2D>();

		// Enable the walls to push back objects so that they don't stick to it
		PhysicsMaterial2D bouncyMaterial = new PhysicsMaterial2D(SwipeballConstants.MaterialNames.BouncyMaterial);
		leftWall.sharedMaterial = bouncyMaterial;
		rightWall.sharedMaterial = bouncyMaterial;
		bottomWall.sharedMaterial = bouncyMaterial;
		topWall.sharedMaterial = bouncyMaterial;

		// Place the colliders along the boundaries of the viewport
		leftWall.points = new System.Collections.Generic.List<Vector2>() { 
			Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z)), 
			Camera.main.ViewportToWorldPoint(new Vector3(0, 1, -Camera.main.transform.position.z)) 
		}.ToArray();
		rightWall.points = new System.Collections.Generic.List<Vector2>() { 
			Camera.main.ViewportToWorldPoint(new Vector3(1, 0, -Camera.main.transform.position.z)), 
			Camera.main.ViewportToWorldPoint(new Vector3(1, 1, -Camera.main.transform.position.z)) 
		}.ToArray();
		bottomWall.points = new System.Collections.Generic.List<Vector2>() { 
			Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z)), 
			Camera.main.ViewportToWorldPoint(new Vector3(1, 0, -Camera.main.transform.position.z)) 
		}.ToArray();
		topWall.points = new System.Collections.Generic.List<Vector2>() {
			Camera.main.ViewportToWorldPoint(new Vector3(0, 1, -Camera.main.transform.position.z)), 
			Camera.main.ViewportToWorldPoint(new Vector3(1, 1, -Camera.main.transform.position.z)) 
		}.ToArray();
	}

}
