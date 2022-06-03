using System.Threading.Tasks;
using _StateMachine;
using DOTMogCore.Manager;
using SubstrateNetWallet;
using UnityEngine.SceneManagement;


namespace Init.Authentication
{
    public class AuthenticationState : State<InitManager, AuthenticationUI>
    {
        public AuthenticationState(InitManager stateMachine, AuthenticationUI ui) : base(stateMachine, ui)
        {
        }

        // private string _accountAddress;
        private string _walletName = "dev_wallet";
        private string _websocketUrl = "wss://mogiway-01.dotmog.com";


        private Task _loginTask;
        public AccountManager AccountManager { get; private set; }
        private Wallet Wallet { get; set; }


        public override void Enter()
        {
            StateUI.createBtn.gameObject.SetActive(false);
            StateUI.decryptBtn.gameObject.SetActive(false);
            StateUI.inputsCnt.gameObject.SetActive(false);
            StateUI.infoTxt.gameObject.SetActive(false);

            StateUI.createBtn.onClick.AddListener(OnCreateClicked);
            StateUI.decryptBtn.onClick.AddListener(OnDecryptClicked);

            Wallet = new Wallet();
            AccountManager = new AccountManager(Wallet);

            _loginTask = AttemptLogin();

            StateUI.ShowUI();
        }


        public override void Exit()
        {
            StateUI.createBtn.onClick.RemoveListener(OnCreateClicked);
            StateUI.decryptBtn.onClick.RemoveListener(OnDecryptClicked);

            StateUI.HideUI();
        }

        public async void OnCreateClicked()
        {
            var password = StateUI.passwordInput.text;

          //  


            if (!Wallet.IsValidPassword(password))
            {
                StateUI.infoTxt.gameObject.SetActive(true);
                StateUI.infoTxt.text = "Not a Valid Password";
                return;
            }

            if (!await Wallet.CreateAsync(password, _walletName))
            {
                StateUI.infoTxt.gameObject.SetActive(true);
                StateUI.infoTxt.text = "Failed To Create New Account";
            }


          //  StateUI.infoTxt.text = "New Account Created";
            _loginTask = AttemptLogin();
        }

        async void OnDecryptClicked()
        {
            var password = StateUI.passwordInput.text;

            if (!await Wallet.UnlockAsync(password))
            {
                StateUI.infoTxt.gameObject.SetActive(true);
                StateUI.infoTxt.text = "Invalid Password";
                return;
            }
          

            _loginTask = AttemptLogin();
        }


        #region AccountController

        async Task AttemptLogin()
        {
            //Connect
            if (!Wallet.IsConnected)
            {
                StateUI.statusTxt.text = "Connecting...";

                await Wallet.StartAsync(_websocketUrl);

               // StateUI.statusTxt.text = "Connected";
            }

            //Load Wallet
            if (!Wallet.Load(_walletName))
            {
                StateUI.statusTxt.text = "Please Create a New Wallet";
                StateUI.createBtn.gameObject.SetActive(true);
                StateUI.inputsCnt.gameObject.SetActive(true);
                return;
            }

          

            //Unlock Wallet
            if (!Wallet.IsUnlocked)
            {
                //show decrypt  
                StateUI.statusTxt.text = "Please Enter Your Password";
                StateUI.createBtn.gameObject.SetActive(false);
                StateUI.inputsCnt.gameObject.SetActive(true);
                StateUI.decryptBtn.gameObject.SetActive(true);
                return;
            }

            _loginTask = null;
            //goto next state
            SceneManager.LoadScene("MainMenu");
           
        }

        
        

        async Task DisconnectWallet()
        {
            await Wallet.StopAsync();
        }

        void OnApplicationQuit()
        {
            _ = DisconnectWallet();
        }

        #endregion
    }
}