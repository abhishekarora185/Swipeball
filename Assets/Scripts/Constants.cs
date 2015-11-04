using UnityEngine;
using System.Collections;

public class SwipeballConstants {

	// Constants to be used throughout the project

	public class EntityNames
	{
		// Names of entities in the game

		public const string Ball = "Ball";

		public const string Cleaver = "Cleaver";

		public const string Mine = "Mine(Clone)";

		public const string VerticalWalls = "Vertical Walls";

		public const string HorizontalWalls = "Horizontal Walls";

        public const string Spawner = "Spawner";

        public const string Walls = "Walls";

        public const string Scorekeeper = "Scorekeeper";

        public const string Score = "Score";

        public const string HighScore = "High Score";

        public const string GameOver = "Game Over";

        public const string PlayAgain = "Play Again";

        public const string MainMenu = "Main Menu";

        public const string Instructions = "Instructions";

        public const string Credits = "Credits";

        public const string Play = "Play";

        public const string Title = "Title";

        public const string Paragraph = "Paragraph";

        public const string BallButton = "Ball Button";

        public const string CleaverButton = "Cleaver Button";

        public const string MineButton = "Mine Button";

        public const string ActiveEntityTag = "Active Entity";

        public const string DecorativeTag = "Decorative";

        public const string TextTag = "Text";
	}

    public class Effects
    {
        public const float DeathLightIntensityFade = 0.01f;

        public const float GameStartAnimationDuration = 2.0f;
    }

    public class MenuText
    {
        public const string GameOver = "GAME\nOVER";

        public const string Score = "Your Score : ";

        public const string HighScore = "High Score : ";

        public const string PlayAgain = "Play\nAgain";

        public const string MainMenu = "Main\nMenu";

        public const string Instructions = "Instructions";

        public const string Credits = "Credits";
    }

    public class MaterialNames
    {
        public const string BouncyMaterial = "Bouncy Material";
    }

    public class LevelNames
    {
        public const string Game = "Game";

        public const string MainMenu = "Main Menu";

        public const string Instructions = "Instructions";
    }

    public class FileSystem
    {
        public const string AppDataFileName = "/playerinfo.dat";
    }

    // The Screen.height values for a scale of 1 on the main objects' (ball, cleaver and mines) transform
    // Used to determine the scale value for different screen sizes, given the desired scale on a screen of given resolution
    public class Scaling
    {
        public const float MenuHeightForOriginalSize = 80;

        public const float InstructionsHeightForOriginalSize = 160;

        public const float GameHeightForOriginalSize = 480;
    }

    public class PhysicsHacks
    {
        public const float SpeedCap = 775.0f;

        public const float SquareSpeedCap = 600000.0f;
    }
}
