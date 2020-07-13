using System.Windows;

namespace BattleShip.WPF.Models
{
    public class EnemyField : Field
    {
        public EnemyField()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    cellField[i, j].Click += EnemyField_Click;
                }
            }
        }

        private async void EnemyField_Click(object sender, RoutedEventArgs e)
        {
            Cell cell = (Cell)sender;
            if (!Game.status.IsMyTurn || cell.Explored || !Game.status.IsOpponentReady)
            {
                return;
            }
            await Game.opponent.SendAttackRequest(cell.I, cell.J);
        }
        public void ReactToAttackResult(AttackMessage atckMsg)
        {
            ICell cell = cellField[atckMsg.X, atckMsg.Y];
            cell.Explored = true;

            switch (atckMsg.Result)
            {
                case AttackResult.Hit:
                    cell.State = true;
                    break;
                case AttackResult.Miss:
                    cell.State = false;
                    break;
                case AttackResult.Destroy:
                    cell.State = true;
                    UnsettedShipsByLength[atckMsg.Ship.Length - 1].Value--;
                    ShipDestroyed(atckMsg.Ship);
                    break;
                default:
                    break;
            }
        }
    }
}
