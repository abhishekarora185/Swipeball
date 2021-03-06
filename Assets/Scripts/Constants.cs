﻿/*
 * Author: Abhishek Arora
 * All Constants (strings/numerical) used throughout the project are defined here
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwipeballConstants {

	// Constants/Enums to be used throughout the project

	public const int MobileFrameRate = 60;

	[System.Serializable]
	public enum ControlMode
	{
		DragAndRelease,
		FollowSwipe
	}

	[System.Serializable]
	public enum Tutorial
	{
		Ball,
		Mine,
		Cleaver,
		CleaverYellow,
		CleaverRed
	}

	public class GameObjectNames
	{
		// Present in every level as the UI game object container
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

			public const string TutorialBehaviour = "Tutorial";

			public const string TutorialText = "Tutorial Text";

			public const string NewHighScore = "New High Score";

			public const string LevelUp = "Level Up";

			public const string GameOver = "Game Over";

			public const string PlayAgain = "Play Again";

			public const string MainMenu = "Main Menu";

			public const string Music = "Music";

			// Each tutorial will have a primary game object to highlight during its run
			public static Dictionary<Tutorial, string> PrimaryGameObjectNameForTutorial = new Dictionary<Tutorial, string>
			{
				{Tutorial.Ball, GameObjectNames.Game.Ball},
				{Tutorial.Mine, GameObjectNames.Game.Mine},
				{Tutorial.Cleaver, GameObjectNames.Game.Cleaver},
				{Tutorial.CleaverYellow, GameObjectNames.Game.Cleaver},
				{Tutorial.CleaverRed, GameObjectNames.Game.Cleaver}
			};
		}

		public class Credits
		{
			public const string Title = "Title";
			
			public const string Paragraph = "Paragraph";

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

			public const string Tip = "Tip";

			public const string TipText = "Tip Text";
		}

		public class Settings
		{
			public const string Title = "Title";

			public const string MainMenu = "Main Menu";

			public const string Sound = "Sound";

			public const string SyncWithFacebook = "Sync With Facebook";

			public const string ControlMode = "Control Mode";

			public const string ResetTutorials = "Reset Tutorials";
		}

		public class Leaderboard
		{
			public const string Title = "Title";

			public const string MainMenu = "Main Menu";

			public const string LeaderboardPanel = "Leaderboard Panel";

			public const string UserList = "User List";

			public const string ListItem = "List Item";

			public const string Picture = "Picture";

			public const string Name = "Name";

			public const string Score = "Score";

			public const string LeaderboardEffects = "Leaderboard Effects";
		}

		public class GameObjectTags
		{
			public const string ActiveEntityTag = "Active Entity";

			public const string DecorativeTag = "Decorative";

			public const string TextTag = "Text";
		}

	}

	public class FacebookConstants
	{
		public const string LoggedInUserId = "me";
	}

	public class Effects
	{
		public const float RespawnLightRangeMagnify = 2.0f;

		public const float BallMoveLightRangeMagnify = 1.1f;

		public const float MineDisturbLightRangeMagnify = 1.5f;

		public const float TutorialLightRangeMagnify = 1.5f;

		public const float DeathLightIntensityFade = 0.01f;

		public const float GameStartAnimationDuration = 5.0f;

		public const float GameStartGraphicFadeDuration = 2.0f;

		public const float MineBumpAnimationDuration = 0.5f;

		public const float MineNearMissAnimationDuration = 0.5f;

		public const float SyncedMessageDuration = 2.0f;

		public const float LightIntensity = 2.5f;

		public const float ProfilePictureSize = 50.0f;

		public const string LowPowerSound = "Sounds/SFX/LowPower.wav";

		public const string MediumPowerSound = "Sounds/SFX/MediumPower.wav";

		public const string HighPowerSound = "Sounds/SFX/HighPowerSound.wav";

		public const string NewHighScoreSound = "Sounds/SFX/Button_Press.wav";
	}

	public class GameObjectQuantities
	{
		public class Mine
		{
			// The number of update cycles till the mines reorient themselves towards the ball
			public const int ReorientationDelay = 100;
			// The multiplier to the force added to the mine for reorientation
			public const float ReorientationSensitivity = 20.0f;
			// The multiplier to the force with which two mines repel each other (they tend to get stuck together otherwise)
			public const float RepulsionSensitivity = 50.0f;
			// The multiplier to the explosive force released during the death of a mine
			public const float ExplosionSensitivity = 3.0f;
		}

		public class Cleaver
		{
			// The maximum battery level of the cleaver
			public const int MaxPower = 5000;
			// The maximum mass that can be added to the cleaver through power drain
			public const float MaxAdditionalMass = 3;
			// The multiplier to the power restored by an impact from the ball
			public const float ChargeSensitivity = 20.0f;
			// The multiplier to the force which prevents the cleaver from sticking to walls
			public const float RepulsionSensitivity = 50.0f;
		}
	}

	public class ScoreIncrements
	{
		public static int Persistent = 1;

		public static int MineKilled = 30;

		public static int MineNearMissed = 5;

		public static int MineBumped = 50;

		public static int GameObjectAliveFrames = 1;
	}

	public class Colors
	{
		public class UI
		{
			public static Color NormalText = Color.white;

			public static Color ButtonText = Color.cyan;

			public static Color ScoreIncrementText = Color.white;
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

		public const string Credits = "People";

		public const string Settings = "Settings";

		public const string NewHighScore = "New High Score";

		public const string YouBeatSomeone = "You Beat ";

		public const string Level = "Level ";

		public const string Lives = "Lives ";

		public const string Sound = "Sound ";

		public const string SyncWithFacebook = "Sync With Facebook ";

		public const string ResetTutorials = "Reset Tutorials";

		public const string On = "On";

		public const string Off = "Off";

		public const string OfflineName = "Offline ";

		public const string ConnectToFacebook = "Connect To\nFacebook";

		public const string Disconnect = "Disconnect";

		public const string Leaderboard = "Leaderboard";

		public const string Synced = "Synced";

		public const string User = "User";

		public const string CreditsText = "Game & Music : Abhishek Arora [Surreal, Inc.]\n\n" +
			"Font \"SavedByZero\" : Ray Larabie\n";

		public const string ControlModes = "Control Mode ";

		public static Dictionary<ControlMode, string> ControlModeDisplayName = new Dictionary<ControlMode,string> {
			{ControlMode.FollowSwipe, "Follow Swipe"},
			{ControlMode.DragAndRelease, "Drag and Release"}
		};

		public static Dictionary<Tutorial, string> TutorialText = new Dictionary<Tutorial, string>
		{
			{Tutorial.Ball, "Move the ball around using your finger."},
			{Tutorial.Mine, "Bump into blue mines, avoid red ones."},
			{Tutorial.Cleaver, "The Cleaver can be used to destroy red mines."},
			{Tutorial.CleaverYellow, "The Cleaver has powered down!\n" +
									  "Keep hitting it with the ball to energize it."},
			{Tutorial.CleaverRed, "The Cleaver's power level is too low to destroy mines.\n" +
								  "It must be recharged through impacts with the ball."}
		};

		public static string[] TipText = {
			"Try to hit mines as they spawn, when they're blue.\n" +
			"But do so carefully - they will quickly turn lethal.",
			"You can choose between two control modes in the Settings menu.",
			"Compete with your Facebook friends by turning on\n" +
			"Facebook sync in the Settings menu.",
			"If you think you need to see the tutorials again,\n" +
			"reset them in the Settings menu.",
			"You are rewarded for every near miss with a mine.",
			"The Cleaver continuously powers down, going from green to yellow to red.\n" +
			"It cannot destroy mines when out of power.\n"+
			"Keep knocking it around to charge it.",
			"Destroying successive waves of mines grants you extra lives\n" +
			"to keep you going.",
			"Try to hide behind the cleaver while mines approach.\n" +
			"They aren't smart enough to avoid its wrath.",
			"Stay up to date with your Facebook friends' progress by viewing\n" +
			"the Leaderboard.",
			"You are periodically awarded a point as a survival reward."
		};

	}

	public class MaterialNames
	{
		public const string BouncyMaterial = "Bouncy Material";
	}

	public class LevelNames
	{
		public const string Game = "Game";

		public const string MainMenu = "Main Menu";

		public const string Credits = "Credits";

		public const string Settings = "Settings";

		public const string Leaderboard = "Leaderboard";
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

		public const float GameHeightForOriginalSize = 480;

		public const float CleaverScaleMultiplier = 2.5f;
	}

	public class Input
	{
		public const float DragAndReleaseInputSensitivity = 1.0f;

		public const float FollowSwipeInputSensitivity = 70.0f;
	}

	// Needed to enable the thin wall colliders to contain all objects within them
	public class PhysicsHacks
	{
		public const float SpeedCap = 200.0f;

		public const float SquareSpeedCap = 40000.0f;
	}
}
