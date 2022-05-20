using System;
using System.Threading.Tasks;
using _StateMachine;
using MainMenu.Searching.UI;
using UnityEngine.SceneManagement;

namespace MainMenu.Searching
{
    public class SearchingState : State<MainMenuManager, SearchingUI>
    {
        public SearchingState(MainMenuManager stateMachine, SearchingUI ui) : base(stateMachine, ui)
        {
        }

        public override void Enter()
        {
            StateUI.ShowUI();
            StateUI.cancelBtn.onClick.AddListener(OnBackClicked);

            Searching();
        }

        public override void Exit()
        {
            StateUI.HideUI();
            StateUI.cancelBtn.onClick.RemoveListener(OnBackClicked);
        }

        async void Searching()
        {
            StateUI.searchingText.text = "searching for a game";
            await Task.Delay(TimeSpan.FromSeconds(2));

            MatchFound();
        }
        async void MatchFound()
        {
            StateUI.searchingText.text = "match found";
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            Joining();
        }
        async void Joining()
        {
            StateUI.searchingText.text = "joining";
            await Task.Delay(TimeSpan.FromSeconds(1));
            JoinMatch();
        }


        #region Conditions

        void JoinMatch()
        {
            SceneManager.LoadScene("Game");
        }

        void OnBackClicked()
        {
            StateMachine.CurrentState = StateMachine.previousState;
        }

        #endregion
    }
}

