namespace Networking.Packets.Outgoing
{
    public class Hello : OutgoingPacket
    {
        public override PacketId Id => PacketId.Hello;
        
        private readonly int _gameId;
        private readonly string _username;
        private readonly string _password;

        public Hello(int gameId, string username, string password)
        {
            _gameId = gameId;
            _username = username;
            _password = password;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write("");
            wtr.Write(_gameId);
            wtr.Write(_username);
            wtr.Write(_password);
            wtr.Write(0);
        }
    }
}