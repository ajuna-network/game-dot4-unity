using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using _StateMachine;
using Ajuna.GenericGameEngine.Enums;
using Ajuna.NetApi.Model.Base;
using Game.Board;
using Game.Engine;
using Game.InGame;
using GameEngine.UnityMock;
using TMPro;
using UnityEngine;

namespace Game.States
{
    public class PlayingState : State<GameManager, InGameUI>
    {
        public NetworkManager Network => NetworkManager.Instance;

        private int _currentPlayer = -1;

        public PlayingState(GameManager stateMachine, InGameUI ui) : base(stateMachine, ui)
        {
        }

        private float turnStartDelay = 2f;
        private float turnEndDelay = 2f;


        public override void Enter()
        {
            _currentPlayer = StateMachine.Dot4GObj.Next;

            StateUI.ShowUI();
            StateUI.inputUI.SetActive(false);


            StateUI.turnBtn.onClick.AddListener(TurnClicked);

            StartNextTurn(_currentPlayer);

            //state enter and exit would have to be IEnum or bool after its done to wait for anim to play first??
        }

        public override void Action()
        {
            if (StateMachine.Dot4GObj.Winner != null)
            {
                StateMachine.CurrentState = new ResultState(StateMachine, StateMachine.Dot4GObj, StateMachine.resultsUI);
            } 
            else if (_currentPlayer != -1 && StateMachine.Dot4GObj.Next != _currentPlayer)
            {
                _currentPlayer = StateMachine.Dot4GObj.Next;
                StartNextTurn(_currentPlayer);
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

        void TurnClicked()
        {
            //TODO this validation check should be on the board
            if (StateMachine.Dot4GObj.ValidateStone(StateMachine.gameBoard.selectedSide, (int)StateMachine.gameBoard.selectedRow))
            {
                TakeTurn(StateMachine.Dot4GObj.Next);
                AudioManager.Instance.PlaySound(Sound.ValidMove);
            }
            else
            {
                Debug.Log("Cannot Place");
                AudioManager.Instance.PlaySound(Sound.InvalidMove);
            }
        }


        #region TURNS

        async void StartNextTurn(int next)
        {
            if (StateMachine.Dot4GObj.GamePhase == GamePhase.Play && StateMachine.Dot4GObj.Winner == null)
            {
                StateUI.inputUI.SetActive(false);

                StateUI.SetGameText("Player" + next + " Get Ready");
                //StateUI.timer.ResetTimer();

                await Task.Delay(TimeSpan.FromSeconds(2));

                StartTurn();
            }
        }


        void StartTurn()
        {
            if (Network.IsMe(StateMachine.Dot4GObj.Players[StateMachine.Dot4GObj.Next].Address)) 
            { 
                StateUI.ShowUI();
                StateMachine.gameBoard.currentToken = null;
                StateUI.inputUI.SetActive(true);
                // StateUI.SetGameText("Player" + currentPlayer + " Make your move");
                StateUI.SetGameText("Make your move");
                StateMachine.gameBoard.SetSelectedSide(Side.North, 0);
                StateMachine.gameBoard.ToggleIndicator(true);
                StateMachine.gameBoard.SpawnSkin(StateMachine.Dot4GObj.Next);
                //StateUI.timer.StartTimer();
            }
        }

        void TakeTurn(int next)
        {
            StateUI.inputUI.SetActive(false);

            StateUI.SetGameText("Player" + next + " Made His Move");

            StateUI.timer.StopTimer();

            StateMachine.gameBoard.MakeMove();

            WaitForTokenAnim();
        }

        //should be on gameboard?
        async void WaitForTokenAnim()
        {
            while (StateMachine.gameBoard.IsAnimating)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            Debug.Log("animDone");

            //StartNextTurn(StateMachine.Dot4GObj.Next);
        }

        #endregion

    }
}