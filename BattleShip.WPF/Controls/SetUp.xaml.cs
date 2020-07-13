using BattleShip.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BattleShip.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для SetUp.xaml
    /// </summary>
    public partial class SetUp : Page
    {
        public SetUp()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            myField.Reset();
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            myField.SetRandom();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            myField.DeleteSelectedShip();
        }

        private async void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            Game.status.IsIReady = true;
            await Game.opponent.SendGameMessage(GameMessageType.Ready);
            PageHelper.GoToPage(PageType.FightVsAI, myField);
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            await Game.opponent.SendGameMessage(GameMessageType.End);
            Game.ConnectionManager.CloseAllConnections();
            PageHelper.GoToPage(PageType.MainMenu);
        }
    }
}
