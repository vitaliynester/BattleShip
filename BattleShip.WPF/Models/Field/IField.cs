namespace BattleShip.WPF.Models
{
    public interface IField
    {
        void ShipDestroyed(IShip ship);
        void SetBorderAfterDestroy(IShip ship);
    }
}
