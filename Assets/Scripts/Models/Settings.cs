namespace Models
{
    public static class Settings
    {
        private const int WEB_PORT = 7777;
        public const int GAME_PORT = 2050;
        public const string IP_ADDRESS = "127.0.0.1";

        public const float PLAYER_ROTATE_SPEED = 0.003f;

        public static readonly string ServerURL = $"http://{IP_ADDRESS}:{WEB_PORT}";

        public static float CameraAngle; // in radians
    }
}