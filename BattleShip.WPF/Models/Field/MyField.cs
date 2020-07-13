using System;
using System.Windows;
using System.Windows.Media;

namespace BattleShip.WPF.Models
{
    public class MyField : Field
    {
        private IShip SelectedShip = null;
        public MyField()
        {
            Action<Cell> setDefaultEventHandlersToCell = delegate (Cell cell)
            {
                cell.DragEnter += MyField_DragEnter;
                cell.DragLeave += MyField_DragLeave;
                cell.Drop += MyField_Drop;
                cell.Click += Cell_Click;
            };
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    setDefaultEventHandlersToCell(cellField[i, j]);
                }
            }
        }
        protected IShip GetShipByIJCoordinates(int i, int j)
        {
            foreach (var item in placedShips)
            {
                if (item.I == i && item.Orientation == Orientation.Horizontal && (item.J <= j && j < item.J + item.Length))
                {
                    return item;
                }
                if (item.J == j && item.Orientation == Orientation.Vertical && (item.I <= i && i < item.I + item.Length))
                {
                    return item;
                }
            }
            return null;
        }
        public void DeleteSelectedShip()
        {
            if (SelectedShip == null) 
            {
                return;
            }

            switch (SelectedShip.Orientation)
            {
                case Orientation.Horizontal:
                    for (int j = SelectedShip.J; j < SelectedShip.J + SelectedShip.Length; j++)
                    {
                        this[SelectedShip.I, j].State = false;
                        this[SelectedShip.I, j].Background = Brushes.Transparent;
                    }
                    break;
                case Orientation.Vertical:
                    for (int i = SelectedShip.I; i < SelectedShip.I + SelectedShip.Length; i++)
                    {
                        this[i, SelectedShip.J].State = false;
                        this[i, SelectedShip.J].Background = Brushes.Transparent;

                    }
                    break;
                default:
                    break;
            }
            placedShips.Remove(SelectedShip);
            UnsettedShipsByLength[SelectedShip.Length - 1].Value++;
            SelectedShip = null;
        }
        public bool TrySetShip(IShip ship, int i, int j)
        {
            ship.I = i;
            ship.J = j;

            if (!CanSetShip(ship))
            {
                return false;
            }
            placedShips.Add(new ShipModel(ship.Length, ship.Orientation, ship.I, ship.J));

            switch (ship.Orientation)
            {
                case Orientation.Horizontal:
                    for (int _j = j; _j < j + ship.Length; _j++)
                    {
                        cellField[i, _j].State = true;
                    }
                    break;
                case Orientation.Vertical:
                    for (int _i = i; _i < i + ship.Length; _i++)
                    {
                        cellField[_i, j].State = true;
                    }
                    break;
                default:
                    break;
            }
            UnsettedShipsByLength[ship.Length - 1].Value--;
            return true;
        }
        public void CopyInfo(MyField field)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    cellField[i, j].State = field[i, j].State;
                }
            }
            placedShips = field.placedShips;
        }
        public void Reset()
        {
            UnsettedShipsByLength[0].Value = 4;
            UnsettedShipsByLength[1].Value = 3;
            UnsettedShipsByLength[2].Value = 2;
            UnsettedShipsByLength[3].Value = 1;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    cellField[i, j].State = false;
                    cellField[i, j].Explored = false;
                    cellField[i, j].Background = Brushes.Transparent;
                }
            }
            placedShips.Clear();

        }
        public void SetRandom()
        {
            Reset();
            for (int ii = 3; ii >= 0; ii--)
            {
                while (UnsettedShipsByLength[ii].Value > 0)
                {
                    Random r = new Random();
                    int i, j, o;
                    Orientation orientation;
                    IShip ship;
                    do
                    {
                        i = r.Next(10);
                        j = r.Next(10);
                        o = r.Next(2);
                        orientation = (Orientation)o;

                        ship = new ShipModel(ii + 1);
                        ship.I = i;
                        ship.J = j;
                        ship.Orientation = orientation;

                    } while (!TrySetShip(ship, i, j));
                }
            }
        }
        public AttackMessage GetAttackOnFieldResult(int i, int j)
        {
            cellField[i, j].Explored = true;
            IShip ship = GetShipByIJCoordinates(i, j);
            if (ship == null)
            {
                return new AttackMessage(AttackResult.Miss, i, j);
            }
            else
            {
                if (ship.Attack() == AttackResult.Destroy)
                {
                    ShipDestroyed(ship);
                    return new AttackMessage(AttackResult.Destroy, i, j, ship);
                }
                else
                {
                    return new AttackMessage(AttackResult.Hit, i, j);
                }
            }
        }
        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            Cell cell = (Cell)sender;
            if (SelectedShip != null)
            {
                switch (SelectedShip.Orientation)
                {
                    case Orientation.Horizontal:
                        for (int j = SelectedShip.J; j < SelectedShip.J + SelectedShip.Length; j++)
                        {
                            this[SelectedShip.I, j].Background = Brushes.Transparent;
                        }
                        break;
                    case Orientation.Vertical:
                        for (int i = SelectedShip.I; i < SelectedShip.I + SelectedShip.Length; i++)
                        {
                            this[i, SelectedShip.J].Background = Brushes.Transparent;
                        }
                        break;
                    default:
                        break;
                }
            }
            SelectedShip = GetShipByIJCoordinates(cell.I, cell.J);

            if (SelectedShip != null)
            {
                switch (SelectedShip.Orientation)
                {
                    case Orientation.Horizontal:
                        for (int j = SelectedShip.J; j < SelectedShip.J + SelectedShip.Length; j++)
                        {
                            this[SelectedShip.I, j].Background = Brushes.Green;
                        }
                        break;
                    case Orientation.Vertical:
                        for (int i = SelectedShip.I; i < SelectedShip.I + SelectedShip.Length; i++)
                        {
                            this[i, SelectedShip.J].Background = Brushes.Green;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private void MyField_Drop(object sender, DragEventArgs e)
        {
            Cell cell = (Cell)sender;
            ShipVisual ship = (ShipVisual)e.Data.GetData("Ship");

            TrySetShip(ship, cell.I, cell.J);

            MyField_DragLeave(sender, e);
        }
        private void MyField_DragLeave(object sender, DragEventArgs e)
        {
            Cell cell = (Cell)sender;
            ShipVisual ship = (ShipVisual)e.Data.GetData("Ship");
            if (ship.Orientation == Orientation.Horizontal)
            {
                for (int j = cell.J; j < cell.J + ship.Length; j++)
                {
                    if (j >= 0 && j < 10)
                    {
                        cellField[cell.I, j].Background = Brushes.Transparent;
                    }
                }
            }
            else
            {
                for (int i = cell.I; i < cell.I + ship.Length; i++)
                {
                    if (i >= 0 && i < 10) 
                    {
                        cellField[i, cell.J].Background = Brushes.Transparent;
                    }
                }
            }
        }
        private void MyField_DragEnter(object sender, DragEventArgs e)
        {
            Cell cell = (Cell)sender;
            ShipVisual ship = (ShipVisual)e.Data.GetData("Ship");
            ship.I = cell.I;
            ship.J = cell.J;
            bool buff = CanSetShip(ship);
            if (ship.Orientation == Orientation.Horizontal)
            {
                for (int j = cell.J; j < cell.J + ship.Length; j++)
                {
                    if (j >= 0 && j < 10) 
                    {
                        cellField[cell.I, j].Background = (buff) ? Brushes.GreenYellow : Brushes.IndianRed;
                    }
                }
            }
            else
            {
                for (int i = cell.I; i < cell.I + ship.Length; i++)
                {
                    if (i >= 0 && i < 10)
                    {
                        cellField[i, cell.J].Background = (buff) ? Brushes.GreenYellow : Brushes.IndianRed;
                    }
                }
            }
        }
    }
}
