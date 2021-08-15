using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Models;
using Networking.Packets;
using UI;
using UnityEngine;

namespace Networking
{
    public enum SocketEventState
    {
        Awaiting,
        InProgress
    }

    public static class TcpTicker
    {
        private const int BUFFER_SIZE = 0x50000;
        private const int PREFIX_LENGTH = 5;
        private const int PREFIX_LENGTH_WITH_ID = PREFIX_LENGTH - 1;
        
        private static Task _tickingTask;
        private static bool _crashed;
        public static bool Running => _tickingTask != null && !_tickingTask.IsCompleted;

        private static readonly SendState _Send = new SendState();
        private static readonly ReceiveState _Receive = new ReceiveState();
        
        private static Socket _socket;

        private static ConcurrentQueue<OutgoingPacket> _pending;
        
        private static PacketHandler _packetHandler;

        private static readonly Dictionary<PacketId, IncomingPacket> _IncomingPackets =
            new Dictionary<PacketId, IncomingPacket>();

        static TcpTicker()
        {
            foreach (var type in typeof(IncomingPacket).Assembly.GetTypes())
            {
                if (typeof(IncomingPacket).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var packet = (IncomingPacket)Activator.CreateInstance(type);
                    _IncomingPackets.Add(packet.Id, packet);
                }
            }
        }

        // called on main thread
        public static void Start(PacketHandler packetHandler)
        {
            if (Running)
            {
                Debug.LogWarning("TcpTicker already started");
                return;
            }
            
            try
            {
                _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(Settings.IP_ADDRESS, Settings.GAME_PORT);
                _socket.Blocking = false;
            }
            catch (Exception)
            {
                Debug.LogError("Unable to connect to game server");
                ViewManager.Instance.ChangeView(View.Character);
                return;
            }

            Debug.Log("Connected!");
            
            _packetHandler = packetHandler;
            _pending = new ConcurrentQueue<OutgoingPacket>();
            _crashed = false;
            _tickingTask = Task.Run(Tick);
        }

        // called on main thread
        public static void Stop()
        {
            if (!Running && !_crashed)
                return;
            
            _Send.Reset();
            _Receive.Reset();
            _tickingTask = null;
            _crashed = false;
            
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch (Exception e)
            {
               Debug.Log(e.Message); 
            }
            
            Debug.Log("Disconnected!");
        }

        // called on main thread
        public static void Send(OutgoingPacket packet)
        {
            if (!Running)
            {
                Debug.LogError("Can not add packet to inactive ticker");
                return;
            }
            
            _pending.Enqueue(packet);
        }

        // called on worker thread
        private static void Tick()
        {
            try
            {
                while (Running)
                {
                    StartSend();
                    StartReceive();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                _crashed = true;
            }
        }
        
        // called on worker thread
        private static void StartSend()
        {
            switch (_Send.State)
            {
                case SocketEventState.Awaiting:
                    if (_pending.TryDequeue(out var packet))
                    {
                        using (var wtr = new PacketWriter(new MemoryStream()))
                        {
                            wtr.Write((byte)packet.Id);
                            packet.Write(wtr);
                            
                            var bytes = ((MemoryStream) wtr.BaseStream).ToArray();
                            _Send.PacketBytes = bytes;
                            _Send.PacketLength = bytes.Length;
                        }
                        _Send.State = SocketEventState.InProgress;
                        StartSend();
                    }
                    break;
                case SocketEventState.InProgress:
                    Buffer.BlockCopy(_Send.PacketBytes, 0, _Send.Data, PREFIX_LENGTH_WITH_ID, _Send.PacketLength);
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(_Send.PacketLength + PREFIX_LENGTH_WITH_ID)), 0, _Send.Data, 0, PREFIX_LENGTH_WITH_ID);
                    var written = _socket.Send(_Send.Data, _Send.BytesWritten, _Send.PacketLength + PREFIX_LENGTH_WITH_ID - _Send.BytesWritten, SocketFlags.None);
                    if (written < _Send.PacketLength + PREFIX_LENGTH_WITH_ID)
                        _Send.BytesWritten += written;
                    else
                        _Send.Reset();
                    StartSend();
                    break;
            }
        }
        
        // called on worker thread
        private static void StartReceive()
        {
            switch (_Receive.State)
            {
                case SocketEventState.Awaiting:
                    if (_socket.Available >= PREFIX_LENGTH)
                    {
                        _socket.Receive(_Receive.PacketBytes, PREFIX_LENGTH, SocketFlags.None);
                        _Receive.PacketLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_Receive.PacketBytes, 0));
                        _Receive.State = SocketEventState.InProgress;
                        StartReceive();
                    }
                    break;
                case SocketEventState.InProgress:
                    if (_Receive.PacketLength < PREFIX_LENGTH ||
                        _Receive.PacketLength > BUFFER_SIZE)
                    {
                        throw new Exception($"Unable to process packet of size {_Receive.PacketLength}");
                    }

                    if (_socket.Available + PREFIX_LENGTH >= _Receive.PacketLength) //Full packet now arrived. Time to process it.
                    {
                        if (_socket.Available != 0)
                            _socket.Receive(_Receive.PacketBytes, PREFIX_LENGTH, _Receive.PacketLength - PREFIX_LENGTH, SocketFlags.None);
                        var packetId = (PacketId) _Receive.GetPacketId();
                        Debug.Log(packetId);
                        var packetBody = _Receive.GetPacketBody();
                        var packet = _IncomingPackets[packetId].CreateInstance();
                        using (var rdr = new PacketReader(new MemoryStream(packetBody)))
                        {
                            packet.Read(rdr);
                        }
                        _packetHandler.AddPacket(packet);
                        _Receive.Reset();
                    }

                    StartReceive();
                    break;
            }
        }
        
        private class ReceiveState
        {
            public int PacketLength;
            public readonly byte[] PacketBytes;
            public SocketEventState State;

            public ReceiveState()
            {
                PacketBytes = new byte[BUFFER_SIZE];
                PacketLength = PREFIX_LENGTH;
            }

            public byte[] GetPacketBody()
            {
                var packetBody = new byte[PacketLength - PREFIX_LENGTH];
                Array.Copy(PacketBytes, PREFIX_LENGTH, packetBody, 0, packetBody.Length);
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

        private class SendState
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
    }
}