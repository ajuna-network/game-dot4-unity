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

namespace Game.States
{
    public class SetupState : State<GameManager, SetupUI>
    {
        public SetupState(GameManager stateMachine, SetupUI ui) : base(stateMachine, ui)
        {
        }


        public override void Enter()
        {
            StateUI.ShowUI();

            EngineManager.SetupGame(StateMachine.gameId, StateMachine.player1ID, StateMachine.player2ID);
            StateMachine.gameBoard.GenerateBoard(EngineManager.Fullstate);

            if (StateMachine.gameSettings.pvAI)
            {
                AssignAI();
                PlaceAIBombs();
            }

            StateUI.timer.StartTimer();
        }

        public override void Action()
        {
            if (EngineManager.Fullstate.GameState != GameState.INITIALIZED)
            {
                WaitForAnim();
            }
        }

        public override void Exit()
        {
           /// WaitForAnim();
            StateUI.HideUI();
            StateMachine.gameBoard.boardRaycaster.enabled = false;
            StateUI.timer.StopTimer();
            StateUI.timer.ResetTimer();
        }


        void AssignAI()
        {
            StateMachine.playerAI = new PlayerAI(EngineManager.gameEngine, StateMachine.player2ID);
        }


        //should be on game board
        void PlaceAIBombs()
        {
            var waitTimeA = 1;

            while (waitTimeA < 30)
            {
                if (StateMachine.playerAI.GetBombPosAndTime(StateMachine.gameId, out int waitTimeNew,
                        out int[] position))
                {
                    StateMachine.gameBoard.PlaceBomb(2, new Vector2(position[0], position[1]));

                    waitTimeA = waitTimeNew > waitTimeA ? waitTimeNew : waitTimeA;
                }
            }
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