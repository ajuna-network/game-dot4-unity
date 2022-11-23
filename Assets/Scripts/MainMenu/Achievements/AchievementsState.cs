using _StateMachine;
using MainMenu.Achievements.UI;

namespace MainMenu.Achievements
{
    public class AchievementsState : State<MainMenuManager, AchievementsUI>
    {
        public AchievementsState(MainMenuManager stateMachine, AchievementsUI ui) : base(stateMachine, ui)
        {
        }


        public override void Enter()
        {
            StateUI.ShowUI();
        
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