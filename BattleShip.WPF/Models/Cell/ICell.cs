namespace BattleShip.WPF.Models
{
    public interface ICell
    {
        bool State { get; set; }
        bool Explored { get; set; }
        int I { get; set; }
        int J { get; set; }
    }
}
