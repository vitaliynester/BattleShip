using BattleShip.WPF.Models;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для FightResult.xaml
    /// </summary>
    public partial class FightResult : Page
    {
        public FightResult()
        {
            InitializeComponent();
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            Game.ConnectionManager.CloseAllConnections();
            PageHelper.GoToPage(PageType.MainMenu);
        }
    }
}
