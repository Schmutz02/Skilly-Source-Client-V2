using Game;
using Models;

namespace Networking.Packets.Incoming
{
    public class Update : IncomingPacket
    {
        public override PacketId Id => PacketId.Update;
        
        private TileData[] _tiles;
        private ObjectDefinition[] _adds;
        private ObjectDrop[] _drops;

        public override IncomingPacket CreateInstance() => new Update();

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
                map.AddObject(add);
            }

            foreach (var drop in _drops)
            {
                map.RemoveObject(drop);
            }
        }
    }
}