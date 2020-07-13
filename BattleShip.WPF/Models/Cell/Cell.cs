using System.Windows;
using System.Windows.Controls;

namespace BattleShip.WPF.Models
{
    public class Cell : Button, ICell
    {
        public Cell()
        {
            Background = System.Windows.Media.Brushes.Transparent;
            BorderThickness = new Thickness(0.3);
            BorderBrush = System.Windows.Media.Brushes.Gray;
            Style = (Style)Application.Current.Resources["CellStyle"];
        }
        public Cell(int i, int j)
        {
            Background = System.Windows.Media.Brushes.Transparent;
            BorderThickness = new Thickness(0.3);
            BorderBrush = System.Windows.Media.Brushes.Gray;
            Style = (Style)Application.Current.Resources["CellStyle"];
            I = i;
            J = j;
        }

        public static readonly DependencyProperty CellState = DependencyProperty.Register("CellState", typeof(bool), typeof(Cell), new PropertyMetadata(false));
        public static readonly DependencyProperty CellExplored = DependencyProperty.Register("CellExplored", typeof(bool), typeof(Cell), new PropertyMetadata(false));
        public static readonly DependencyProperty CellSelected = DependencyProperty.Register("CellSelected", typeof(bool), typeof(Cell), new PropertyMetadata(false));
        public bool State
        {
            get
            {
                return (bool)GetValue(CellState);
            }
            set
            {
                SetValue(CellState, value);
            }
        }
        public bool Explored
        {
            get
            {
                return (bool)GetValue(CellExplored);
            }
            set
            {
                SetValue(CellExplored, value);
            }
        }
        public int I { get; set; }
        public int J { get; set; }
        public bool Selected
        {
            get
            {
                return (bool)GetValue(CellSelected);
            }
            set
            {
                SetValue(CellSelected, value);
            }
        }
       
        public static bool GetCellState(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellState);
        }
        public static void SetCellState(DependencyObject obj, bool value)
        {
            obj.SetValue(CellState, value);
        }
        public static bool GetCellExplored(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellExplored);
        }
        public static void SetCellExplored(DependencyObject obj, bool value)
        {
            obj.SetValue(CellExplored, value);
        }
        public static bool GetCellSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellSelected);
        }
        public static void SetCellSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(CellSelected, value);
        }
    }
}
