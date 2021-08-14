using UnityEngine;
using Utils;

namespace Models
{
    public static class Settings
    {
        private const string _MAP_SCALE_KEY = "Map Scale";
        private const string _CAMERA_OFFSET_KEY = "Camera Offset";
        
        private const int _WEB_PORT = 7777;
        public const int GAME_PORT = 2050;
        public const string IP_ADDRESS = "127.0.0.1";

        public const float PLAYER_ROTATE_SPEED = 0.003f;

        public const float MAXIMUM_INTERACTION_DISTANCE = 1;

        public static readonly string ServerURL = $"http://{IP_ADDRESS}:{_WEB_PORT}";

        public static float CameraAngle; // in radians
        public static float MapScale;
        public static bool CameraOffset;

        public static readonly Color NameColor = ParseUtils.ColorFromInt(16572160);
        public static readonly Color GuildNameColor = ParseUtils.ColorFromInt(10944349);

        public static void Save()
        {
            PlayerPrefs.SetFloat(_MAP_SCALE_KEY, MapScale);
            PlayerPrefs.SetInt(_CAMERA_OFFSET_KEY, CameraOffset ? 1 : 0);
        }
        
        public static void Load()
        {
            MapScale = PlayerPrefs.GetFloat(_MAP_SCALE_KEY, 6);
            CameraOffset = PlayerPrefs.GetInt(_CAMERA_OFFSET_KEY) == 1;
        }
    }
}