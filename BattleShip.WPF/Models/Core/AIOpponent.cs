using System.Threading;
using System.Threading.Tasks;

namespace BattleShip.WPF.Models
{
    public class AIOpponent : Opponent
    {
        Bot bot;
        public AIOpponent()
        {
            bot = new Bot();
        }
        public override async Task SendAttackRequest(int X, int Y)
        {
            var a_msg = bot.myField.GetAttackOnFieldResult(X, Y);
            Game.session.enemyField.ReactToAttackResult(a_msg);

            if (a_msg.Result == AttackResult.Miss)
            {
                await Game.opponent.GiveTurn();
            }

            if (!Game.status.IsMyTurn)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(bot.AttackAsync));
            }
        }
    }
}
