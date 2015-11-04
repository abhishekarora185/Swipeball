using UnityEngine;
using System.Collections;

public class TextConstants {

	public class StandardSelection
    {
        public const string Title = "Instructions";

        public const string Paragraph = "Swipeball is a high score-based game.\n" +
                                        "While minor points are awarded for survival,\n" +
                                        "the major rewards (also in terms of fun)\n" +
                                        "come from destroying Mines.\n" +
                                        "\n" +
                                        "Select an item to view its description.";
    }

    public class BallSelection
    {
        public const string Title = "The Ball";

        public const string Paragraph = "The player controls the Ball.\n" +
                                        "The length and duration of the swipe input\n" + 
                                        "determine its speed.\n" +
                                        "Though frail, the Ball has an uncanny survival\n" +
                                        "instinct that lets it squeeze through\n" +
                                        "the tightest of spaces.";
    }

    public class CleaverSelection
    {
        public const string Title = "The Cleaver";

        public const string Paragraph = "The Cleaver is the Ball's partner in crime.\n" +
                                        "The Ball's only defense mechanism, it can destroy\n" +
                                        "anything in its path\n" +
                                        "(except the all-powerful walls, of course).\n" +
                                        "Its wrath is, however, short-lived.\n" +
                                        "It runs on a power source that is constantly depleting,\n" +
                                        "which can be replenished through impacts from the Ball.\n" +
                                        "When healthy, it gives off a green light that turns yellow\n" + 
                                        "at half-power.\n" +
                                        "Once it turns red, it will no longer be able to destroy Mines,\n" +
                                        "and must be reinvigorated with powerful impacts.";
    }

    public class MineSelection
    {
        public const string Title = "The Mines";

        public const string Paragraph = "The Mines are out to destroy the Ball.\n" +
                                        "Not the smartest objects in the universe,\n" + 
                                        "they rely on numbers to achieve their ends.\n" +
                                        "They are harmless to the Ball and invulnerable to the Cleaver\n" + 
                                        "when they first spawn, taking on an innocent shade of cyan,\n" + 
                                        "but they soon turn a lethal red and must be destroyed.";
    }

}
