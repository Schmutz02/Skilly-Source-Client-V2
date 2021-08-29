using System;
using System.Collections;
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

        [SerializeField]
        private RectTransform _hud;

        private PacketHandler _packetHandler;

        private int _screenWidth;
        private int _screenHeight;

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
            StartCoroutine(Resize());

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
            
            if (Screen.width != _screenWidth || Screen.height != _screenHeight)
                StartCoroutine(Resize());

            _packetHandler.Tick();
            _map.Tick();
        }

        private IEnumerator Resize()
        {
            yield return new WaitForEndOfFrame();
            var screenWidth = Screen.width;
            Camera.main.rect = new Rect(0, 0, (screenWidth - 200 * _hud.localScale.x) / screenWidth, 1);
            _screenWidth = screenWidth;
            _screenHeight = Screen.height;
        }
    }
}