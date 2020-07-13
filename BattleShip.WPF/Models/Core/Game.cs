namespace BattleShip.WPF.Models
{
    public enum GameType
    {
        VsHuman,
        VsAI,
        VsMult
    }
    public class Game
    {
        public static Session session;
        public static Opponent opponent;
        public static GameStatus status;
        public static ConnectionManager ConnectionManager;
        public Game(GameType type)
        {
            session = new Session();
            status = new GameStatus();
            ConnectionManager = new ConnectionManager();

            switch (type)
            {
                case GameType.VsHuman:
                    opponent = new CoopOpponent();
                    break;
                case GameType.VsAI:
                    Game.status.IsHost = true;
                    Game.status.IsMyTurn = true;
                    opponent = new AIOpponent();
                    break;
                case GameType.VsMult:
                    opponent = new MultiPlayerOpponent();
                    break;
                default:
                    break;
            }
        }
    }
}
