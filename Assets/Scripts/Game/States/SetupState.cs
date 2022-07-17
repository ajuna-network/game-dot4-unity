using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using _StateMachine;
using Ajuna.GenericGameEngine.Enums;
using Ajuna.NetApi.Model.Base;
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
            StateMachine.gameBoard.Clear();
           
        }

        public override void Action()
        {
            if (StateMachine.Dot4GObj != null && StateMachine.Dot4GObj.GamePhase != GamePhase.Bomb)
            {
                WaitForAnim();
            }          
            
            StateUI.SetPlayerBombCount(StateMachine.gameBoard.player1BombCells.Count);
        }

        public override void Exit()
        {
            StateUI.HideUI();
            StateMachine.gameBoard.boardRaycaster.enabled = false;
        }


        void WaitForAnim()
        {
            Debug.Log("WaitForAnim");

            Exit();
            
            if (!StateMachine.gameBoard.IntroDone())
            {
                return;
            }
            
            StateMachine.CurrentState = new PlayingState(StateMachine, StateMachine.inGameUI);
        }

    }
}