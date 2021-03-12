using System.IO;

namespace Networking
{
    public static partial class TcpClient
    {
        public static void SendHello(int gameId, string username, string password)
        {
            using (var wtr = new PacketWriter(new MemoryStream()))
            {
                wtr.Write((byte)PacketId.Hello);
                wtr.Write("");
                wtr.Write(gameId);
                wtr.Write(username);
                wtr.Write(password);
                wtr.Write(0);
                wtr.Write(new byte[0]);
                _Pending.Enqueue((wtr.BaseStream as MemoryStream).ToArray());
            }
        }
    }
}