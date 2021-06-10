using System.Collections.Generic;
using Game;
using Game.Entities;
using Models;
using Models.Static;
using Networking.Packets.Outgoing;
using UnityEngine;

namespace Networking.Packets.Incoming
{
    public class NewTick : IncomingPacket
    {
        public override PacketId Id => PacketId.NewTick;
        public override IncomingPacket CreateInstance() => new NewTick();

        private ObjectStatus[] _objectStats;
        private Dictionary<StatType, object> _playerStats;

        public override void Read(PacketReader rdr)
        {
            _objectStats = new ObjectStatus[rdr.ReadInt16()];
            for (var i = 0; i < _objectStats.Length; i++)
            {
                _objectStats[i] = new ObjectStatus(rdr);
            }

            if (rdr.BaseStream.Position >= rdr.BaseStream.Length)
                return;

            _playerStats = new Dictionary<StatType, object>();
            var playerStatsCount = rdr.ReadByte();
            for (var i = 0; i < playerStatsCount; i++)
            {
                var key = (StatType)rdr.ReadByte();
                _playerStats[key] = ObjectStatus.IsStringStat(key) ?
                    (object) rdr.ReadString() : 
                    rdr.ReadInt32();
            }
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            foreach (var objectStat in _objectStats)
            {
                ProcessObjectStatus(objectStat, handler.PlayerId, map.GetEntity(objectStat.Id));
            }

            if (_playerStats != null)
            {
                handler.Player.UpdateObjectStats(_playerStats);
            }
            
            //TODO might need to moves requested
            TcpTicker.Send(new Move(GameTime.Time, handler.Player.Position));
        }

        private void ProcessObjectStatus(ObjectStatus objectStatus, int playerId, Entity entity)
        {
            var isMyPlayer = objectStatus.Id == playerId;
            if (!isMyPlayer)
            {
                entity.OnNewTick(objectStatus.Position);    
            }
            
            entity.UpdateObjectStats(objectStatus.Stats);

            if (!(entity is Player player))
                return;
            
            //TODO finish xp handling
        }
    }
}