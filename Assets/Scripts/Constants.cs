﻿using UnityEngine;

public static class Constants
{
    public static class Levels
    {
        public const string START_MENU = "Start Menu";
        public const string LEVEL_0 = "Level 0";
        public const string LEVEL_1 = "Level 1";
        public const string PAUSE_MENU = "Pause Menu";
        public const string CREDITS = "Credits";
    }

    public static class Prefs
    {
        public const string USE_24_HOUR_CLOCK = "24HourClock";
        public const string VOLUME = "Volume";

        public const string CHANGING_FLOOR = "ChangingFloor";
        public const string FORCE_SLEEPING = "ForceSleeping";

        public const string GAME_TIME = "Time";
        public const string PLAYER_STATUS = "Pointer";
        public const string LAST_ACTIVITIES = "LastActivities";

        public const string LAST_LEVEL = "LastLevel";
        public const string LAST_POSITION_X = "LastPositionX";
        public const string LAST_POSITION_Y = "LastPositionY";
        public const string LAST_ORIENTATION = "LastOrientation";

        public const string THESIS_SUBMITTED = "ThesisDelivered";

        public static class Defaults
        {
            public const int USE_24_HOUR_CLOCK = 1;
            public const float VOLUME = 1;

            public const int CHANGING_FLOOR = 0;
            public const int FORCE_SLEEPING = 1;

            public const int GAME_TIME = 0;
            public const float PLAYER_STATUS = 45;
            public const string LAST_ACTIVITIES = "";

            public const string LAST_LEVEL = Levels.LEVEL_0;
            public const float LAST_POSITION_X = 0;
            public const float LAST_POSITION_Y = 0;
            public const int LAST_ORIENTATION = (int) PlayerController.PlayerOrientation.Down;

            public const int THESIS_DELIVERED = 0;
        }
    }

    public static class Credits
    {
        public const string PETE1 = "Pedro Lucas";
        public const string PETE2 = "Pedro Pereira";

        public const string SUCCESS_GREETING = 
            "Congratulations!\n" +
            "\n" + 
            "After several months of hard work,\n" +
            "you were able to submit your thesis!\n" +
            "\n" + 
            "Your final grade is...\n";

        public const string UNDELIVERED_GREETING = 
            "What a pity!\n" + 
            "\n" + 
            "After so much time and work, you were unable to\n" +
            "submit your thesis on time. Better luck next time.\n" + 
            "\n" + 
            "If you had delivered it, your grade would be...\n";

        public const string TIP_GREETING = 
            "\n\n\n\n\n\n\n\n" +
            "TIP: Moms always know best. Make sure to follow her tip!";

        public const string CREDITS_STRING = 
            "Made for 1GAM by:\n" + 
            PETE1 + "              " + PETE2 + "\n" +
            "@ordepsacul" + "               " + "@pmhpereira  ";
    }

    public static class Actions
    {
        public const string LOSE_TIME = "LOSE_TIME";
        public const string USE_BATHROOM = "USE_BATHROOM";
        public const string SLEEP_NIGHT = "SLEEP_NIGHT";
        public const string TAKE_NAP = "TAKE_NAP";
        public const string WRITE_THESIS = "WRITE_THESIS";
        public const string SUBMIT_THESIS = "SUBMIT_THESIS";
        public const string PARENTS_BEDROOM = "PARENTS_BEDROOM";

        public const string WATCH_MOVIE = "WATCH_MOVIE";
        public const string EAT_MEAL = "EAT_MEAL";
        public const string EAT_SNACK = "EAT_SNACK";
        public const string READ_LETTER = "READ_LETTER";
        public const string OPENED_LETTER = "OPENED_LETTER";
        public const string DAILY_MESSAGE = "DAILY_MESSAGE";

        public static string[] values = new string[]
        {
            LOSE_TIME,
            USE_BATHROOM,
            SLEEP_NIGHT,
            TAKE_NAP,
            WRITE_THESIS,
            SUBMIT_THESIS,
            PARENTS_BEDROOM,
            WATCH_MOVIE,
            EAT_MEAL,
            EAT_SNACK,
            READ_LETTER,
            OPENED_LETTER,
            DAILY_MESSAGE
        };
    }

    public static class Colors
    {
        public static Color FADE = new Color(.09f, .09f, .09f);
    }
    
    public static class Game
    {
        public static string PERSISTENT_OBJECT = "PersistentDataObject";
        public static string FADER_OBJECT = "Fader";
    }

    public static class Strings
    {
        public static string LETTER_MESSAGE = 
            "Hi sweetie, your father and I had to go visit grandma for this week. " +
            "We know you have been super busy finishing your thesis, but don't forget " +
            "to eat properly and try to have some fun too.\n" +
            "But, more importantly, don't forget to submit it!\n" + 
            "\n"+
            "Love,\n" + 
            "Mom";

        public static string WAIT_MESSAGE = "I should wait a little longer.";
        
        public static string[] DAILY_MOTIVATIONS = 
        {
            "I can do this!",
            "Almost there!",
            "Never give up!",
            "Final push!"
        };

        public static string LAST_DAY_MESSAGE = "I can't forget to submit it by midnight!";
    }
}
