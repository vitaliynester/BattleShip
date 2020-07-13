using System.ComponentModel;

namespace BattleShip.WPF.Models
{
    public enum GameMessageType
    {
        Ready,
        IsHost,
        YourTurn,
        End
    }
    public enum GameState
    {
        NotStarted,
        Ready,
        Running,
        End
    }
    public enum GameResult
    {
        Win,
        Loss
    }
    public enum AttackResult
    {
        Hit,
        Miss,
        Destroy
    }
    public class GameStatus : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public GameStatus()
        {
            Reset();
        }
        private bool _isMyTurn = false;
        private bool _isHost;
        private bool _isOpponentReady;
        private bool _isIReady;
        private GameState _state;
        private GameResult _result;
        public bool IsMyTurn
        {
            get
            {
                return _isMyTurn;
            }
            set
            {
                _isMyTurn = value;
                OnPropertyChanged("IsMyTurn");
            }
        }
        public bool IsHost
        {
            get
            {
                IsMyTurn = true;
                return _isHost;
            }
            set
            {
                _isHost = value;
            }            
        }
        public bool IsOpponentReady
        {
            get
            {
                return _isOpponentReady;
            }
            set
            {
                _isOpponentReady = value;
                OnPropertyChanged("IsOpponentReady");
            }
        }
        public bool IsIReady
        {
            get
            {
                return _isIReady;
            }
            set
            {
                _isIReady = value;
                OnPropertyChanged("IsIReady");
            }
        }
        public GameState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }
        public GameResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                OnPropertyChanged("Result");
            }
        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Reset()
        {
            State = GameState.NotStarted;
            IsIReady = false;
            IsOpponentReady = false;
        }
        public void ReverseTurn()
        {
            IsMyTurn = (IsMyTurn == true) ? false : true;
        }
    }
}
