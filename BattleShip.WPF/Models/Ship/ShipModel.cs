namespace BattleShip.WPF.Models
{
    public class ShipModel : IShip
    {
        public int Length { get; set; }
        public int Health { get; set; }
        public Orientation Orientation { get; set; }
        public int I { get; set; }
        public int J { get; set; }
        public AttackResult Attack()
        {
            return (--Health == 0) ? AttackResult.Destroy : AttackResult.Hit;
        }
        public ShipModel(int health)
        {
            Length = health;
            Health = health;
        }
        public ShipModel(int health, Orientation orientation, int i = 0, int j = 0)
        {
            Length = health;
            Health = health;
            Orientation = orientation;
            I = i;
            J = j;
        }
        public bool IsIntersectWith(IShip ship)
        {
            int a_min_i = I - 1;
            int a_min_j = J - 1;
            int a_max_i = (Orientation == Orientation.Vertical) ? I + Length : I + 1;
            int a_max_j = (Orientation == Orientation.Horizontal) ? J + Length : J + 1;

            int b_min_i = ship.I;
            int b_min_j = ship.J;
            int b_max_i = (ship.Orientation == Orientation.Vertical) ? ship.I + ship.Length - 1 : ship.I;
            int b_max_j = (ship.Orientation == Orientation.Horizontal) ? ship.J + ship.Length - 1 : ship.J;

            for (int a_i = a_min_i; a_i <= a_max_i; a_i++)
            {
                for (int a_j = a_min_j; a_j <= a_max_j; a_j++)
                {
                    for (int b_i = b_min_i; b_i <= b_max_i; b_i++)
                    {
                        for (int b_j = b_min_j; b_j <= b_max_j; b_j++)
                        {
                            if (a_i == b_i && a_j == b_j)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
