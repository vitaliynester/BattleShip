using System.Net.Sockets;

namespace BattleShip.SERVER
{
    public class ConnectedClient
    {
        public TcpClient Client { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public int ShipsLeft { get; set; }
        public ConnectedClient(TcpClient client, string name, string hash)
        {
            Client = client;
            Name = name;
            Hash = hash;
            ShipsLeft = 10;
        }
    }
}
