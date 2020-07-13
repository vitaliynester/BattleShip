namespace BattleShip.WPF.Models
{
    public struct AttackMessage
    {
        public AttackResult Result;
        public int X;
        public int Y;
        public IShip Ship;
        public AttackMessage(AttackResult result, int x, int y)
        {
            Result = result;
            X = x;
            Y = y;
            Ship = null;
        }
        public AttackMessage(AttackResult result, int x, int y, IShip ship)
        {
            Result = result;
            X = x;
            Y = y;
            Ship = ship;
        }
    }
}
