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

        private Task<Dot4GObj> _boardTask = null;

        public Dot4GObj Dot4GObj;
        
        [Header("Refrences")] 
        public ResultsUI resultsUI;

        public SetupUI setupUI;

        public InGameUI inGameUI;

        public GameBoard gameBoard;

        public GameSettings gameSettings;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        void Start()
        {
            CurrentState = new SetupState(this, setupUI);

            InvokeRepeating("PollGameBoard", 0, 0.6f);

        }

        private void Update()
        {
            CurrentState?.Action();
        }

        public void PollGameBoard()
        {
            if (_boardTask is null)
            {
                _boardTask = Network.WorkerClient.GetGameBoardAsync();
                return;
            }

            if(_boardTask.IsFaulted)
            {
                Debug.Log($"Board updated faulted with {_boardTask.Exception}!");
                _boardTask = null;
                return;
            }

            if (_boardTask.IsCompleted) {

                Dot4GObj = _boardTask.Result;
                if (Dot4GObj != null)
                {
                    gameBoard.UpdateBoard(Dot4GObj);
                }
                _boardTask = null;
                return;
            } 
            
            Debug.Log($"Timeout next Board update!");
        }

    }
}