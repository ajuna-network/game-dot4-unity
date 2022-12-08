using MainMenu.Achievements.UI;
using MainMenu.Faucet.UI;
using MainMenu.Landing;
using MainMenu.Landing.UI;
using MainMenu.Leaderboard.UI;
using MainMenu.Searching.UI;
using MainMenu.Setup.UI;
using StateLogic;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuManager : StateMachine
    {
        public LandingUI landingUI;
        public FaucetUI faucetUI;
        public AchievementsUI achievementsUI;
        public LeaderBoardUI leaderBoardUI;
        public SetupUI setupUI;
        public SearchingUI searchingUI;
        public GameObject SceneUI;

        public GameSettings gameSettings;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        private void Start()
        {
            CurrentState = new LandingState(this, landingUI);
        }
    }
}