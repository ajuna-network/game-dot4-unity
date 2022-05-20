using _StateMachine;
using MainMenu.Authentication;

namespace MainMenu.Splash
{
    public class SplashState : State<MainMenuManager, SplashUI>
    {
        public SplashState(MainMenuManager stateMachine, SplashUI ui) : base(stateMachine, ui)
        {
        }


        public override void Enter()
        {
            StateUI.ShowUI();
        }

        public override void Action()
        {
            if (StateUI.IsPlaying())
            {
                return;
            }

            StateMachine.CurrentState = new AuthenticationState(StateMachine, StateMachine.authenticationUI);
        }

        public override void Exit()
        {
            StateUI.HideUI();
        }
    }
}