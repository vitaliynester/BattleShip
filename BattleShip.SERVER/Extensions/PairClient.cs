namespace BattleShip.SERVER
{
    public class PairClient
    {
        public ConnectedClient client_one;
        public ConnectedClient client_two;
        public int match_id = -1;
        public PairClient(ConnectedClient connectedClient_one, ConnectedClient connectedClient_two)
        {
            client_one = connectedClient_one;
            client_two = connectedClient_two;
        }
    }
}
