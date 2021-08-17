using System;
using Game;
using Models;
using Networking;
using Networking.Packets.Incoming;
using Networking.Packets.Outgoing;
using UnityEngine;

namespace UI.GameScreen
{
    public class GameScreenController : UIController
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
        
        private void OnReconnect(GameInitData initData)
        {
            ViewManager.Instance.ChangeView(View.Game, initData);
        }
        
        public override void Reset(object data)
        {
            Camera.main.backgroundColor = Color.black;
            Camera.main.rect = new Rect(0, 0, .75f, 1);

            var initData = (GameInitData) data;
            _packetHandler = new PacketHandler(initData, _map);
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
            _map.Tick();
        }
    }
}