using System;
using System.Collections.Concurrent;
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
    
    public class PacketHandler : MonoBehaviour
    {
        private static readonly ConcurrentQueue<Packet> _ToBeHandled = new ConcurrentQueue<Packet>();

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

        public static void Read(Packet packet)
        {
            _ToBeHandled.Enqueue(packet);
        }

        private async void OnEnable()
        {
            await TcpClient.InitAsync();
            TcpClient.SendHello(Account.GameInitData.GameId, Account.Username, Account.Password);
        }

        private async void OnDisable()
        {
            await TcpClient.StopAsync();
        }

        private void Handle(Packet packet)
        {
            Debug.Log(packet.Id);
            
            switch (packet.Id)
            {
                case PacketId.MapInfo:
                    if (Account.GameInitData.NewCharacter)
                    {
                        
                    }
                    else
                    {
                        TcpClient.SendLoad(Account.GameInitData.CharId); 
                    }
                    break;
            }
        }
    }
}