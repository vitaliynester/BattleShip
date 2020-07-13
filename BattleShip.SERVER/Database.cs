using Npgsql;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BattleShip.SERVER
{
    public class Database
    {
        private static readonly string ip_db = "127.0.0.1";
        private static readonly int port_db = 5432;
        private static readonly string dbName = "battleship";
        private static readonly string user_id_db = "postgres";
        private static readonly string password_db = "";

        private string connString = $"Server={ip_db};Port={port_db};Database={dbName};User Id={user_id_db};Password={password_db};";
        private NpgsqlConnection connection;
        private string sql = string.Empty;
        private NpgsqlCommand cmd;

        public Database()
        {
            connection = new NpgsqlConnection(connString);
        }
        //для работы записей в бд
        public bool UserExist(string username)
        {
            try
            {
                connection.Open();
                sql = $"select * from account where account_login='{username}';";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    connection.Close();
                    reader.Close();
                    return true;
                }
                connection.Close();
                reader.Close();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                connection.Close();
                return false;
            }
        }
        public void RegisterUser(string username, string password)
        {
            try
            {
                connection.Open();
                sql = $"insert into account(account_login, account_password) values('{username}', '{hash_password(password)}');";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                reader.Read();
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                connection.Close();
            }
        }
        public bool LoginUser(string username, string password)
        {
            try
            {
                connection.Open();
                sql = $"select * from account where account_login='{username}' and account_password='{hash_password(password)}';";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    connection.Close();
                    return true;
                }
                connection.Close();
                return false;
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }
        }
        public int CreateMatch(string first_player, string second_player)
        {
            try
            {
                connection.Open();
                sql = $"insert into match(match_start_date) values(now()) RETURNING match_id;";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                int match_id = -1;
                while (reader.Read())
                {
                    match_id = Int32.Parse(reader[0].ToString());
                }
                reader.Close();
                connection.Close();
                AddPlayerInMatch(match_id, first_player);
                AddPlayerInMatch(match_id, second_player);
                return match_id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                connection.Close();
                return -1;
            }
        }
        public void MakeStep(int match_id, string acc_login, string place, string attack_result, int left_first, int left_second)
        {
            try
            {
                connection.Open();
                sql = $"insert into step(match_id, account_login, attack_place, attack_result, first_player_ships, second_player_ships) values ({match_id}, '{acc_login}', '{place}', '{attack_result}', {left_first}, {left_second});";
                cmd = new NpgsqlCommand(sql, connection);
                cmd.ExecuteReader();
                connection.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.Message); 
            }
        }
        public void EndMatch(int match_id, string winner_name)
        {
            try
            {
                connection.Open();
                sql = $"update match set match_end_date=now() where match_id={match_id};";
                cmd = new NpgsqlCommand(sql, connection);
                cmd.ExecuteReader();
                connection.Close();
                UpdateWinnerByMatchId(match_id, winner_name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void AddPlayerInMatch(int match_id, string name)
        {
            try
            {
                connection.Open();
                sql = $"insert into take_part(match_id, account_login, is_winner) values ({match_id}, '{name}', false);";
                cmd = new NpgsqlCommand(sql, connection);
                cmd.ExecuteReader();
                connection.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                throw e;
            }
        }
        private void UpdateWinnerByMatchId(int match_id, string name)
        {
            try
            {
                connection.Open();
                sql = $"update take_part set is_winner=True where match_id={match_id} and account_login='{name}';";
                cmd = new NpgsqlCommand(sql, connection);
                cmd.ExecuteReader();
                connection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //для чтения из бд
        public int TotalCountGamesByName(string name)
        {
            try
            {
                connection.Open();
                int count_games = 0;
                sql = $"select count(match_id) from account A inner join take_part tp on A.account_login = tp.account_login where A.account_login='{name}' group by A.account_login;";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    count_games = int.Parse(reader[0].ToString());
                }
                connection.Close();
                reader.Close();
                return count_games;
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.Message);
                return -1;
            }
        }
        public double WinrateByName(string name)
        {
            var total_games = TotalCountGamesByName(name);
            var win_games = TotalWinnerGamesByName(name);
            if (win_games == 0)
            {
                return 0;
            }
            return Math.Round((double)win_games / total_games, 2);
        }
        public double AccuracyByName(string name)
        {
            var total_shoot = TotalShootByName(name);
            var success_shoot = TotalSuccessShootByName(name);
            if (success_shoot == 0)
            {
                return 0;
            }
            return Math.Round((double)success_shoot / total_shoot, 2);
        }
        public List<int> MatchIDsByName(string name)
        {
            List<int> games = new List<int>();
            try
            {
                connection.Open();
                int buff = 0;
                sql = $"select M.match_id from match M inner join take_part tp on M.match_id = tp.match_id where account_login = '{name}' and M.match_end_date>'2020-01-01' order by M.match_id;";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    buff = int.Parse(reader[0].ToString());
                    games.Add(buff);
                }
                connection.Close();
                reader.Close();
                return games;
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.Message);
                return games;
            }
        }
        public string NameOpponentByMatchIDAndName(int match_id, string name)
        {
            string opponent_name = string.Empty;
            try
            {
                connection.Open();
                sql = $"select account_login from take_part tp inner join match m on tp.match_id = m.match_id where m.match_id={match_id} and tp.account_login!='{name}';";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    opponent_name = reader[0].ToString();
                }
                connection.Close();
                reader.Close();
                return opponent_name;
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.Message);
                return opponent_name;
            }
        }
        public List<StepInfo> GetStepsByMatchID(int match_id)
        {
            List<StepInfo> steps = new List<StepInfo>();
            try
            {
                connection.Open();
                sql = $"select s.account_login, s.attack_place, s.attack_result, s.first_player_ships, s.second_player_ships from step s where match_id={match_id};";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var step = new StepInfo
                    {
                        NamePlayer = reader[0].ToString(),
                        PlaceShoot = reader[1].ToString(),
                        Result = reader[2].ToString(),
                        ShipsLeftFirstPlayer = int.Parse(reader[3].ToString()),
                        ShipsLeftSecondPlayer = int.Parse(reader[4].ToString())
                    };
                    steps.Add(step);
                }
                connection.Close();
                reader.Close();
                return steps;
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.Message);
                return steps;
            }
        }
        private int TotalWinnerGamesByName(string name)
        {
            try
            {
                connection.Open();
                int count_games = 0;
                sql = $"select count(match_id) from account A inner join take_part tp on A.account_login = tp.account_login where A.account_login='{name}' and tp.is_winner=True group by A.account_login;";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    count_games = int.Parse(reader[0].ToString());
                }
                connection.Close();
                reader.Close();
                return count_games;
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.Message);
                return -1;
            }
        }
        private int TotalShootByName(string name)
        {
            try
            {
                connection.Open();
                int count_shoot = 0;
                sql = $"select count(s.step_id) from account a inner join step s on a.account_login = s.account_login where a.account_login='{name}' group by a.account_login;";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    count_shoot = int.Parse(reader[0].ToString());
                }
                connection.Close();
                reader.Close();
                return count_shoot;
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.Message);
                return -1;
            }
        }
        private int TotalSuccessShootByName(string name)
        {
            try
            {
                connection.Open();
                int count_shoot = 0;
                sql = $"select count(s.step_id) from account a inner join step s on a.account_login = s.account_login where a.account_login='{name}' and (s.attack_result='Попадание' or s.attack_result='Корабль уничтожен') group by a.account_login;";
                cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    count_shoot = int.Parse(reader[0].ToString());
                }
                connection.Close();
                reader.Close();
                return count_shoot;
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        private static string hash_password(string password)
        {
            byte[] data = Encoding.Default.GetBytes(password);
            var result = new SHA512Managed().ComputeHash(data);
            return BitConverter.ToString(result).Replace("-", "").ToLower();
        }
    }
}
