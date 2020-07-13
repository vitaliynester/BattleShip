using BattleShip.WPF.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BattleShip.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для CoopConnetionPage.xaml
    /// </summary>
    public partial class CoopConnetionPage : Page
    {
        public CoopConnetionPage()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var item in localIP)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    ServerIpAddress.Text = item.ToString();
                }
            }
            ConnectIpAddress.Text = ServerIpAddress.Text;
        }

        private async void CreateServerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await Game.ConnectionManager.StartServer(IPAddress.Parse(ServerIpAddress.Text), ServerPort.Text))
                {
                    ConnectionLog.AppendText("Сервер запущен! Ожидание игрока...", Brushes.Green);
                }
                CreateServerBtn.IsEnabled = false;
                ConnectButton.IsEnabled = false;

                await WaitForOpponent();

                Game.status.IsHost = true;
                Game.status.IsMyTurn = true;

                await Game.ConnectionManager.RunRecive();

                ContinueButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                CreateServerBtn.IsEnabled = true;
                ConnectButton.IsEnabled = true;
                ConnectionLog.AppendText(ex);
            }
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Game.ConnectionManager.ConnectTo(IPAddress.Parse(ConnectIpAddress.Text), Convert.ToInt32(ConnectPort.Text));

                ConnectionLog.AppendText($"Подключено!\n" +
                                         $"Локальная позиция : {Game.ConnectionManager.Client.LocalEndPoint}\n" +
                                         $"Удаленная позиция: {Game.ConnectionManager.Listener.RemoteEndPoint}", Brushes.Green);

                Game.status.IsHost = false;
                Game.status.IsMyTurn = false;

                await Game.ConnectionManager.RunRecive();

                CreateServerBtn.IsEnabled = false;
                ConnectButton.IsEnabled = false;

                ContinueButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                CreateServerBtn.IsEnabled = true;
                ConnectButton.IsEnabled = true;

                ConnectionLog.AppendText(ex);
            }
        }

        private async Task WaitForOpponent()
        {
            try
            {
                await Game.ConnectionManager.WaitServerForUsers();
                ConnectionLog.AppendText($"Пользователь подключен!\n" +
                                             $"Локальная позиция : {Game.ConnectionManager.Client.LocalEndPoint}\n" +
                                             $"Удаленная позиция: {Game.ConnectionManager.Listener.RemoteEndPoint}", Brushes.Green);
                ConnectionLog.ScrollToEnd();
            }
            catch (Exception ex)
            {
                ConnectionLog.AppendText(ex);
            }
        }

        private bool IsValidIp(string ip)
        {
            Regex ipPattern = new Regex(@"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$");
            return ipPattern.IsMatch(ip);
        }

        private bool IsValidPort(string port)
        {
            Regex portPattern = new Regex(@"^()([1-9]|[1-5]?[0-9]{2,4}|6[1-4][0-9]{3}|65[1-4][0-9]{2}|655[1-2][0-9]|6553[1-5])$");
            return portPattern.IsMatch(port);
        }

        private void ServerIpAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsValidIp(ServerIpAddress.Text))
            {
                ServerIpAddress.Foreground = (Brush)Application.Current.Resources["BluePrimaryBrush"];
                CreateServerBtn.IsEnabled = true;
            }
            else
            {
                ServerIpAddress.Foreground = Brushes.Red;
                CreateServerBtn.IsEnabled = false;
            }
        }

        private void ConnectIpAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsValidIp(ConnectIpAddress.Text))
            {
                ConnectIpAddress.Foreground = (Brush)Application.Current.Resources["BluePrimaryBrush"];
                if (ConnectButton != null)
                {
                    ConnectButton.IsEnabled = true;
                }                
            }
            else
            {
                ConnectIpAddress.Foreground = Brushes.Red;
                if (ConnectButton != null)
                {
                    ConnectButton.IsEnabled = false;
                }               
            }
        }

        private void ServerPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsValidPort(ServerPort.Text))
            {
                ServerPort.Foreground = (Brush)Application.Current.Resources["BluePrimaryBrush"];
                if (CreateServerBtn != null)
                {
                    CreateServerBtn.IsEnabled = true;
                }                
            }
            else
            {
                ServerPort.Foreground = Brushes.Red;
                if (CreateServerBtn != null)
                {
                    CreateServerBtn.IsEnabled = false;
                }                
            }
        }

        private void ConnectPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsValidPort(ConnectPort.Text))
            {
                ConnectPort.Foreground = (Brush)Application.Current.Resources["BluePrimaryBrush"];
                if (ConnectButton != null)
                {
                    ConnectButton.IsEnabled = true;
                }                
            }
            else
            {
                ConnectPort.Foreground = Brushes.Red;
                if (ConnectButton != null)
                {
                    ConnectButton.IsEnabled = false;
                }                
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Game.ConnectionManager?.CloseAllConnections();
            PageHelper.GoToPage(PageType.MainMenu);
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.GoToPage(PageType.SetUp);
        }
    }
}
