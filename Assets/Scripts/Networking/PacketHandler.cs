using System;
using System.Collections.Concurrent;
using Models;
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
        public static PacketHandler Instance { get; private set; }
        
        private static readonly ConcurrentQueue<Packet> _ToBeHandled = new ConcurrentQueue<Packet>();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            while (_ToBeHandled.TryDequeue(out var packet))
            {
                Debug.Log(packet.Id);
                Debug.Log(packet.Body);
            }
        }

        public void Read(Packet packet)
        {
            _ToBeHandled.Enqueue(packet);
        }

        private void OnEnable()
        {
            TcpClient.Init();
            TcpClient.SendHello(-1, Account.Username, Account.Password);
        }

        private async void OnDisable()
        {
            await TcpClient.StopAsync();
        }
    }
}