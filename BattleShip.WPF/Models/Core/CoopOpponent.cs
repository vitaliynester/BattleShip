using System.Threading.Tasks;
using System.Windows;

namespace BattleShip.WPF.Models
{
    public class CoopOpponent : Opponent
    {
        public override async Task SendAttackRequest(int X, int Y)
        {
            await Game.ConnectionManager.SendMessage(MessageType.AttackRequest, new Point(X, Y));
        }
        public override async void SendAttackResult(AttackMessage message)
        {
            await Game.ConnectionManager.SendMessage(MessageType.AttackResult, message);
        }
        public async override Task SendGameMessage(GameMessageType message)
        {
            await Game.ConnectionManager.SendMessage(MessageType.Game, message);
        }
        public async override Task GiveTurn()
        {
            await SendGameMessage(GameMessageType.YourTurn);
            await base.GiveTurn();
        }
    }
}
