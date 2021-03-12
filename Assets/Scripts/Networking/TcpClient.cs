using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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

    public readonly struct Packet
    {
        public readonly PacketId Id;
        public readonly byte[] Body;

        public Packet(PacketId id, byte[] body)
        {
            Id = id;
            Body = body;
        }
    }
    
    public static partial class TcpClient
    {
        public const int BUFFER_SIZE = 0x50000;
        public const int PREFIX_LENGTH = 5;
        public const int PREFIX_LENGTH_WITH_ID = PREFIX_LENGTH - 1;

        private static readonly Socket _Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private static readonly ConcurrentQueue<byte[]> _Pending = new ConcurrentQueue<byte[]>();

        private static Task _tickingTask;
        private static bool _running;

        private static readonly SendState _Send = new SendState();
        private static readonly ReceiveState _Receive = new ReceiveState();

        public static void Init()
        {
            _Send.Reset();
            _Receive.Reset();
            try
            {
                _Socket.Connect(Settings.IP_ADDRESS, Settings.GAME_PORT);
            }
            catch (Exception)
            {
                Debug.LogError("Unable to connect to game server");
                ScreenManager.Instance.ChangeScreen(Screen.Character);
                return;
            }
            
            if (!_Socket.Connected)
            {
                Debug.LogError("Unable to connect to game server");
                ScreenManager.Instance.ChangeScreen(Screen.Character);
                return;
            }
            
            Debug.Log("Connected!");
            _tickingTask = Task.Run(NetworkTick);
            _running = true;
        }

        public static async Task StopAsync()
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
                _Socket.Disconnect(true);
            }
            catch
            {
                
            }

            Debug.Log("Disconnected!");
        }

        private static void NetworkTick()
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
                StopAsync();
            }
        }
        
        private static void StartSend()
        {
            switch (_Send.State)
            {
                case SocketEventState.Awaiting:
                    if (_Pending.TryDequeue(out var packet))
                    {
                        _Send.PacketBytes = packet;
                        _Send.PacketLength = packet.Length;
                        _Send.State = SocketEventState.InProgress;
                        StartSend();
                    }
                    break;
                case SocketEventState.InProgress:
                    Buffer.BlockCopy(_Send.PacketBytes, 0, _Send.Data, PREFIX_LENGTH_WITH_ID, _Send.PacketLength);
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(_Send.PacketLength + PREFIX_LENGTH_WITH_ID)), 0, _Send.Data, 0, PREFIX_LENGTH_WITH_ID);
                    var written = _Socket.Send(_Send.Data, _Send.BytesWritten, _Send.PacketLength + PREFIX_LENGTH_WITH_ID - _Send.BytesWritten, SocketFlags.None);
                    if (written < _Send.PacketLength + PREFIX_LENGTH_WITH_ID)
                        _Send.BytesWritten += written;
                    else
                        _Send.Reset();
                    StartSend();
                    break;
            }
        }
        
        private static void StartReceive()
        {
            switch (_Receive.State)
            {
                case SocketEventState.Awaiting:
                    if (_Socket.Available >= PREFIX_LENGTH)
                    {
                        _Socket.Receive(_Receive.PacketBytes, PREFIX_LENGTH, SocketFlags.None);
                        _Receive.PacketLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_Receive.PacketBytes, 0));
                        _Receive.State = SocketEventState.InProgress;
                        StartReceive();
                    }
                    break;
                case SocketEventState.InProgress:
                    if (_Receive.PacketLength < PREFIX_LENGTH ||
                        _Receive.PacketLength > BUFFER_SIZE)
                    {
                        //TODO will probably cause problems if this happens
                        StopAsync().RunSynchronously();
                        return;
                    }

                    if (_Socket.Available + PREFIX_LENGTH >= _Receive.PacketLength) //Full packet now arrived. Time to process it.
                    {
                        if (_Socket.Available != 0)
                            _Socket.Receive(_Receive.PacketBytes, PREFIX_LENGTH, _Receive.PacketLength - PREFIX_LENGTH, SocketFlags.None);
                        PacketHandler.Instance.Read(new Packet((PacketId)_Receive.GetPacketId(), _Receive.GetPacketBody()));
                        _Receive.Reset();
                    }

                    StartReceive();
                    break;
            }
        }
    }
}