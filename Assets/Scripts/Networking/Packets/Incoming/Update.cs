using System;
using Game;
using Game.Entities;
using Models;

namespace Networking.Packets.Incoming
{
    public class Update : IncomingPacket
    {
        public static Action<Player> OnMyPlayerJoined;
        
        public override PacketId Id => PacketId.Update;
        public override IncomingPacket CreateInstance() => new Update();
        
        private TileData[] _tiles;
        private ObjectDefinition[] _adds;
        private ObjectDrop[] _drops;

        public override void Read(PacketReader rdr)
        {
            _tiles = new TileData[rdr.ReadInt16()];
            for (var i = 0; i < _tiles.Length; i++)
            {
                _tiles[i] = new TileData(rdr);
            }

            _adds = new ObjectDefinition[rdr.ReadInt16()];
            for (var i = 0; i < _adds.Length; i++)
            {
                _adds[i] = new ObjectDefinition(rdr);
            }
            
            _drops = new ObjectDrop[rdr.ReadInt16()];
            for (var i = 0; i < _drops.Length; i++)
            {
                _drops[i] = new ObjectDrop(rdr);
            }
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            foreach (var tile in _tiles)
            {
                map.AddTile(tile);
            }
            
            foreach (var drop in _drops)
            {
                map.RemoveObject(drop.Id);
            }

            foreach (var add in _adds)
            {
                var isMyPlayer = add.ObjectStatus.Id == handler.PlayerId;
                var entity = Entity.Resolve(add.ObjectType, add.ObjectStatus.Id, isMyPlayer, map);

                map.AddObject(entity, add.ObjectStatus.Position);

                if (entity.ObjectId == handler.PlayerId)
                {
                    OnMyPlayerJoined?.Invoke(entity as Player);
                }

                entity.UpdateObjectStats(add.ObjectStatus.Stats);
            }
        }
    }
}