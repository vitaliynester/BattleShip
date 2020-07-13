using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleShip.WPF.Models
{
    public class ConnectionManager
    {
        private TcpClient tcpClient;
        private TcpListener tcpListener;
        private NetworkStream NtStream;
        public string ErrorMessage { get; set; }
        public string PathReport { get; set; }
        public bool Connect { get; set; }
        ~ConnectionManager()
        {
            tcpClient?.Dispose();
            tcpListener?.Stop();
            tcpListener = null;
        }
        public Socket Client
        {
            get
            {
                return tcpClient?.Client;
            }
        }
        public Socket Listener
        {
            get
            {
                return tcpClient?.Client;
            }
        }
        //получение данных
        private async Task<string> ReciveAsync()
        {
            byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
            int bytesReceive = await NtStream.ReadAsync(buffer, 0, tcpClient.ReceiveBufferSize);
            string recive = Encoding.Default.GetString(buffer, 0, bytesReceive);

            return recive;
        }
        private async Task<string> ReciveAsyncJSON()
        {
            var sr = new StreamReader(tcpClient.GetStream());
            string recive = sr.ReadToEnd();
            return recive;
        }
        public async Task RunReciveJSON()
        {
            while (tcpClient.Connected)
            {
                try
                {
                    var recive = await ReciveAsyncJSON();
                    var result = JsonConvert.DeserializeObject<List<ReportData>>(recive);
                    if (result == null || result.Count == 0)
                    {
                        ErrorMessage = "Данного пользователя не существует";
                    }
                    else
                    {
                        ErrorMessage = string.Empty;
                        PathReport = PDFCreater.ReportData(result);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        public async Task RunRecive()
        {
            Connect = tcpClient.Client.Connected;
            while (tcpClient.Connected && Game.status.State != GameState.End)
            {
                try
                {                    
                    var recive = await ReciveAsync();
                    var type = recive.Split(' ')[0];
                    recive = recive.Replace(type, "").Remove(0, 1);

                    switch (MessageFormat.GetType(type))
                    {
                        case MessageType.AttackResult:
                            await AttackResultMessageRecived(recive);
                            break;
                        case MessageType.AttackRequest:
                            await AttackRequestMessageRecived(recive);
                            break;
                        case MessageType.Game:
                            await GameMessageRecieved(recive);
                            break;
                        default:
                            throw new Exception("Неизвестная команда!");
                    }
                }
                catch (Exception)
                {
                    Game.status.State = GameState.End;
                    MessageBox.Show("Соединение прервано!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAllConnections();
                    PageHelper.GoToPage(PageType.MainMenu);
                    return;
                }
            }
        }
        private async Task AttackRequestMessageRecived(string message)
        {
            Point p = (Point)MessageFormat.GetInfo(MessageType.AttackRequest, message);
            int i = (int)p.X;
            int j = (int)p.Y;
            await SendMessage(MessageType.AttackResult, Game.session.myField.GetAttackOnFieldResult(i, j));
        }
        private async Task AttackResultMessageRecived(string message)
        {
            AttackMessage atckMsg = (AttackMessage)MessageFormat.GetInfo(MessageType.AttackResult, message);

            if (atckMsg.Result == AttackResult.Miss) await Game.opponent.GiveTurn();

            Game.session.enemyField.ReactToAttackResult(atckMsg);
        }
        private async Task GameMessageRecieved(string msg)
        {
            var buff = MessageFormat.GetInfo(MessageType.Game, msg);

            switch ((GameMessageType)buff)
            {
                case GameMessageType.Ready:
                    Game.status.IsOpponentReady = true;
                    break;
                case GameMessageType.YourTurn:
                    Game.status.IsMyTurn = true;
                    break;
                case GameMessageType.End:
                    Game.status.State = GameState.End;
                    CloseAllConnections();
                    PageHelper.GoToPage(PageType.FightResult);
                    break;
                case GameMessageType.IsHost:
                    Game.status.IsMyTurn = true;
                    break;
                default:
                    break;
            }
        }
        //отправка данных
        private async Task SendAsync(string message)
        {
            byte[] buffer = Encoding.Default.GetBytes(message);
            await NtStream.WriteAsync(buffer, 0, message.Length);
        }
        public async Task SendMessage(MessageType msgType, object msgInfo = null)
        {
            if (tcpClient.Connected)
            {
                switch (msgType)
                {
                    case MessageType.AttackResult:
                        await SendAttackResultMessage((AttackMessage)msgInfo);
                        break;
                    case MessageType.AttackRequest:
                        await SendAttackRequestMessage((Point)msgInfo);
                        break;
                    case MessageType.Game:
                        await SendGameMessage((GameMessageType)msgInfo);
                        break;
                    default:
                        break;
                }
            }
        }
        private async Task SendAttackResultMessage(AttackMessage msg)
        {
            await SendAsync(MessageFormat.Concat(MessageType.AttackResult, msg));
        }
        private async Task SendAttackRequestMessage(Point p)
        {
            await SendAsync(MessageFormat.Concat(MessageType.AttackRequest, p));
        }
        private async Task SendGameMessage(GameMessageType msg)
        {
            await SendAsync(MessageFormat.Concat(MessageType.Game, msg));

            if (msg == GameMessageType.End)
            {
                CloseAllConnections();
                Game.status.State = GameState.End;
            }
        }
        //подключение пользователей
        public void CloseAllConnections()
        {
            tcpClient?.Close();
            tcpListener?.Stop();
        }
        public async Task<bool> StartServer(IPAddress addr, string ServerPort)
        {
            try
            {
                tcpListener = new TcpListener(addr, Convert.ToInt32(ServerPort));
                tcpListener.Start();
                return true;
            }
            catch (Exception)
            {
                throw new Exception("Невозможно запустить на этом IP или данный Port занят");
            }
        }
        public async Task WaitServerForUsers()
        {
            tcpClient = await tcpListener.AcceptTcpClientAsync();
            NtStream = tcpClient.GetStream();
            tcpListener.Stop();
        }
        public async Task ConnectTo(IPAddress ipAddr, int Port)
        {
            tcpClient = new TcpClient();
            try
            {
                await tcpClient.ConnectAsync(ipAddr, Port);
                if (tcpClient.Connected)
                {
                    NtStream = tcpClient?.GetStream();
                }
            }
            catch (Exception)
            {
                throw new Exception("Невозможно подключиться к данному серверу!");
            }
        }

        //для игры по онлайну
        //отправка данных
        public async Task SendAsyncOnline(string msg)
        {
            var sw = new StreamWriter(tcpClient.GetStream());
            sw.AutoFlush = true;
            await sw.WriteLineAsync(msg);
        }
        public async Task SendConnectionMessage(string msg)
        {
            await SendAsyncOnline(msg);
        }
        private async Task SendGameOnlineMessage(GameMessageType msg)
        {
            var send_msg = MessageFormat.Concat(MessageType.Game, msg);            
            await SendAsyncOnline(send_msg);

            if (msg == GameMessageType.End)
            {
                CloseAllConnections();
                Game.status.State = GameState.End;
            }
        }
        private async Task SendAttackRequestOnlineMessage(Point p)
        {
            var msg_send = MessageFormat.Concat(MessageType.AttackRequest, p);
            await SendAsyncOnline(msg_send);
        }
        private async Task SendAttackResultOnlineMessage(AttackMessage msg)
        {
            var msg_send = MessageFormat.Concat(MessageType.AttackResult, msg);
            await SendAsyncOnline(msg_send);
        }
        public async Task SendOnlineMessage(MessageType messageType, object msgInfo = null)
        {
            if (tcpClient.Connected)
            {
                switch (messageType)
                {
                    case MessageType.AttackResult:
                        await SendAttackResultOnlineMessage((AttackMessage)msgInfo);
                        break;
                    case MessageType.AttackRequest:
                        await SendAttackRequestOnlineMessage((Point)msgInfo);
                        break;
                    case MessageType.Game:
                        await SendGameOnlineMessage((GameMessageType)msgInfo);
                        break;
                    default:
                        break;
                }
            }
        }
        //принятие данных
        public async Task RunOnlineRecive()
        {
            while (tcpClient.Connected && Game.status.State != GameState.End)
            {
                try
                {
                    var recive = await ReciveAsync();
                    var type = recive.Split(' ')[0];
                    recive = recive.Replace(type, "").Remove(0, 1);

                    switch (MessageFormat.GetType(type))
                    {
                        case MessageType.AttackResult:
                            await AttackResultOnlineMessageRecived(recive);
                            break;
                        case MessageType.AttackRequest:
                            await AttackRequestOnlineMessageRecived(recive);
                            break;
                        case MessageType.Game:
                            await GameMessageRecieved(recive);
                            break;
                        default:
                            throw new Exception("Неизвестная команда!");
                    }
                }
                catch (Exception)
                {
                    Game.status.State = GameState.End;
                    MessageBox.Show("Соединение прервано!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAllConnections();
                    PageHelper.GoToPage(PageType.MainMenu);
                    return;
                }
            }
        }
        private async Task AttackRequestOnlineMessageRecived(string message)
        {
            Point p = (Point)MessageFormat.GetInfo(MessageType.AttackRequest, message);
            int i = (int)p.X;
            int j = (int)p.Y;
            await SendOnlineMessage(MessageType.AttackResult, Game.session.myField.GetAttackOnFieldResult(i, j));
        }
        private async Task AttackResultOnlineMessageRecived(string message)
        {
            AttackMessage atckMsg = (AttackMessage)MessageFormat.GetInfo(MessageType.AttackResult, message);

            if (atckMsg.Result == AttackResult.Miss) await Game.opponent.GiveTurn();

            Game.session.enemyField.ReactToAttackResult(atckMsg);
        }
        private async Task GameOnlineMessageRecieved(string msg)
        {
            var buff = MessageFormat.GetInfo(MessageType.Game, msg);

            switch ((GameMessageType)buff)
            {
                case GameMessageType.YourTurn:
                    Game.status.IsMyTurn = true;
                    break;
                case GameMessageType.End:
                    Game.status.State = GameState.End;
                    CloseAllConnections();
                    PageHelper.GoToPage(PageType.FightResult);
                    break;
                case GameMessageType.IsHost:
                    Game.status.IsMyTurn = true;
                    break;
                default:
                    break;
            }
        }
    }
}
