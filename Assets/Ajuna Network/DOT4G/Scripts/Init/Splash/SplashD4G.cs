using StateLogic;
using Init.Authentication;

namespace Init.Splash
{
    public class SplashD4G : State<InitManager, SplashUI>
    {
        public SplashD4G(InitManager stateMachine, SplashUI ui) : base(stateMachine, ui)
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