using BattleShip.WPF.Models;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для MultiplayerConnectionPage.xaml
    /// </summary>
    public partial class MultiplayerConnectionPage : Page
    {
        public MultiplayerConnectionPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Game.ConnectionManager.ConnectTo(IPAddress.Parse("192.168.52.1"), 5050);
                await Game.ConnectionManager.SendConnectionMessage($"Login: {LoginData.Text} Password: {PasswordData.Text}");
                ConnectBtn.IsEnabled = false;
                await Game.ConnectionManager.RunOnlineRecive();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Game.ConnectionManager.Client?.Connected == true)
            {
                PageHelper.GoToPage(PageType.SetUp);
            }
            else
            {
                MessageBox.Show("Вы не подключены к серверу!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectBtn.IsEnabled = true;
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.GoToPage(PageType.MainMenu);
        }
    }
}
