using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Game;
using Models;
using UI;
using UnityEngine;
using Screen = UI.Screen;

namespace Networking
{
    public enum PacketId : byte
    {
        Failure,
        CreateSuccess,
        Create,
        PlayerShoot,
        Move,
        PlayerText,
        Text,
        ServerPlayerShoot,
        Damage,
        Update,
        Notification,
        NewTick,
        InvSwap,
        UseItem,
        ShowEffect,
        Hello,
        Goto,
        InvDrop,
        InvResult,
        Reconnect,
        MapInfo,
        Load,
        Teleport,
        UsePortal,
        Death,
        Buy,
        BuyResult,
        Aoe,
        PlayerHit,
        EnemyHit,
        AoeAck,
        ShootAck,
        SquareHit,
        EditAccountList,
        AccountList,
        QuestObjId,
        CreateGuild,
        GuildResult,
        GuildRemove,
        GuildInvite,
        AllyShoot,
        EnemyShoot,
        Escape,
        InvitedToGuild,
        JoinGuild,
        ChangeGuildRank,
        PlaySound,
        Reskin,
        GotoAck,
        TradeRequest,
        TradeRequested,
        TradeStart,
        ChangeTrade,
        TradeChanged,
        CancelTrade,
        TradeDone,
        AcceptTrade,
        TradeAccepted,
        SwitchMusic
    }
    
    public partial class TcpClient
    {
        private readonly struct Packet
        {
            public readonly PacketId Id;
            public readonly byte[] Body;

            public Packet(PacketId id, byte[] body)
            {
                Id = id;
                Body = body;
            }
        }
        
        private static readonly ConcurrentQueue<Packet> _ToBeHandled = new ConcurrentQueue<Packet>();
        private Dictionary<PacketId, Action<PacketReader>> _packetHandlers;

        [SerializeField]
        private Map _map;

        // [SerializeField]
        // private EntityManager _entityManager;

        private void Awake()
        {
            _packetHandlers = new Dictionary<PacketId, Action<PacketReader>>
            {
                { PacketId.MapInfo, MapInfo },
                { PacketId.AccountList, AccountList },
                { PacketId.CreateSuccess, CreateSuccess },
                { PacketId.Update, OnUpdate },
                { PacketId.NewTick, NewTick }
            };
        }

        private void Update()
        {
            while (_ToBeHandled.TryDequeue(out var packet))
            {
                Handle(packet);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ScreenManager.Instance.ChangeScreen(Screen.Character);
            }
        }

        private void Handle(Packet packet)
        {
            Debug.Log(packet.Id);

            //TODO might be able to make data parsing done on ticking task
            using (var rdr = new PacketReader(new MemoryStream(packet.Body)))
            {
                _packetHandlers[packet.Id].Invoke(rdr);
            }
        }

        private static void SendHello(int gameId, string username, string password)
        {
            using (var wtr = new PacketWriter(new MemoryStream()))
            {
                wtr.Write((byte)PacketId.Hello);
                wtr.Write("");
                wtr.Write(gameId);
                wtr.Write(username);
                wtr.Write(password);
                wtr.Write(0);
                wtr.Write(new byte[0]);
                _Pending.Enqueue((wtr.BaseStream as MemoryStream).ToArray());
            }
        }

        private static void SendLoad(int charId)
        {
            using (var wtr = new PacketWriter(new MemoryStream()))
            {
                wtr.Write((byte)PacketId.Load);
                wtr.Write(charId);
                _Pending.Enqueue((wtr.BaseStream as MemoryStream).ToArray());
            }
        }

        private void MapInfo(PacketReader rdr)
        {
            if (Account.GameInitData.NewCharacter)
            {
                        
            }
            else
            {
                SendLoad(Account.GameInitData.CharId); 
            }
        }

        private void AccountList(PacketReader rdr)
        {
            
        }

        private void CreateSuccess(PacketReader rdr)
        {
            
        }

        private void OnUpdate(PacketReader rdr)
        {
            var count = rdr.ReadInt16();
            while (count > 0)
            {
                _map.AddTile(new TileData(rdr));
                count--;
            }
            
            count = rdr.ReadInt16();
            while (count > 0)
            {
                _map.AddObject(new ObjectDefinition(rdr));
                count--;
            }
            
            count = rdr.ReadInt16();
            while (count > 0)
            {
                _map.RemoveObject(new ObjectDrop(rdr));
                count--;
            }
        }

        private void NewTick(PacketReader rdr)
        {
            
        }
    }
}