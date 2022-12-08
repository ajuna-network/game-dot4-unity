using _StateMachine;

namespace Init.Splash
{
    public class SplashAjuna : State<InitManager, SplashUI>
    {
        public SplashAjuna(InitManager stateMachine, SplashUI ui) : base(stateMachine, ui)
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

            StateMachine.CurrentState = new SplashD4G(StateMachine, StateMachine.splashD4GUI);
        }

        public override void Exit()
        {
            StateUI.HideUI();
        }
    }
}