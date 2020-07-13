using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip.WPF.Models
{
    public abstract class Field : Grid, IField
    {
        public int ShipsLeft = 10;
        protected Cell[,] cellField;
        public List<IShip> placedShips = new List<IShip>();
        public ObservableCollection<UpdateableType<int>> UnsettedShipsByLength { get; set; } = new ObservableCollection<UpdateableType<int>>
        {
            new UpdateableType<int>(4),
            new UpdateableType<int>(3),
            new UpdateableType<int>(2),
            new UpdateableType<int>(1)
        };
        public Field()
        {
            Style fieldStyle = (Style)Application.Current.Resources["FieldStyle"];
            //итерация по буквам
            for (int i = 0; i < 10; i++)
            {
                TextBox buff = new TextBox()
                {
                    IsReadOnly = true,
                    Style = fieldStyle,
                    FontSize = 20,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Text = ((char)('A' + i)).ToString()
                };
                SetRow(buff, 0);
                SetColumn(buff, i + 1);
                Children.Add(buff);
            }
            //итерация по цифрам
            for (int i = 0; i < 10; i++)
            {
                TextBox buff = new TextBox()
                {
                    IsReadOnly = true,
                    Style = fieldStyle,
                    FontSize = 20,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Text = (i + 1).ToString()
                };
                SetRow(buff, i + 1);
                SetColumn(buff, 0);
                Children.Add(buff);
            }
            //заполняем само поле
            cellField = new Cell[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    cellField[i, j] = new Cell(i, j);
                    SetRow(cellField[i, j], i + 1);
                    SetColumn(cellField[i, j], j + 1);
                    Children.Add(cellField[i, j]);
                }
            }
        }
        public void SetBorderAfterDestroy(IShip ship)
        {
            int min_i, max_i, min_j, max_j;

            min_i = Math.Max(0, ship.I - 1);
            min_j = Math.Max(0, ship.J - 1);
            max_i = (ship.Orientation == Orientation.Horizontal) ? Math.Min(9, ship.I + 1) : Math.Min(9, ship.I + ship.Length);
            max_j = (ship.Orientation == Orientation.Vertical) ? Math.Min(9, ship.J + 1) : Math.Min(9, ship.J + ship.Length);

            for (int i = min_i; i <= max_i; i++)
            {
                for (int j = min_j; j <= max_j; j++)
                {
                    cellField[i, j].Explored = true;
                }
            }
        }
        public bool CanSetShip(IShip ship)
        {
            if (UnsettedShipsByLength[ship.Length - 1].Value <= 0)
            {
                return false;
            }
            if (ship.Orientation == Orientation.Horizontal && ship.J + ship.Length > 10)
            {
                return false;
            }
            if (ship.Orientation == Orientation.Vertical && ship.I + ship.Length > 10)
            {
                return false;
            }
            foreach (var item in placedShips)
            {
                if (ship.IsIntersectWith(item))
                {
                    return false;
                }
            }
            return true;
        }
        public bool AllowDrop
        {
            set
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        cellField[i, j].AllowDrop = value;
                    }
                }
            }
        }
        public Cell this[int i, int j]
        {
            get 
            {
                return cellField[i, j]; 
            }
            set 
            {
                cellField[i, j] = value; 
            }
        }
        public async void ShipDestroyed(IShip ship)
        {
            ShipsLeft--;
            SetBorderAfterDestroy(ship);

            if (ShipsLeft == 0)
            {
                Game.status.Result = (Game.status.IsMyTurn) ? GameResult.Win : GameResult.Loss;
                Game.status.State = GameState.End;
                await Game.opponent.SendGameMessage(GameMessageType.End);
                PageHelper.GoToPage(PageType.FightResult);
            }
        }
    }
}
