using StateLogic;
using Ajuna.NetApi.Model.Dot4gravity;
using Game.InGame;
using UnityEngine;

namespace Game.States
{
    public class PlayingState : State<GameManager, InGameUI>
    {
        public NetworkManager Network => NetworkManager.Instance;

        private string _currentPlayer = "";

        public PlayingState(GameManager stateMachine, InGameUI ui) : base(stateMachine, ui)
        {
        }

        public override void Enter()
        {
            StateUI.ShowUI();
            StateUI.inputUI.SetActive(false);

            StateUI.turnBtn.onClick.AddListener(TurnClicked);
        }

        public override void Action()
        {
            // if we have a winner change state
            if (StateMachine.Dot4GObj.Winner != null)
            {
                StateMachine.CurrentState = new ResultState(StateMachine, StateMachine.Dot4GObj, StateMachine.resultsUI);
            } 
            // if player changed call next turn
            else if (StateMachine.Dot4GObj.Next != _currentPlayer)
            {
                _currentPlayer = StateMachine.Dot4GObj.Next;
                StateUI.inputUI.SetActive(false);

                // if it's me then enable UI for input.
                if (Network.IsMe(_currentPlayer))
                {
                    
                    StateUI.ShowUI();
                    StateMachine.gameBoard.currentToken = null;
                    StateUI.inputUI.SetActive(true);
                    StateUI.SetGameText("Make your move");
                    StateMachine.gameBoard.SetSelectedSide(Side.North, 0);
                    StateMachine.gameBoard.ToggleIndicator(true);
                    StateMachine.gameBoard.SpawnSkin(StateMachine.Dot4GObj.Players[_currentPlayer].Stone - 1);
                }
                else
                {
                    StateUI.SetGameText("Opponent player move");
                }
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
                StateUI.inputUI.SetActive(false);
                StateUI.SetGameText("Player" + _currentPlayer + " Made His Move");
                StateMachine.gameBoard.MakeMove();
                //WaitForTokenAnim();
                AudioManager.Instance.PlaySound(Sound.ValidMove);
            }
            else
            {
                Debug.Log("Cannot Place");
                AudioManager.Instance.PlaySound(Sound.InvalidMove);
            }
        }

    }
}