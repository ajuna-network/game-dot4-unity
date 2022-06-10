using System.Threading.Tasks;
using DOTMogCore.Manager;
using NLog;
using NLog.Config;
using SubstrateNetWallet;
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

        [SerializeField] private string websocketUrl = "wss://mogiway-01.dotmog.com";
        private string _walletName = "dev_wallet";

        private AccountManager accountManager;
        private Wallet wallet;
        
        
        ////After Crash
        public GameManager GameManager { get; private set; }
    
        public ExplorerManager ExplorerManager { get; private set; }


        public void CreateWallet()
        {
            LoggingConfiguration config = new LoggingConfiguration();
            LogManager.Configuration = config;
            
            wallet = new Wallet();
            accountManager = new AccountManager(wallet);
            
            
            ////After crash
            GameManager = new GameManager(wallet);
            ExplorerManager = new ExplorerManager(wallet);
            ExplorerManager.SetPageSize(15);
        }

        
        private void Start()
        {
            createBtn.onClick.AddListener(OnCreateClicked);
            decryptBtn.onClick.AddListener(OnDecryptClicked);
        }


        public async Task AttemptLogin()
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

        #endregion
        
       
    }
    

    
    // public class AccountController : MonoBehaviour
    // {
    //     [SerializeField]
    //     private string _websocketUrl = "wss://mogiway-01.dotmog.com";
    //
    //     public string WebSocketUrl => _websocketUrl;
    //
    //     public Wallet Wallet { get; private set; }
    //
    //     private bool ChainInfoUpdateEventReceived = false;
    //
    //     public AccountManager AccountManager { get; private set; }
    //
    //     public GameManager GameManager { get; private set; }
    //
    //     public ExplorerManager ExplorerManager { get; private set; }
    //
    //     private void Awake()
    //     {
    //         LoggingConfiguration config = new LoggingConfiguration();
    //         LogManager.Configuration = config;
    //     }
    //
    //     // Start is called before the first frame update
    //     void Start()
    //     {
    //         Wallet = new Wallet();
    //         //Wallet.ChainInfoUpdated += Wallet_ChainInfoUpdated;
    //
    //         AccountManager = new AccountManager(Wallet);
    //         GameManager = new GameManager(Wallet);
    //         ExplorerManager = new ExplorerManager(Wallet);
    //         ExplorerManager.SetPageSize(15);
    //     }
    //
    //     private void Wallet_ChainInfoUpdated(object sender, ChainInfo e)
    //     {
    //         ChainInfoUpdateEventReceived = true;
    //     }
    //
    //     // Update is called once per frame
    //     void Update()
    //     {
    //         if (ChainInfoUpdateEventReceived)
    //         {
    //             Debug.Log($"ChainInfoUpdateEvent Received! {Wallet.ChainInfo.BlockNumber}");
    //             ChainInfoUpdateEventReceived = false;
    //         }
    //     }
    //
    //     public async Task ConnectAsync()
    //     {
    //         await Wallet.StartAsync(_websocketUrl);
    //
    //     }
    //
    //     public async Task DisconnectAsync()
    //     {
    //         await Wallet.StopAsync();
    //     }
    //
    //     void OnApplicationQuit()
    //     {
    //         _ = DisconnectAsync();
    //     }
    //
    // }

}