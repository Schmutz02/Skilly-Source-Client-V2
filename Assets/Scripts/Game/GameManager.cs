using Networking;
using Networking.Packets.Incoming;
using Networking.Packets.Outgoing;
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

        private void Awake()
        {
            Reconnect.OnReconnect += OnReconnect;
        }

        private void OnReconnect(int worldId, int charId, bool newCharacter)
        {
            ViewManager.Instance.DisableCurrentView(); // clean up assets and disconnect
            _packetHandler = new PacketHandler(worldId, charId, newCharacter, _map);
            ViewManager.Instance.EnableCurrentView(); // reconnect using new packet handler
        }

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
            _map.Dispose();
            _cameraManager.Clear();
            _cameraManager.SetFocus(null);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (_packetHandler.PlayerId == -1 || _map.WorldName == "Nexus")
                    return;
                
                TcpTicker.Send(new Escape());
            }
            
            _packetHandler.Tick();
        }
    }
}