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

            StateUI.StartCoroutine("Searching");
        }

        public override void Exit()
        {
            StateUI.HideUI();
            StateUI.cancelBtn.onClick.RemoveListener(OnBackClicked);
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

