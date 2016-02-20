using UnityEngine;
using System.Collections;

public class SwipeballConstants {

	// Constants to be used throughout the project

	public class GameObjectNames
	{
		// Present in every level
		public const string Canvas = "Canvas";

		// Names of entities in the game
		public class Game
		{
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

			public const string Lives = "Lives";

			public const string NewHighScore = "New High Score";

			public const string LevelUp = "Level Up";

			public const string GameOver = "Game Over";

			public const string PlayAgain = "Play Again";

			public const string MainMenu = "Main Menu";

			public const string Music = "Music";
		}

		public class InstructionsAndCredits
		{
			public const string Title = "Title";
			
			public const string Paragraph = "Paragraph";

			public const string BallButton = "Ball Button";

			public const string CleaverButton = "Cleaver Button";

			public const string MineButton = "Mine Button";

			public const string MainMenu = "Main Menu";
		}

		public class MainMenu
		{
			public const string MenuEffects = "Menu Effects";

			public const string Instructions = "Instructions";

			public const string Credits = "Credits";

			public const string Settings = "Settings";

			public const string HighScore = "High Score";

			public const string Play = "Play";

			public const string Title = "Title";

			public const string Greeting = "Greeting";

			public const string Leaderboard = "Leaderboard";

			public const string ProfilePicture = "Profile Picture";
		}

		public class Settings
		{
			public const string Title = "Title";

			public const string MainMenu = "Main Menu";

			public const string Sound = "Sound";

			public const string SyncWithFacebook = "Sync With Facebook";
		}

		public class Leaderboard
		{
			public const string Title = "Title";

			public const string MainMenu = "Main Menu";
		}

		public class ObjectTags
		{
			public const string ActiveEntityTag = "Active Entity";

			public const string DecorativeTag = "Decorative";

			public const string TextTag = "Text";
		}

	}

	public class Effects
	{
		public const float RespawnLightRangeMagnify = 2.0f;

		public const float DeathLightIntensityFade = 0.01f;

		public const float GameStartAnimationDuration = 2.0f;

		public const float SyncedMessageDuration = 2.0f;

		public const float LightIntensity = 2.5f;

		public const string LowPowerSound = "Sounds/SFX/LowPower.wav";

		public const string MediumPowerSound = "Sounds/SFX/MediumPower.wav";

		public const string HighPowerSound = "Sounds/SFX/HighPowerSound.wav";

		public const string NewHighScoreSound = "Sounds/SFX/Button_Press.wav";
	}

	public class Colors
	{
		public class UI
		{
			public static Color NormalText = Color.white;

			public static Color ButtonText = Color.cyan;
		}

		public class Cleaver
		{
			public static Color HighPower = Color.green;

			public static Color LowPower = Color.yellow;

			public static Color NoPower = Color.red;
		}

		public class Mine
		{
			public static Color Dormant = Color.cyan;

			public static Color Hostile = Color.red;

			public static Color Particle1 = Color.red;

			public static Color Particle2 = Color.yellow;
		}
	}

	public class UIText
	{
		public const string GameOver = "GAME\nOVER";

		public const string Score = "Score ";

		public const string HighScore = "High Score ";

		public const string PlayAgain = "Play\nAgain";

		public const string MainMenu = "Main\nMenu";

		public const string Instructions = "Instructions";

		public const string Credits = "People";

		public const string Settings = "Settings";

		public const string NewHighScore = "New High Score!";

		public const string Level = "Level ";

		public const string Lives = "Lives ";

		public const string Sound = "Sound ";

		public const string SyncWithFacebook = "Sync With Facebook ";

		public const string On = "On";

		public const string Off = "Off";

		public const string OfflineName = "Offline ";

		public const string ConnectToFacebook = "Connect To\nFacebook";

		public const string Disconnect = "Disconnect";

		public const string Leaderboard = "Leaderboard";

		public const string Synced = "Synced";

		public const string User = "User";

		public const string CreditsText = "Game & Music : Abhishek Arora [Surreal, Inc.]\n" +
			"Font \"SavedByZero\" : Ray Larabie\n";

		public class GeneralInstructions
		{
			public const string Title = "Instructions";

			public const string Paragraph = "Swipeball is a high score-based game.\n" +
											"While minor points are awarded for survival,\n" +
											"the major rewards come from destroying Mines.\n" +
											"\n" +
											"Select any of the below items to read further.";
		}

		public class BallInstructions
		{
			public const string Title = "The Ball";

			public const string Paragraph = "The player controls the Ball.\n" +
											"The length of the swipe input determines its speed.\n" +
											"The player starts with one life, but is awarded more chances\n" + 
											"as the game progresses.";
		}

		public class CleaverInstructions
		{
			public const string Title = "The Cleaver";

			public const string Paragraph = "The Cleaver is the Ball's sole defense mechanism.\n" +
											"It can destroy mines when they are not in their\n" + 
											"dormant state. It, however, runs on a power source\n" + 
											"that is constantly depleting, which can be replenished\n" + 
											"through impacts from the Ball.\n" +
											"When healthy, it gives off a green light that turns yellow\n" +
											"at half-power.\n" +
											"Once it turns red, it will no longer be able to destroy Mines,\n" +
											"and must be recharged with powerful impacts.";
		}

		public class MineInstructions
		{
			public const string Title = "The Mines";

			public const string Paragraph = "The Mines are out to destroy the Ball.\n" +
											"They are harmless to the Ball\n" + 
											"and invulnerable to the Cleaver\n" +
											"when they first spawn in their dormant state,\n" + 
											"taking on an innocent shade of cyan,\n" +
											"but they soon turn a lethal red and must be destroyed.";
		}
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

		public const string Credits = "Credits";

		public const string Settings = "Settings";
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

		public const float CleaverScaleMultiplier = 2.5f;
	}

	public class PhysicsHacks
	{
		public const float SpeedCap = 200.0f;

		public const float SquareSpeedCap = 40000.0f;
	}
}
