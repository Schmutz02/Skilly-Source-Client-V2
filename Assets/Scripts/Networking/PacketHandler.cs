using System.Collections.Concurrent;
using Game;
using Game.Entities;
using Models;
using Networking.Packets;
using Networking.Packets.Incoming;
using Networking.Packets.Outgoing;
using UI;

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
    
    public class PacketHandler
    {
        private ConcurrentQueue<IncomingPacket> _toBeHandled;

        public int PlayerId;

        public readonly GameInitData InitData;

        public Player Player;
        
        private readonly Map _map;

        public wRandom Random;

        public PacketHandler(GameInitData initData, Map map)
        {
            InitData = initData;
            _map = map;
            Update.OnMyPlayerJoined += OnMyPlayerJoined;
        }

        private void OnMyPlayerJoined(Player player)
        {
            Player = player;
            player.Random = Random;
        }

        public void Start()
        {
            _toBeHandled = new ConcurrentQueue<IncomingPacket>();
            TcpTicker.Start(this);
            TcpTicker.Send(new Hello(InitData.WorldId, Account.Username, Account.Password));
        }

        public void Stop()
        {
            TcpTicker.Stop();
            // this needs to be unassigned since we create again on reconnect
            // assignments done in Awake() shouldn't be unassigned unless changing scenes
            Update.OnMyPlayerJoined -= OnMyPlayerJoined;
        }

        public void Tick()
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