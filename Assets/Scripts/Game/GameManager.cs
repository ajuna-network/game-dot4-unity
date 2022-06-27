using _StateMachine;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using Game.Board;
using Game.GameResults;
using Game.GameSetup;
using Game.InGame;
using Game.States;
using GameEngine.UnityMock;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GameManager : StateMachine
    {
        public NetworkManager Network => NetworkManager.Instance;

        private bool _iniBoard = true;
        private bool _boardTaskFlag = false;
        private Task<Dot4GObj> _boardTask = null;
        public Dot4GObj Dot4GObj;


        [Header("Game Session")] public int gameId = 1234;
        public int player1ID = 1111;
        public int player2ID = 2222;
        public PlayerAI playerAI;


        [Header("Refrences")] public ResultsUI resultsUI;
        public SetupUI setupUI;
        public InGameUI inGameUI;

        public GameBoard gameBoard;
        // public Timer timer;

        public GameSettings gameSettings;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        void Start()
        {
            CurrentState = new SetupState(this, setupUI);

        }

        private void Update()
        {
            if (!_boardTaskFlag)
            {
                _boardTaskFlag = true;
                _boardTask = Network.Dot4GClient.GetGameBoardAsync();
            }
            else if (_boardTask.IsCompleted)
            {
                Dot4GObj = _boardTask.Result;
                gameBoard.UpdateBoard(Dot4GObj);
                _boardTaskFlag = false;

                Debug.Log($"Board updated in {CurrentState.GetType().Name}!");
            }

            CurrentState?.Action();
        }

    }
}