using _StateMachine;
using Game.Board;
using Game.GameResults;
using Game.GameSetup;
using Game.InGame;
using Game.States;
using GameEngine.UnityMock;
using UnityEngine;

namespace Game
{
    public class GameManager : StateMachine
    {
        [Header("Game Session")] public int gameId = 1234;
        public int player1ID = 1111;
        public int player2ID = 2222;
        public PlayerAI playerAI;


        [Header("Refrences")] public ResultsUI resultsUI;
        public SetupUI setupUI;
        public InGameUI inGameUI;

        public GameBoard gameBoard;
        // public Timer timer;

        [Header("Tokens")] public Achievement player1Token;
        public Achievement player2Token;

        public GameSettings gameSettings;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        void Start()
        {
            CurrentState = new SetupState(this, setupUI);
        }

        private void FixedUpdate()
        {
//            Debug.Log(EngineManager.Fullstate.GameState);
            // Debug.Log(EngineManager.Fullstate.CurrentPlayer);
        }
    }
}