using System;
using System.Windows;

namespace BattleShip.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для ReportView.xaml
    /// </summary>
    public partial class ReportView : Window
    {
        public string Path { get; set; }
        public ReportView(string path)
        {
            InitializeComponent();
            Path = path;
            Browser.Navigate(new Uri(Path));
        }
    }
}
