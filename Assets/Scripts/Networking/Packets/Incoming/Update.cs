using Game;
using Game.Entities;
using Game.MovementControllers;
using Models;
using UnityEngine;

namespace Networking.Packets.Incoming
{
    public class Update : IncomingPacket
    {
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

            foreach (var add in _adds)
            {
                var desc = AssetLibrary.GetObjectDesc(add.ObjectType);
                var prefab = Resources.Load<Entity>($"Entities/{desc.Class}"); //TODO should probably cache these
                var entity = map.CreateEntity(prefab); //TODO add pooling class
                entity.Init(add, map);
                
                map.AddObject(entity, add.ObjectStatus.Position);

                var isMyPlayer = false;
                if (entity.ObjectId == handler.PlayerId)
                {
                    handler.Player = entity as Player;
                    handler.Player!.Random = handler.Random;
                    isMyPlayer = true;
                }
                
                entity.AddControllers(isMyPlayer);
            }

            foreach (var drop in _drops)
            {
                map.RemoveObject(drop);
            }
        }
    }
}