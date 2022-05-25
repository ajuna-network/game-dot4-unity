using _StateMachine;
using MainMenu.Landing;
using UnityEngine.SceneManagement;


namespace Init.Authentication
{
    public class AuthenticationState : State<InitManager, AuthenticationUI>
    {
        public AuthenticationState(InitManager stateMachine, AuthenticationUI ui) : base(stateMachine, ui)
        {
        }
        
        public override void Enter()
        {
            if (Authentication.IsAuthenticated)
            {
                //update state
            }
            
            StateUI.ShowUI();
            StateUI.loginBtn.onClick.AddListener(LoginClicked);
        }

        public override void Action()
        {
           
        }


        public override void Exit()
        {
            StateUI.HideUI();
            StateUI.loginBtn.onClick.RemoveListener(LoginClicked);
        }
        
        #region Conditions

        void LoginClicked()
        {
            //temp function to force auth
            Authentication.Authenticate();
            
            if (Authentication.IsAuthenticated)
            {
                SceneManager.LoadScene("MainMenu");
                //  StateMachine.CurrentState = new LandingState(StateMachine,StateMachine.landingUI);
            }
        }

        #endregion


    }
}
