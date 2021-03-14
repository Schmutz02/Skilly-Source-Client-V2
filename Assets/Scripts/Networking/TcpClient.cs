using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Models;
using UI;
using UnityEngine;
using Screen = UI.Screen;

namespace Networking
{
    public enum SocketEventState
    {
        Awaiting,
        InProgress
    }

    public class ReceiveState
    {
        public int PacketLength;
        public readonly byte[] PacketBytes;
        public SocketEventState State;

        public ReceiveState()
        {
            PacketBytes = new byte[TcpClient.BUFFER_SIZE];
            PacketLength = TcpClient.PREFIX_LENGTH;
        }

        public byte[] GetPacketBody()
        {
            var packetBody = new byte[PacketLength - TcpClient.PREFIX_LENGTH];
            Array.Copy(PacketBytes, TcpClient.PREFIX_LENGTH, packetBody, 0, packetBody.Length);
            return packetBody;
        }

        public int GetPacketId()
        {
            return PacketBytes[4];
        }

        public void Reset()
        {
            State = SocketEventState.Awaiting;
            PacketLength = 0;
        }
    }

    public class SendState
    {
        public int BytesWritten;
        public int PacketLength;
        public byte[] PacketBytes;
        public SocketEventState State;

        public readonly byte[] Data;

        public SendState()
        {
            Data = new byte[0x10000];
        }

        public void Reset()
        {
            State = SocketEventState.Awaiting;
            PacketLength = 0;
            BytesWritten = 0;
            PacketBytes = null;
        }
    }

    public partial class TcpClient : MonoBehaviour
    {
        public const int BUFFER_SIZE = 0x50000;
        public const int PREFIX_LENGTH = 5;
        public const int PREFIX_LENGTH_WITH_ID = PREFIX_LENGTH - 1;

        private Socket _socket;
        
        private static readonly ConcurrentQueue<byte[]> _Pending = new ConcurrentQueue<byte[]>();

        private Task _tickingTask;
        private bool _running;

        private readonly SendState _send = new SendState();
        private readonly ReceiveState _receive = new ReceiveState();
        
        private async void OnEnable()
        {
            await InitAsync();
            SendHello(Account.GameInitData.GameId, Account.Username, Account.Password);
        }

        private async void OnDisable()
        {
            await StopAsync();
        }

        private async Task InitAsync()
        {
            _send.Reset();
            _receive.Reset();
            try
            {
                _socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
                {
                    Blocking = false
                };
                
                await _socket.ConnectAsync(Settings.IP_ADDRESS, Settings.GAME_PORT);
            }
            catch (Exception e)
            {
                Debug.LogError("Unable to connect to game server");
                ScreenManager.Instance.ChangeScreen(Screen.Character);
                return;
            }

            Debug.Log("Connected!");
            _tickingTask = Task.Run(NetworkTick);
            _running = true;
        }

        private async Task StopAsync()
        {
            lock (_Pending)
            {
                _running = false;
            }
            
            while (_Pending.TryDequeue(out var packet))
            {
                
            }

            await _tickingTask;
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch (Exception e)
            {
                
            }

            Debug.Log("Disconnected!");
            Debug.Log(Thread.CurrentThread.Name);
        }

        private async void NetworkTick()
        {
            try
            {
                while (_running)
                {
                    StartSend();
                    StartReceive();
                }
            }
            catch (Exception)
            {
                await StopAsync();
            }
        }
        
        private void StartSend()
        {
            switch (_send.State)
            {
                case SocketEventState.Awaiting:
                    if (_Pending.TryDequeue(out var packet))
                    {
                        _send.PacketBytes = packet;
                        _send.PacketLength = packet.Length;
                        _send.State = SocketEventState.InProgress;
                        StartSend();
                    }
                    break;
                case SocketEventState.InProgress:
                    Buffer.BlockCopy(_send.PacketBytes, 0, _send.Data, PREFIX_LENGTH_WITH_ID, _send.PacketLength);
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(_send.PacketLength + PREFIX_LENGTH_WITH_ID)), 0, _send.Data, 0, PREFIX_LENGTH_WITH_ID);
                    var written = _socket.Send(_send.Data, _send.BytesWritten, _send.PacketLength + PREFIX_LENGTH_WITH_ID - _send.BytesWritten, SocketFlags.None);
                    if (written < _send.PacketLength + PREFIX_LENGTH_WITH_ID)
                        _send.BytesWritten += written;
                    else
                        _send.Reset();
                    StartSend();
                    break;
            }
        }
        
        private void StartReceive()
        {
            switch (_receive.State)
            {
                case SocketEventState.Awaiting:
                    if (_socket.Available >= PREFIX_LENGTH)
                    {
                        _socket.Receive(_receive.PacketBytes, PREFIX_LENGTH, SocketFlags.None);
                        _receive.PacketLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_receive.PacketBytes, 0));
                        _receive.State = SocketEventState.InProgress;
                        StartReceive();
                    }
                    break;
                case SocketEventState.InProgress:
                    if (_receive.PacketLength < PREFIX_LENGTH ||
                        _receive.PacketLength > BUFFER_SIZE)
                    {
                        throw new Exception($"Unable to process packet of size {_receive.PacketLength}");
                    }

                    if (_socket.Available + PREFIX_LENGTH >= _receive.PacketLength) //Full packet now arrived. Time to process it.
                    {
                        if (_socket.Available != 0)
                            _socket.Receive(_receive.PacketBytes, PREFIX_LENGTH, _receive.PacketLength - PREFIX_LENGTH, SocketFlags.None);
                        _ToBeHandled.Enqueue(new Packet((PacketId)_receive.GetPacketId(), _receive.GetPacketBody()));
                        _receive.Reset();
                    }

                    StartReceive();
                    break;
            }
        }
    }
}