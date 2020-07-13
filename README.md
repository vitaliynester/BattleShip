# BattleShip
Морской бой с возможностью игры по сети и онлайн
## Настройка базы данных
В данном проекте используется база данных PostgreSQL для хранения информации об игроках и их матчах. Для удобного создания таблиц в базе данных в данном репозитории имеется файл database.sql, который необходимо выполнить для создания отношений между таблицами в базе данных. После создания отношений необходимо внести информацию в файл BattleShip.SERVER/Database.cs, это необходимо для правильной работы приложения.
## Содержание проекта
```
├───BattleShip.SERVER --серверная часть приложения
│   │   BattleShip.SERVER.csproj
│   │   Database.cs --взаимодействие с базой данных
│   │   Program.cs	--главный файл проекта
│   │
│   └───Extensions	--дополнительные вспомогательные файлы
│           ConnectedClient.cs	--класс описывающий подключенного пользователя
│           JSONDataClasses.cs	--класс для передачи информации из бд клиенту
│           MessageConcat.cs	--описание серверного взаимодействия между клиентами
│           PairClient.cs	--класс описывающий играющую пару игроков
│           ReportDataGenerate.cs	--класс для составление pdf отчета
│
└───BattleShip.WPF	--клиентская часть приложения
    │   App.config
    │   App.xaml
    │   App.xaml.cs
    │   BattleShip.WPF.csproj
    │   icon.ico
    │   MainWindow.xaml
    │   MainWindow.xaml.cs
    │   packages.config
    │
    ├───Assets	--дополнительные файлы
    │   ├───Fonts	--шрифты использующиеся в приложении
    │   │       Coral waves.ttf	--шрифт клиентского приложения
    │   │
    │   ├───Images	--изображения для приложения
    │   │   │   Background.png	--изображения фона в пользовательском приложении
    │   │   │   icon.ico	--иконка приложения
    │   │   │
    │   │   ├───Buttons	--изображения для отображения кнопок
    │   │   │       Circle_button_1.png
    │   │   │       Circle_button_2.png
    │   │   │       Circle_button_3.png
    │   │   │       closeapp.png
    │   │   │       Delete_button.png
    │   │   │       minimize.png
    │   │   │       Random_button.png
    │   │   │       Reset_button.png
    │   │   │
    │   │   ├───Cells	--изображения для отображения клеток на игровом поле
    │   │   │       CrossCell.png
    │   │   │       CrossedShipCell.png
    │   │   │       EmptyCell.png
    │   │   │       ShipCell.png
    │   │   │
    │   │   └───Ships	--изображения для отображения кораблей на поле
    │   │           four_hor.png
    │   │           four_vert.png
    │   │           one_hor.png
    │   │           one_vert.png
    │   │           three_hor.png
    │   │           three_vert.png
    │   │           two_hor.png
    │   │           two_vert.png
    │   │
    │   └───Styles	--стили использующиеся в приложении
    │           ButtonStyle.xaml	--стили для кнопок
    │           CellStyle.xaml	--стили для клетки
    │           CircleButtonStyle.xaml	--стили для круглой кнопки
    │           CountShipStyle.xaml	--стили для описания счетчика кораблей
    │           FieldStyle.xaml	--стили для описания поля
    │           FightResultBoxStyle.xaml	--стили для описания результата сражения
    │           MainStyle.xaml	--основные стили приложения
    │           TextBoxInputStyle.xaml	--стили для элементов ввода информации
    │
    ├───Controls	--папка для пользовательских элементов
    │       CoopConnetionPage.xaml	--пользовательский элемент для совместной игры
    │       CoopConnetionPage.xaml.cs
    │       DataFromDB.xaml	--пользовательский элемент для меню показа отчета из бд
    │       DataFromDB.xaml.cs
    │       FightResult.xaml	--пользовательский элемент для показа результата сражения
    │       FightResult.xaml.cs
    │       FightVsAI.xaml	--пользовательский элемент для игры против компьютера
    │       FightVsAI.xaml.cs
    │       MainMenu.xaml	--пользовательский элемент для главного меню
    │       MainMenu.xaml.cs
    │       MultiplayerConnectionPage.xaml	--пользовательский элемент для многопользовательского режима
    │       MultiplayerConnectionPage.xaml.cs
    │       ReportView.xaml	--пользовательский элемент для открытия pdf отчета
    │       ReportView.xaml.cs
    │       SetUp.xaml	--пользовательский элемент для расстановки кораблей
    │       SetUp.xaml.cs
    │
    ├───Models	--папка описывающая модели используемые в приложении
    │   ├───Cell	--описание модели клетки
    │   │       Cell.cs	--описание функционирования клетки
    │   │       ICell.cs	--интерфейс для описания клетки
    │   │
    │   ├───Core	--основные классы описания взаимодействия
    │   │       AIOpponent.cs	--класс описывающий компьютерного противника
    │   │       Bot.cs	--класс описывающий бота против которого будет происходить игра
    │   │       CoopOpponent.cs	--класс описывающий противника в кооперативном режиме
    │   │       Game.cs	--класс описывающий непосредственно игру
    │   │       GameStatus.cs	--класс описывающий текущий статус игры
    │   │       MultiPlayerOpponent.cs	--класс описывающий противника в многопользовательском режиме
    │   │       Opponent.cs	--класс описывающий противника от которого происходит наследование
    │   │       Session.cs	--класс описывающий текующую игру в кооперативном или многопользовательском режиме
    │   │
    │   ├───Extensions	--дополнительные классы 
    │   │       AttackMessage.cs	--класс для описания модели атаки
    │   │       ConnectionManager.cs	--класс для удобного подключения между игроками
    │   │       JSONDeserializeClasses.cs	--класс для десериализации данных полученных с сервера
    │   │       MessageFormat.cs	--класс для преобразования данных полученных между игроками
    │   │       PageHelper.cs	--класс для переключения между пользовательскими элементами
    │   │       PDFCreater.cs	--класс для создания из данных pdf отчет
    │   │       RichTextBoxExtension.cs	--класс для описания функционала элемента RichTextBox
    │   │
    │   ├───Field	--описание модели поля
    │   │       EnemyField.cs	--класс для описания вражеского поля
    │   │       Field.cs	--класс для описания поля в общем
    │   │       IField.cs	--интерфейс для описания поля
    │   │       MyField.cs	--класс для описания поля игрока
    │   │       UpdateableType.cs
    │   │
    │   └───Ship	--описание модели кораблей
    │           IShip.cs	--интерфейс для описания корабля
    │           ShipModel.cs	--класс описывающий модель корабля
    │           ShipVisual.cs	--класс описывающий визуальную модель корабля
    │
    └───Properties
            AssemblyInfo.cs
            Resources.Designer.cs
            Resources.resx
            Settings.Designer.cs
            Settings.settings
```