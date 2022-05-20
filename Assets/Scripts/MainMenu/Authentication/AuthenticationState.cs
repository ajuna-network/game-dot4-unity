using _StateMachine;
using MainMenu.Landing;


namespace MainMenu.Authentication
{
    public class AuthenticationState : State<MainMenuManager, AuthenticationUI>
    {
        public AuthenticationState(MainMenuManager stateMachine, AuthenticationUI ui) : base(stateMachine, ui)
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
                StateMachine.CurrentState = new LandingState(StateMachine,StateMachine.landingUI);
            }
        }

        #endregion


    }
}
