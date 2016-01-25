public static class Constants
{
    public static class Levels
    {
        public const string START_MENU = "Start Menu";
        public const string LEVEL_0 = "Level 0";
        public const string LEVEL_1 = "Level 1";
        public const string PAUSE_MENU = "Pause Menu";
    }

    public static class Prefs
    {
        public const string USE_24_HOUR_CLOCK = "24HourClock";
        public const string VOLUME = "Volume";

        public const string CHANGING_FLOOR = "ChangingFloor";
        public const string GAME_TIME = "Time";
        public const string PLAYER_STATUS = "Pointer";
        public const string LAST_ACTIVITIES = "LastActivities";

        public const string LAST_LEVEL = "LastLevel";
        public const string LAST_POSITION_X = "LastPositionX";
        public const string LAST_POSITION_Y = "LastPositionY";
        public const string LAST_ORIENTATION = "LastOrientation";

        public static class Defaults
        {
            public const int USE_24_HOUR_CLOCK = 1;
            public const float VOLUME = 1;

            public const int CHANGING_FLOOR = 0;
            public const int GAME_TIME = 0;
            public const float PLAYER_STATUS = 45;
            public const string LAST_ACTIVITIES = "";

            public const string LAST_LEVEL = Levels.LEVEL_0;
            public const float LAST_POSITION_X = 0;
            public const float LAST_POSITION_Y = 0;
            public const int LAST_ORIENTATION = (int) PlayerController.PlayerOrientation.Down;
        }
    }

    public static class Credits
    {
        public const string PETE1 = "Pedro Lucas";
        public const string PETE2 = "Pedro Pereira";
        public const string SUCCESS_GREETING = "Congratulations!\n\nYou managed to score %SCORE% this time.\nTry and balance your activities the best you can!\n";
        public const string FAILURE_GREETING = "Game Over!\n\nYou only scored %SCORE% and failed to reach the minimum grade to pass.\nMaybe you should %ACTIVITY% more next time.\n";
        public const string CREDITS_STRING = "Credits\n\n"+PETE1+"\n"+PETE2;
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
            EAT_SNACK
        };
    }
}
