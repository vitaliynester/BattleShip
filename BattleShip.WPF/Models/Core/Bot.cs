using System;
using System.Threading;
using System.Windows;

namespace BattleShip.WPF.Models
{
    public class Bot
    {
        public MyField myField;
        private EnemyField enemyField;

        public Bot()
        {
            myField = new MyField();
            enemyField = new EnemyField();
            myField.SetRandom();
            Game.status.IsOpponentReady = true;
        }

        private void RemovePoints()
        {
            if (enemyField.UnsettedShipsByLength[0].Value > 0)
            {
                return;
            }

            int length = 2;

            while (length < 4)
            {
                if (enemyField.UnsettedShipsByLength[length - 1].Value > 0)
                {
                    break;
                }
                length++;
            }

            var map = new byte[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (enemyField.UnsettedShipsByLength[length - 1].Value <= 0)
                    {
                        continue;
                    }
                    if (enemyField.CanSetShip(new ShipModel(length, Orientation.Horizontal,i,j)))
                    {
                        for (int k = j; k < Math.Min(j + length, 9); k++)
                        {
                            map[i, k] = 1;
                        }
                    }
                    if (enemyField.CanSetShip(new ShipModel(length, Orientation.Horizontal, i, j)))
                    {
                        for (int k = i; k < Math.Min(i + length, 9); k++)
                        {
                            map[k, j] = 1;
                        }
                    }
                }
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (map[i, j] == 0)
                    {
                        enemyField[i, j].Explored = true;
                    }
                }
            }
        }
        private Point GuessPointSmart()
        {
            Point p = new Point();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (enemyField[i, j].Explored) 
                    {
                        if (enemyField[Math.Min(i + 1, 9), j].Explored && 
                            enemyField[Math.Max(0, i - 1), j].Explored && 
                            enemyField[i, Math.Min(j + 1, 9)].Explored && 
                            enemyField[i, Math.Max(0, j - 1)].Explored)
                        {
                            continue;
                        }
                    }
                    if (enemyField[i, j].State) 
                    {
                        if (i + 1 < 10 && !enemyField[i + 1, j].Explored)
                        {
                            p = new Point(i + 1, j);
                        }
                        if (i - 1 >= 0 && !enemyField[i - 1, j].Explored)
                        {
                            p = new Point(i - 1, j);
                        }
                        if (j + 1 < 10 && !enemyField[i, j + 1].Explored)
                        {
                            p = new Point(i, j + 1);
                        }
                        if (j - 1 >= 0 && !enemyField[i, j - 1].Explored)
                        {
                            p = new Point(i, j - 1);
                        }



                        if (i + 1 < 10 && 
                            !enemyField[i + 1, j].Explored && 
                            i - 1 >= 0 && 
                            enemyField[i - 1, j].State)
                        {
                            return new Point(i + 1, j);
                        }
                        if (i - 1 >= 0 && 
                            !enemyField[i - 1, j].Explored && 
                            i + 1 < 10 && 
                            enemyField[i + 1, j].State)
                        {
                            return new Point(i - 1, j);
                        }
                        if (j + 1 < 10 && 
                            !enemyField[i, j + 1].Explored && 
                            j - 1 >= 0 && 
                            enemyField[i, j - 1].State)
                        {
                            return new Point(i, j + 1);
                        }
                        if (j - 1 >= 0 && 
                            !enemyField[i, j - 1].Explored && 
                            j + 1 < 10 && 
                            enemyField[i, j + 1].State)
                        {
                            return new Point(i, j - 1);
                        }
                    }
                }
            }
            RemovePoints();

            if (enemyField[(int)p.X, (int)p.Y].Explored)
            {
                int x, y;
                Random r = new Random();
                do
                {
                    x = r.Next(10);
                    y = r.Next(10);
                    p.X = x;
                    p.Y = y;
                } while (enemyField[x, y].Explored);
            }

            return p;
        }
        public async void AttackAsync(object o)
        {
            while (!Game.status.IsMyTurn && Game.status.State != GameState.End)
            {
                Thread.Sleep(350);
                int X, Y;
                Point p = new Point();
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    p = GuessPointSmart();
                }));

                X = (int)p.X;
                Y = (int)p.Y;

                bool ifExplored = false;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ifExplored = enemyField[X, Y].Explored;
                }));

                if (ifExplored)
                {
                    continue;
                }

                AttackMessage a_msg;
                a_msg.Result = AttackResult.Miss;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    a_msg = Game.session.myField.GetAttackOnFieldResult(X, Y);
                    enemyField.ReactToAttackResult(a_msg);
                }));

                if (a_msg.Result == AttackResult.Miss)
                {
                    Game.status.ReverseTurn();
                }
            }
        }
    }
}
