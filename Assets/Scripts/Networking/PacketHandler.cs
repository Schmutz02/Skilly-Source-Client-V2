using System.Collections.Concurrent;
using Game;
using Game.Entities;
using Models;
using Networking.Packets;
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
        
        public int CharId;
        private readonly int _worldId;
        public readonly bool NewCharacter;
        
        public Player Player;
        
        private readonly Map _map;

        public wRandom Random;

        public PacketHandler(int worldId, int charId, bool newCharacter, Map map)
        {
            _worldId = worldId;
            CharId = charId;
            NewCharacter = newCharacter;
            _map = map;
        }

        public void Start()
        {
            _toBeHandled = new ConcurrentQueue<IncomingPacket>();
            TcpTicker.Start(this);
            TcpTicker.Send(new Hello(_worldId, Account.Username, Account.Password));
        }

        public void Stop()
        {
            TcpTicker.Stop();
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