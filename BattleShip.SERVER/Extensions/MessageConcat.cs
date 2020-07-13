namespace BattleShip.SERVER
{
    public class MessageConcat
    {
        public string Place { get; set; }
        public string Result { get; set; }

        public MessageConcat(string message)
        {
            GetConcat(message);
        }
        private void GetConcat(string msg)
        {
            Place = GetCoord(msg);
            Result = GetResult(msg);
        }
        private string GetCoord(string msg)
        {
            int x, y;
            if (msg == null)
            {
                return string.Empty;
            }
            string[] splits = msg.Split(' ');
            switch (splits[1])
            {
                case "destroy":
                    x = int.Parse(splits[3]);
                    x++;
                    y = int.Parse(splits[2]);
                    y++;
                    return $"{x}-{y}";
                case "hit":
                    x = int.Parse(splits[3]);
                    x++;
                    y = int.Parse(splits[2]);
                    y++;
                    return $"{x}-{y}";
                case "miss":
                    x = int.Parse(splits[3]);
                    x++;
                    y = int.Parse(splits[2]);
                    y++;
                    return $"{x}-{y}";
                default:
                    return "";
            }
        }
        private string GetResult(string msg)
        {
            if (msg == null)
            {
                return string.Empty;
            }
            string[] splits = msg.Split(' ');
            switch (splits[1])
            {
                case "destroy":
                    return $"Корабль уничтожен";
                case "hit":
                    return $"Попадание";
                case "miss":
                    return $"Промах";
                default:
                    return "";
            }
        }
    }
}
