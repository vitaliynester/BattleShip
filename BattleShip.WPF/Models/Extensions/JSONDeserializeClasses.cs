using System.Collections.Generic;

namespace BattleShip.WPF.Models
{
    public class PairInfo
    {
        public PlayerInfo FirstPlayer { get; set; }
        public PlayerInfo SecondPlayer { get; set; }
    }
    public class PlayerInfo
    {
        public string Name { get; set; }
        public int TotalGame { get; set; }
        public double Winrate { get; set; }
        public double Accuracy { get; set; }
    }
    public class StepInfo
    {
        public string NamePlayer { get; set; }
        public string PlaceShoot { get; set; }
        public string Result { get; set; }
        public int ShipsLeftFirstPlayer { get; set; }
        public int ShipsLeftSecondPlayer { get; set; }
    }
    public class ReportData
    {
        public PairInfo Pair { get; set; }
        public List<StepInfo> Steps { get; set; }
    }
}
