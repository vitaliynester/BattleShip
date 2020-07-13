using System.Threading.Tasks;

namespace BattleShip.WPF.Models
{
    public abstract class Opponent
    {
        public abstract Task SendAttackRequest(int X, int Y);
        public virtual void SendAttackResult(AttackMessage msg) { }
        public virtual async Task SendGameMessage(GameMessageType message) { }
        public virtual async Task GiveTurn() { Game.status.ReverseTurn(); }
    }
}
