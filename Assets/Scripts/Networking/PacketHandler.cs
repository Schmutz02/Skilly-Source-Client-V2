using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Game;
using Game.Entities;
using Models;
using Networking.Packets;
using Networking.Packets.Outgoing;
using UI;
using UnityEngine;

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
    
    public class PacketHandler : MonoBehaviour
    {
        private ConcurrentQueue<IncomingPacket> _toBeHandled;

        [HideInInspector]
        public int PlayerId;

        [HideInInspector]
        public int CharId;

        [HideInInspector]
        public Player Player;

        [SerializeField]
        private Map _map;

        // [SerializeField]
        // private EntityManager _entityManager;

        private void OnEnable()
        {
            _toBeHandled = new ConcurrentQueue<IncomingPacket>();
            TcpTicker.Start(this);
            TcpTicker.Send(new Hello(Account.GameInitData.GameId, Account.Username, Account.Password));
        }

        private void OnDisable()
        {
            TcpTicker.Stop();
            _map.Clear();
        }

        private void Update()
        {
            if (!TcpTicker.Running)
            {
                ViewManager.Instance.ChangeView(View.Character);
            }
            
            while (_toBeHandled.TryDequeue(out var packet))
            {
                packet.Handle(this, _map);
            }
        }
        
        public void AddPacket(IncomingPacket packet)
        {
            _toBeHandled.Enqueue(packet);
        }
    }
}