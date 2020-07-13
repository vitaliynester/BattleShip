using BattleShip.WPF.Models;
using System.Windows.Controls;

namespace BattleShip.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для FightVsAI.xaml
    /// </summary>
    public partial class FightVsAI : Page
    {
        public FightVsAI()
        {
            InitializeComponent();
        }
        public FightVsAI(MyField field)
        {
            InitializeComponent();
            myField.CopyInfo(field);

            Game.session.myField = myField;
            Game.session.enemyField = enemyField;
        }
    }
}
