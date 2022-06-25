using System.Threading.Tasks;
using NLog;
using NLog.Config;
using Ajuna.NetWallet;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Init.Authentication
{
    public class AuthenticationUI : UICanvas
    {
        #region Old

        public TextMeshProUGUI statusTxt;
        public TextMeshProUGUI infoTxt;
        public Button decryptBtn;
        public Button createBtn;
        public RectTransform inputsCnt;
        public TMP_InputField passwordInput;

        public NetworkManager networkManager;      
        
        //public void CreateWallet()
        //{
        //    LoggingConfiguration config = new LoggingConfiguration();
        //    /LogManager.Configuration = config;
        //    
        //    wallet = new Wallet();
        //}

        
        private void Start()
        {
            createBtn.onClick.AddListener(OnCreateClicked);
            decryptBtn.onClick.AddListener(OnDecryptClicked);
        }


        public async Task AttemptLogin()
        {
            //do async task for  wallet.Strt

            //Connect
            if (!networkManager.Wallet.IsConnected)
            {
                statusTxt.text = "Connecting...";

                //  var connectTask =  Wallet.StartAsync(_websocketUrl);
                //execute taks 

                await networkManager.Wallet.StartAsync(networkManager.nodeUrl);
                //and waith for when its complete

                // StateUI.statusTxt.text = "Connected";
            }

            //Load Wallet
            if (!networkManager.Wallet.Load(networkManager.walletName))
            {
                statusTxt.text = "Create a New Wallet";
                createBtn.gameObject.SetActive(true);
                inputsCnt.gameObject.SetActive(true);
                return;
            }


            //Unlock Wallet
            if (!networkManager.Wallet.IsUnlocked)
            {
                //show decrypt  
                statusTxt.text = "Enter Your Password";
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


            if (!networkManager.Wallet.IsValidPassword(password))
            {
                infoTxt.gameObject.SetActive(true);
                infoTxt.text = "Not a Valid Password";
                AudioManager.Instance.PlaySound(Sound.InvalidMove);
                return;
            }

            if (!await networkManager.Wallet.CreateAsync(password, networkManager.walletName))
            {
                infoTxt.gameObject.SetActive(true);
                infoTxt.text = "Failed To Create New Account";
                AudioManager.Instance.PlaySound(Sound.InvalidMove);
              
            }


            //  StateUI.infoTxt.text = "New Account Created";
            AttemptLogin();
        }

        async void OnDecryptClicked()
        {
            var password = passwordInput.text;

            if (!await networkManager.Wallet.UnlockAsync(password))
            {
                infoTxt.gameObject.SetActive(true);
                infoTxt.text = "Invalid Password";
                AudioManager.Instance.PlaySound(Sound.InvalidMove);
                return;
            }


            AttemptLogin();
        }

        #endregion
        
       
    }

}