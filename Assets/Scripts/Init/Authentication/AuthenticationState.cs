using _StateMachine;


namespace Init.Authentication
{
    public class AuthenticationState : State<InitManager, AuthenticationUI>
    {
        public AuthenticationState(InitManager stateMachine, AuthenticationUI ui) : base(stateMachine, ui)
        {
        }

        public override void Enter()
        {
            StateUI.createBtn.gameObject.SetActive(false);
            StateUI.decryptBtn.gameObject.SetActive(false);
            StateUI.inputsCnt.gameObject.SetActive(false);
            StateUI.infoTxt.gameObject.SetActive(false);
            StateMachine.NetworkInfo.gameObject.SetActive(true);

            StateUI.ShowUI();
            //StateUI.CreateWallet();
            StateUI.AttemptLogin();
        }


        public override void Exit()
        {
            StateUI.HideUI();
        }

    }
}