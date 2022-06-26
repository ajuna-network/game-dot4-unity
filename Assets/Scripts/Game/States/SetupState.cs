using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using _StateMachine;
using Ajuna.GenericGameEngine.Enums;
using Game.Board;
using Game.Engine;
using Game.GameSetup;
using Game.InGame;
using GameEngine.UnityMock;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.States
{
    public class SetupState : State<GameManager, SetupUI>
    {
        public NetworkManager Network => NetworkManager.Instance;

        public SetupState(GameManager stateMachine, SetupUI ui) : base(stateMachine, ui)
        {
        }       

        public override void Enter()
        {
            StateUI.ShowUI();

            EngineManager.SetupGame(StateMachine.gameId, StateMachine.player1ID, StateMachine.player2ID);

            StateMachine.gameBoard.Clear();

            //StateMachine.gameBoard.GenerateBoard(EngineManager.Fullstate);

            StateUI.timer.StartTimer();
        }

        public override void Action()
        {
            if (EngineManager.Fullstate.GameState != GameState.INITIALIZED)
            {
                WaitForAnim();
            }
            else
            {
                if (StateMachine.gameBoard.player1BombCells.Count == 3)
                {
                    StateUI.timer.ForceStopTimer();
                }
            }

           
        }

        public override void Exit()
        {
            StateUI.HideUI();
            StateMachine.gameBoard.boardRaycaster.enabled = false;
            StateUI.timer.StopTimer();
            StateUI.timer.ResetTimer();
        }


        void WaitForAnim()
        {
            Exit();
            
            if (!StateMachine.gameBoard.IntroDone())
            {
                return;
            }
            
            StateMachine.CurrentState = new PlayingState(StateMachine, StateMachine.inGameUI);
        }


        #region Conditions

        #endregion
    }
}