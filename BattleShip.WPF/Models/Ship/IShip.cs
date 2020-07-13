namespace BattleShip.WPF.Models
{
    public enum Orientation
    {
        Horizontal,
        Vertical
    }
    public interface IShip
    {
        int Length { get; set; }
        int Health { get; set; }
        Orientation Orientation { get; set; }
        int I { get; set; }
        int J { get; set; }
        bool IsIntersectWith(IShip ship);
        AttackResult Attack();
    }
}
