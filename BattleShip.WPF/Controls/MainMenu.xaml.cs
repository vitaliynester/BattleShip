using BattleShip.WPF.Models;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Game.ConnectionManager?.CloseAllConnections();
            Application.Current.Shutdown();
        }

        private void PlayVsAIButton_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(GameType.VsAI);
            PageHelper.GoToPage(PageType.SetUp);
        }

        private void PlayWithFriendButton_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(GameType.VsHuman);
            PageHelper.GoToPage(PageType.CreateRoom);
        }

        private void PlayOnlineButton_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(GameType.VsMult);
            PageHelper.GoToPage(PageType.MultiplayerConnectionPage);
        }

        private void ReportHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(GameType.VsMult);
            PageHelper.GoToPage(PageType.ReportHistory);
        }
    }
}
