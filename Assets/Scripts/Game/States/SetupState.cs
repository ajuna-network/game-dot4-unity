using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using _StateMachine;
using Ajuna.GenericGameEngine.Enums;
using Game.Board;
using Game.Engine;
using Game.GameSetup;
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
                Debug.Log("State");
                StateMachine.CurrentState = new PlayingState(StateMachine, StateMachine.inGameUI);
            }
            
           
        }

        public override void Exit()
        {
            StateUI.HideUI();
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
            var maxBombs = 3;
            var waitTime = 1;

            while (maxBombs > 0)
            {
                if (StateMachine.playerAI.GetBombPosAndTime(StateMachine.gameId, out int waitTimeNew,
                        out int[] position))
                {
                    waitTime = waitTimeNew > waitTime ? waitTimeNew : waitTime;


                   // await Task.Delay(TimeSpan.FromSeconds(waitTime));

                    StateMachine.gameBoard.HighlightBombCell(2, new Vector2(position[0], position[1]));
                    EngineManager.PlaceBomb(2, position);


                    maxBombs--;
                }
            }
            Debug.Log("All Bombs Placed");
           
          
        }


        #region Conditions

        // void ReadyClicked()
        // {
        //     StateMachine.CurrentState = new PlayingState(StateMachine, StateMachine.inGameUI);
        // }

        #endregion
    }
}