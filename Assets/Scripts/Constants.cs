using UnityEngine;
using System.Collections;

public class SwipeballConstants : MonoBehaviour {

	// Constants to be used throughout the project

	public class EntityNames
	{
		// Names of entities in the game

		public const string Ball = "Ball";

		public const string Cleaver = "Cleaver";

		public const string Mine = "Mine(Clone)";

		public const string VerticalWalls = "Vertical Walls";

		public const string HorizontalWalls = "Horizontal Walls";

        public const string ActiveEntityTag = "Active Entity";
	}

    public class Effects
    {
        public const float LightLerpInterval = 0.001f;

        public const float DeathLightIntensityFade = 0.01f;
    }
}
