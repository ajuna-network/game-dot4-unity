using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using _StateMachine;
using Ajuna.GenericGameEngine.Enums;
using Game.Board;
using Game.Engine;
using Game.InGame;
using GameEngine.GravityDot;
using GameEngine.UnityMock;
using TMPro;
using UnityEngine;

namespace Game.States
{
    public class PlayingState : State<GameManager, InGameUI>
    {
        public PlayingState(GameManager stateMachine, InGameUI ui) : base(stateMachine, ui)
        {
        }

        private float turnStartDelay = 2f;
        private float turnEndDelay = 2f;


        public override void Enter()
        {
            StateUI.ShowUI();
            StateUI.inputUI.SetActive(false);


            StateUI.turnBtn.onClick.AddListener(TurnClicked);
            StartNextTurn(EngineManager.Fullstate.CurrentPlayer);

            //state enter and exit would have to be IEnum or bool after its done to wait for anim to play first??
        }

        public override void Action()
        {
            if (EngineManager.Fullstate.GameState == GameState.TIMEOUT ||
                EngineManager.Fullstate.GameState == GameState.FINISHED)
            {
                StateMachine.CurrentState = new ResultState(StateMachine, StateMachine.resultsUI);
            }
        }

        public override void Exit()
        {
            StateUI.turnBtn.onClick.RemoveListener(TurnClicked);

            StateUI.HideUI();
            StateMachine.gameBoard.ToggleIndicator(false);
            StateUI.timer.StopTimer();
            StateMachine.gameBoard.ClearHighlight();
            StateMachine.gameBoard.PlayOutro();
        }


        GameObject GetCurrentPlayerSkin(int currentPlayer)
        {
            switch (currentPlayer)
            {
                case 1:
                    return StateMachine.player1Token.tokenPrefab;
                case 2:
                    return StateMachine.player2Token.tokenPrefab;
            }
            return null;
        }


        void TurnClicked()
        {
            if (EngineManager.IsValidMove(StateMachine.gameBoard.selectedSide, StateMachine.gameBoard.selectedRow))
            {
                TakeTurn(EngineManager.Fullstate.CurrentPlayer);
            }
            else
            {
                Debug.Log("Cannot Place");
                // SoundHapticManager.instance.PlayGameAudio(GameAudio.Error);
            }
        }


        #region TURNS

        async void StartNextTurn(int currentPlayer)
        {
            if (EngineManager.Fullstate.GameState == GameState.RUNNING)
            {
                StateUI.inputUI.SetActive(false);

                StateUI.SetGameText("Player" + currentPlayer + " Get Ready");
                StateUI.timer.ResetTimer();

                await Task.Delay(TimeSpan.FromSeconds(2));

                StartTurn();
            }
        }


        void StartTurn()
        {
            StateUI.ShowUI();
            StateMachine.gameBoard.currentToken = null;
            StateUI.inputUI.SetActive(true);
            // StateUI.SetGameText("Player" + currentPlayer + " Make your move");
            StateUI.SetGameText("Make your move");
            StateMachine.gameBoard.SetSelectedSide(Side.North, 0);
            StateMachine.gameBoard.ToggleIndicator(true);
            StateMachine.gameBoard.SpawnSkin(GetCurrentPlayerSkin(EngineManager.Fullstate.CurrentPlayer));
            StateUI.timer.StartTimer();

            if (StateMachine.gameSettings.pvAI)
            {
                AIPlay();
            }
        }

        void TakeTurn(int currentPlayer)
        {
            StateUI.inputUI.SetActive(false);

            StateUI.SetGameText("Player" + currentPlayer + " Made His Move");

            StateUI.timer.StopTimer();

            StateMachine.gameBoard.MakeMove();
            
            WaitForTokenAnim();
        }

//should be on gameboard?
        async void WaitForTokenAnim()
        {
           // 
            
            await StateMachine.gameBoard.AnimateToken();
            
            Debug.Log("animDone");
           // StateMachine.gameBoard.ClearHighlight();
           

          //do a waitfor bombs if there are

            StateMachine.gameBoard.ToggleIndicator(false);
           

            await Task.Delay(TimeSpan.FromSeconds(2));

           // 
            StartNextTurn(EngineManager.Fullstate.CurrentPlayer);
        }

        #endregion


        void AIPlay()
        {
            if (StateMachine.playerAI.Play(StateMachine.gameId, out int waitTime, out Side side, out byte column))
            {
                StateMachine.gameBoard.SetSelectedSide(side, column);
                TurnClicked();
            }
        }
    }
}