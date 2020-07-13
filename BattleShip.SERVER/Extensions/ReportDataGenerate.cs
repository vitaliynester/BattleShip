using System.Collections.Generic;

namespace BattleShip.SERVER
{
    public class ReportDataGenerate
    {
        private Database db;
        private List<ReportData> ReportData;
        public ReportDataGenerate(string name)
        {
            db = new Database();
            if (!db.UserExist(name))
            {
                return;
            }
            ReportData = new List<ReportData>();
            var player_one = new PlayerInfo
            {
                Name = name,
                TotalGame = db.TotalCountGamesByName(name),
                Winrate = db.WinrateByName(name),
                Accuracy = db.AccuracyByName(name)
            };

            var matchs_ids = db.MatchIDsByName(name);

            foreach (var match_id in matchs_ids)
            {
                string opponent_name = db.NameOpponentByMatchIDAndName(match_id, name);
                var player_two = new PlayerInfo
                {
                    Name = opponent_name,
                    TotalGame = db.TotalCountGamesByName(opponent_name),
                    Winrate = db.WinrateByName(opponent_name),
                    Accuracy = db.AccuracyByName(opponent_name)
                };
                var player_pair = new PairInfo { FirstPlayer = player_one, SecondPlayer = player_two };
                List<StepInfo> steps = db.GetStepsByMatchID(match_id);
                var report_data = new ReportData
                {
                    Pair = player_pair,
                    Steps = steps
                };

                ReportData.Add(report_data);
            }

        }
        public List<ReportData> Data()
        {
            return ReportData;
        }
    }
}
