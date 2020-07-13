using BattleShip.WPF.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip.WPF.Models
{
    public enum PageType
    {
        MainMenu,
        CreateRoom,
        SetUp,
        FightVsAI,
        FightResult,
        ReportHistory,
        MultiplayerConnectionPage,
    }
    public static class PageHelper
    {
        private static Frame mainWindowFrame;
        public static Frame Frame
        {
            get
            {
                return mainWindowFrame;
            }
            set
            {
                if (value is Frame)
                {
                    mainWindowFrame = value;
                }
            }
        }
        public static void GoToPage(PageType pageType, object Parameters = null)
        {
            if (mainWindowFrame == null)
            {
                MessageBox.Show("Данное окно недоступно!","Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception("Данное окно недоступно!");
            }
            switch (pageType)
            {
                case PageType.MainMenu:
                    mainWindowFrame.Content = new MainMenu();
                    break;
                case PageType.CreateRoom:
                    mainWindowFrame.Content = new CoopConnetionPage();
                    break;
                case PageType.SetUp:
                    mainWindowFrame.Content = new SetUp();
                    break;
                case PageType.FightResult:
                    mainWindowFrame.Content = new FightResult();
                    break;
                case PageType.ReportHistory:
                    mainWindowFrame.Content = new DataFromDB();
                    break;
                case PageType.FightVsAI:
                    mainWindowFrame.Content = new FightVsAI((MyField)Parameters);
                    break;
                case PageType.MultiplayerConnectionPage:
                    mainWindowFrame.Content = new MultiplayerConnectionPage();
                    break;
                default:
                    mainWindowFrame.Content = new MainMenu();
                    MessageBox.Show("Данное окно недоступно!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new Exception("Данное окно недоступно!");
            }
        }
    }
}
