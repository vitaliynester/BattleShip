using BattleShip.WPF.Models;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для DataFromDB.xaml
    /// </summary>
    public partial class DataFromDB : Page
    {
        public DataFromDB()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void BtnDataGen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Game.ConnectionManager.ConnectTo(IPAddress.Parse("192.168.52.1"), 5050);
                await Game.ConnectionManager.SendConnectionMessage($"DataLogin: {tbName.Text}");

                await Game.ConnectionManager.RunReciveJSON();
                lblError.Content = Game.ConnectionManager.ErrorMessage;
                if (Game.ConnectionManager.ErrorMessage == string.Empty && Game.ConnectionManager.PathReport != null)
                {
                    var report_view = new ReportView(Game.ConnectionManager.PathReport);
                    report_view.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.GoToPage(PageType.MainMenu);
        }
    }
}
