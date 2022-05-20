using _StateMachine;
using MainMenu.Achievements.UI;
using MainMenu.Authentication;
using MainMenu.Faucet.UI;
using MainMenu.Landing.UI;
using MainMenu.Leaderboard.UI;
using MainMenu.Searching.UI;
using MainMenu.Setup.UI;
using MainMenu.Splash;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuManager : StateMachine
    {
    
        public SplashUI splashUI;
        public AuthenticationUI authenticationUI;
        public LandingUI landingUI;
        public FaucetUI faucetUI;
        public AchievementsUI achievementsUI;
        public LeaderBoardUI leaderBoardUI;
        public SetupUI setupUI;
        public SearchingUI searchingUI;

        public GameSettings gameSettings;
    
        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        void Start()
        {
            CurrentState = new SplashState(this, splashUI);        
            // UpdateState(State.SplashState);
        }


        #region SPLASH STATE

        // IEnumerator SplashState()
        // {
        //    // splashState.ShowUI();
        //    // PlayingSplash();
        //
        //     // while (currentState == State.SplashState)
        //     // {
        //     //     yield return null;
        //     // }
        //
        //
        //   //  splashState.HideUI();
        // }



        #endregion


        // #region AUTHENTICATION STATE
        //
        // IEnumerator AuthenticationState()
        // {
        //     authenticationUI.ShowUI();
        //     CheckAuthentication();
        //
        //     while (currentState == State.AuthenticationState)
        //     {
        //         yield return null;
        //     }
        //
        //     authenticationUI.HideUI();
        // }
        //
        // void CheckAuthentication()
        // {
        //     if (Authentication.IsAuthenticated())
        //     {
        //         UpdateState(State.LandingState);
        //     }
        // }
        //
        //
        // public void LoginClicked()
        // {
        //     Authentication.Authenticate();
        //
        //     if (Authentication.isAuthenticated)
        //     {
        //         UpdateState(State.LandingState);
        //     }
        //     else
        //     {
        //         print("Authentication Error");
        //     }
        // }
        //
        // #endregion
        //
        //
        // #region LANDING STATE
        //
        // IEnumerator LandingState()
        // {
        //     landingUI.ShowUI();
        //
        //
        //     while (currentState == State.LandingState)
        //     {
        //         yield return null;
        //     }
        //
        //     landingUI.HideUI();
        // }
        //
        //
        // public void PlayClicked()
        // {
        //     SceneManager.LoadScene("Game");
        // }
        //
        // public void LeaderboardClicked()
        // {
        //     UpdateState(State.LeaderboardState);
        // }
        //
        // public void AchievementsClicked()
        // {
        //     UpdateState(State.AchievementState);
        // }
        //
        // public void FaucetClicked()
        // {
        //     UpdateState(State.FaucetState);
        // }
        //
        // #endregion
        //
        //
        // #region LEADERBOARD STATE
        //
        // IEnumerator LeaderboardState()
        // {
        //     leaderboardUI.ShowUI();
        //
        //     while (currentState == State.LeaderboardState)
        //     {
        //         yield return null;
        //     }
        //
        //     leaderboardUI.HideUI();
        // }
        //
        // #endregion
        //
        //
        // #region ACHIEVEMENT STATE
        //
        // IEnumerator AchievementState()
        // {
        //     achievementsUI.ShowUI();
        //
        //     while (currentState == State.AchievementState)
        //     {
        //         yield return null;
        //     }
        //
        //     achievementsUI.HideUI();
        // }
        //
        // #endregion
        //
        //
        // #region ACHIEVEMENTSTATE
        //
        // IEnumerator FaucetState()
        // {
        //     faucetUI.ShowUI();
        //
        //     while (currentState == State.FaucetState)
        //     {
        //         yield return null;
        //     }
        //
        //     faucetUI.HideUI();
        // }
        //
        // #endregion
    }
}