using _StateMachine;
using MainMenu.Leaderboard.UI;

namespace MainMenu.Leaderboard
{
    public class LeaderboardState : State<MainMenuManager, LeaderBoardUI>
    {
        public LeaderboardState(MainMenuManager stateMachine, LeaderBoardUI ui) : base(stateMachine, ui)
        {
        }

        public override void Enter()
        {
            StateUI.ShowUI();
            StateMachine.SceneUI.SetActive(false);

            StateUI.backBtn.onClick.AddListener(BackClicked);
        }

        public override void Exit()
        {
            StateUI.HideUI();

            StateUI.backBtn.onClick.RemoveListener(BackClicked);
        }


        #region Conditions

        void BackClicked()
        {
            StateMachine.CurrentState = StateMachine.previousState;
        }

        #endregion
    }
}