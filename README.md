# Swipeball
An Android/iOS 2D arcade game where you survive as long as possible and attain the highest score you can.
Compete with your Facebook friends with the Leaderboard feature.

Android Link:
https://play.google.com/store/apps/details?id=com.SurrealInc.Swipeball

iOS Link:
https://itunes.apple.com/in/app/swipeball+/id1059313729

Built using the Unity engine

Game Loop track, "Swipeball" composed by Abhishek Arora

Code layout:
[There are two types of classes/files used:
Behaviour classes: Classes that are hooked into Unity's event system by being attached to GameObjects
Helper classes: Instantiable/static classes that are used by the behaviour classes and by each other]

<Assets/Scripts/>
  Constants.cs : Contains all string/numerical constants used throughout the game.
  FacebookSession.cs : Handles all interactions with Facebook to retrieve and store the usernames, profile pictures and scores of the user and his/her friends.
  SaveData.cs: Defines a save data model for local storage and exposes methods to the rest of the game for use and modification.
  Scorekeeping.cs (Behaviour): Handles the score computation during the game.
  SwipeballAnimation.cs : Handles most animations for the game.
  UIOperations.cs : Handles UI scaling based on the screen size.
  <Credits/>
    CreditsBehaviour.cs (Behaviour) : Creates UI and handles events for the Credits page.
  <Main Menu/>
    MainMenuBehaviour.cs (Behaviour) : Creates UI and handles events for the Main Menu page, and also performs Facebook sync, if permitted.
  <Settings/>
    SettingsBehaviour.cs (Behaviour) : Creates UI and handles events for the Settings page
  <Leaderboard/>
    LeaderboardBehaviour.cs (Behaviour) : Creates UI and handles events for the Leaderboard page.
  <Game/>
    BallBehaviour.cs (Behaviour) : Player controls (the Ball is the entity controlled by the player).
    CleaverBehaviour.cs (Behaviour) : Handles cleaver functionality and collisions with other objects.
    MineBehaviour.cs (Behaviour) : Handles mine functionality and collisions with other objects.
    GameOver.cs : Renders the game over screen and handles its events.
    PhysicsHacks.cs : Corrections applied through the physics engine to facilitate expected physical interactions between objects.
    ScoreIncrementBehaviour.cs (Behaviour) : Handles the life cycle of score increment text that keeps appearing whenever the score increases.
    SpawnBehaviour.cs (Behaviour) : Handles the spawning and death of the Ball, the Cleaver and Mines.
    TutorialBehaviour.cs (Behaviour) : Pauses the game for tutorials, when needed.
    WallBehaviour.cs (Behaviour) : Configures the play area bounds and adds physics features to them.
    
