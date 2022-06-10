using System.Threading.Tasks;
using DOTMogCore.Manager;
using SubstrateNetWallet;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Init.Authentication
{
    public class AuthenticationUI : UICanvas
    {
        public TextMeshProUGUI statusTxt;
        public TextMeshProUGUI infoTxt;
        public Button decryptBtn;
        public Button createBtn;
        public RectTransform inputsCnt;
        public TMP_InputField passwordInput;

        [SerializeField] private string websocketUrl = "wss://mogiway-01.dotmog.com";
        private string _walletName = "dev_wallet";

        private AccountManager accountManager;
        private Wallet wallet;


        public void CreateWallet()
        {
            wallet = new Wallet();
            accountManager = new AccountManager(wallet);
        }

        private void Start()
        {
            createBtn.onClick.AddListener(OnCreateClicked);
            decryptBtn.onClick.AddListener(OnDecryptClicked);
        }


         async Task AttemptLogin()
        {
            //do async task for  wallet.Strt

            //Connect
            if (!wallet.IsConnected)
            {
                statusTxt.text = "Connecting...";

                //  var connectTask =  Wallet.StartAsync(_websocketUrl);
                //execute taks 

                await wallet.StartAsync(websocketUrl);
                //and waith for when its complete

                // StateUI.statusTxt.text = "Connected";
            }

            //Load Wallet
            if (!wallet.Load(_walletName))
            {
                statusTxt.text = "Please Create a New Wallet";
                createBtn.gameObject.SetActive(true);
                inputsCnt.gameObject.SetActive(true);
                return;
            }


            //Unlock Wallet
            if (!wallet.IsUnlocked)
            {
                //show decrypt  
                statusTxt.text = "Please Enter Your Password";
                createBtn.gameObject.SetActive(false);
                inputsCnt.gameObject.SetActive(true);
                decryptBtn.gameObject.SetActive(true);
                return;
            }


            //goto next state
            SceneManager.LoadScene("MainMenu");
        }

        public async void OnCreateClicked()
        {
            var password = passwordInput.text;


            if (!wallet.IsValidPassword(password))
            {
                infoTxt.gameObject.SetActive(true);
                infoTxt.text = "Not a Valid Password";
                return;
            }

            if (!await wallet.CreateAsync(password, _walletName))
            {
                infoTxt.gameObject.SetActive(true);
                infoTxt.text = "Failed To Create New Account";
            }


            //  StateUI.infoTxt.text = "New Account Created";
            AttemptLogin();
        }

        async void OnDecryptClicked()
        {
            var password = passwordInput.text;

            if (!await wallet.UnlockAsync(password))
            {
                infoTxt.gameObject.SetActive(true);
                infoTxt.text = "Invalid Password";
                return;
            }


            AttemptLogin();
        }
    }
}