public static class Constants
{
    public static class Levels
    {
        public const string LEVEL_0 = "Level 0";
        public const string LEVEL_1 = "Level 1";
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
            public const float PLAYER_STATUS = 50;
            public const string LAST_ACTIVITIES = "";

            public const string LAST_LEVEL = Levels.LEVEL_0;
            public const float LAST_POSITION_X = 0;
            public const float LAST_POSITION_Y = 0;
            public const int LAST_ORIENTATION = (int) PlayerController.PlayerOrientation.Down;
        }
    }
}
