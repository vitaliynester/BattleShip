using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleShip.WPF.Models
{
    public class MultiPlayerOpponent : Opponent
    {
        public override async Task SendAttackRequest(int X, int Y)
        {
            
            await Game.ConnectionManager.SendOnlineMessage(MessageType.AttackRequest, new Point(X, Y));
        }
        public override async void SendAttackResult(AttackMessage message)
        {
            await Game.ConnectionManager.SendOnlineMessage(MessageType.AttackResult, message);
        }
        public async override Task SendGameMessage(GameMessageType message)
        {
            await Game.ConnectionManager.SendOnlineMessage(MessageType.Game, message);
        }
        public async override Task GiveTurn()
        {
            await SendGameMessage(GameMessageType.YourTurn);
            await base.GiveTurn();
        }
    }
}
