using System;
using System.Windows;

namespace BattleShip.WPF.Models
{
    public enum MessageType
    {
        Chat,
        AttackResult,
        AttackRequest,
        Game,
        Login
    }
    public class MessageFormat
    {
        public static object GetInfo(MessageType msgType, object msg)
        {
            var mesg = ((string)msg).Split(' ');
            switch (msgType)
            {
                case MessageType.AttackResult:
                    int I = int.Parse(mesg[1]);
                    int J = int.Parse(mesg[2]);
                    switch (mesg[0])
                    {
                        case "hit":
                            return new AttackMessage(AttackResult.Hit, I, J);
                        case "miss":
                            return new AttackMessage(AttackResult.Miss, I, J);
                        case "destroy":
                            var orientation = (mesg[4] == "h") ? Orientation.Horizontal : Orientation.Vertical;
                            var ship = new ShipModel(int.Parse(mesg[3]), orientation, int.Parse(mesg[5]), int.Parse(mesg[6]));
                            return new AttackMessage(AttackResult.Destroy, I, J, ship);
                        default:
                            MessageBox.Show("Ошибка сообщения!");
                            throw new Exception("Ошибка сообщения!");
                    }
                case MessageType.AttackRequest:
                    int x = int.Parse(mesg[0]);
                    int y = int.Parse(mesg[1]);
                    return new Point(x, y);
                case MessageType.Game:
                    var buf_string = mesg[0].Replace("\r\n", "");
                    switch (buf_string)
                    {
                        case "end":
                            return GameMessageType.End;
                        case "ready":
                            return GameMessageType.Ready;
                        case "yourturn":
                            return GameMessageType.YourTurn;
                        case "ishost":
                            return GameMessageType.YourTurn;
                        default:
                            throw new Exception("Неизвестное сообщение!");
                    }
                default:
                    return null;
            }
        }

        public static MessageType GetType(string msg)
        {
            var type = ((string)msg).Split(' ')[0];

            switch (type)
            {
                case "/attack_result":
                    return MessageType.AttackResult;
                case "/attack_request":
                    return MessageType.AttackRequest;
                case "/game":
                    return MessageType.Game;
                default:
                    throw new Exception($"Неизвестный тип сообщения! {type}");
            }
        }

        public static string Concat(MessageType msgType, object msgInfo)
        {
            string msg = string.Empty;
            switch (msgType)
            {
                case MessageType.Chat:
                    break;
                case MessageType.AttackResult:
                    var a_msg = (AttackMessage)msgInfo;
                    switch (a_msg.Result)
                    {
                        case AttackResult.Hit:
                            msg = $"/attack_result {"hit"} {a_msg.X} {a_msg.Y}";
                            break;
                        case AttackResult.Miss:
                            msg = $"/attack_result {"miss"} {a_msg.X} {a_msg.Y}";
                            break;
                        case AttackResult.Destroy:
                            var orientation = (a_msg.Ship.Orientation == Orientation.Horizontal) ? "h" : "v";
                            msg = $"/attack_result {"destroy"} {a_msg.X} {a_msg.Y} {a_msg.Ship.Length} {orientation} {a_msg.Ship.I} {a_msg.Ship.J}";
                            break;
                        default:
                            break;
                    }
                    break;
                case MessageType.AttackRequest:
                    var point = (Point)msgInfo;
                    msg = $"/attack_request {point.X} {point.Y}";
                    break;
                case MessageType.Game:
                    switch (msgInfo)
                    {
                        case GameMessageType.End:
                            msg = $"/game {"end"}";
                            break;
                        case GameMessageType.Ready:
                            msg = $"/game {"ready"}";
                            break;
                        case GameMessageType.YourTurn:
                            msg = $"/game {"yourturn"}";
                            break;
                    }
                    break;
                default:
                    break;
            }
            return msg;
        }
    }
}
