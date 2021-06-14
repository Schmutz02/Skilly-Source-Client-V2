using Networking;
using UI;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Map _map;

        [SerializeField]
        private MainCameraManager _cameraManager;

        private PacketHandler _packetHandler;
        
        public void StartGame(int worldId, int charId, bool newCharacter)
        {
            _packetHandler = new PacketHandler(worldId, charId, newCharacter, _map);
            ViewManager.Instance.ChangeView(View.Game);
        }

        private void OnEnable()
        {
            _packetHandler.Start();
        }

        private void OnDisable()
        {
            _packetHandler.Stop();
            _map.Clear();
            _cameraManager.Clear();
            _cameraManager.SetFocus(null);
        }

        private void Update()
        {
            _packetHandler.Tick();
        }
    }
}