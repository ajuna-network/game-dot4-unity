using StateLogic;

using MainMenu.Achievements;
using MainMenu.Faucet;
using MainMenu.Leaderboard;

using MainMenu.Landing.UI;
using MainMenu.Searching;

namespace MainMenu.Landing
{
    public class LandingState : State<MainMenuManager, LandingUI>
    {
        public LandingState(MainMenuManager stateMachine, LandingUI ui) : base(stateMachine, ui)
        {
        }
        
        public override void Enter()
        {
            StateUI.ShowUI();
            StateMachine.SceneUI.SetActive(true);
            
            StateUI.playBtn.onClick.AddListener(PlayClicked);
            StateUI.leaderboardbtn.onClick.AddListener(LeaderboardClicked);
            StateUI.achievementsBtn.onClick.AddListener(AchievementsClicked);
            StateUI.faucetBtn.onClick.AddListener(FaucetClicked);
        }
        


        public override void Exit()
        {
            StateUI.HideUI();
            
            StateUI.playBtn.onClick.RemoveListener(PlayClicked);
            StateUI.leaderboardbtn.onClick.RemoveListener(LeaderboardClicked);
            StateUI.achievementsBtn.onClick.RemoveListener(AchievementsClicked);
            StateUI.faucetBtn.onClick.RemoveListener(FaucetClicked);
        }

        
        //Later conditions can be components, like buttonclick, animation played ect
        #region Conditions

        void PlayClicked()
        {
            StateMachine.CurrentState = new SearchingState(StateMachine, StateMachine.searchingUI);
        }
        void LeaderboardClicked()
        {
            StateMachine.CurrentState = new LeaderboardState(StateMachine, StateMachine.leaderBoardUI);
        }
        void AchievementsClicked()
        {
            StateMachine.CurrentState = new AchievementsState(StateMachine, StateMachine.achievementsUI);
        }
        void FaucetClicked()
        {
            StateMachine.CurrentState = new FaucetState(StateMachine, StateMachine.faucetUI);
        }

        #endregion


    }
}
