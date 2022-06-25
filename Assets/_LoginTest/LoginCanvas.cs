
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginCanvas : MonoBehaviour
{
    [SerializeField]
    private string _walletName = "dev_wallet";

    private GameObject _mainController;

    private AccountController _accountController;

  //  private SceneController _sceneController;

 //   private LocalizationController _localizationController;

    public Image ConnectImage;

    public Text ConnectText;

    public Text ClientVersionText;

    public Text WalletNameText;

    public Text MessageText;

    public Text AccountAddressText;

    public Text WalletPathText;

    public Text ChainInfoNameText, ChainInfoVersionText, ChainInfoChainText;

    public Button CreateAccountButton, DecryptButton, CreateButton, CancelButton;

    public InputField PasswordField;

    public GameObject InteractionPanel, LoginPanel, LoadingPanel, PasswordPanel, NoWalletPanel;

    private Task _connectTask;

    private bool _isLoading = false;

    private string _accountAddress;

    private int _updateIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        var rootGameObjectList = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
        _mainController = rootGameObjectList.Find(p => p.name == "MainController");
        _accountController = _mainController.GetComponent<AccountController>();
        //_localizationController = _mainController.GetComponent<LocalizationController>();
       // _sceneController = _mainController.GetComponent<SceneController>();

        ClientVersionText.text = "0.21a";
        CreateAccountButton.interactable = false;
        DecryptButton.interactable = false;

        CreateButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
        LoginPanel.SetActive(false);
        InteractionPanel.SetActive(true);

        LoadingPanel.SetActive(true);

        _accountAddress = "..connecting to star link headquater ...";
       // AccountAddressText.color = ColorSchema.Address; //metastar77
        _isLoading = true;

        WalletNameText.text = _walletName;
        WalletPathText.text = Path.Combine(Application.persistentDataPath, _walletName);
        // initially try connecting
        _connectTask = _accountController.ConnectAsync();
    }

    // Update is called once per frame
    void Update()
    {
        if (_connectTask != null && _connectTask.IsCompleted)
        {
            _connectTask = null;

            if (_accountController.Wallet != null && _accountController.Wallet.IsConnected)
            {
              //  ConnectImage.color = ColorSchema.Connect; //metastar77
              //  ConnectText.GetComponent<LocalizedText>().ChangeKey("Login.ConnectionText.On");


                //ChainInfoNameText.text = _accountController.Wallet.ChainInfo.Name; metastar77
                //ChainInfoVersionText.text = _accountController.Wallet.ChainInfo.Version; metastar77
                ChainInfoChainText.text = _accountController.Wallet.ChainInfo.Chain;

                if (!_accountController.Wallet.Load(_walletName))
                {
                    Debug.Log("Couldn't load wallet!");
                };

                UpdatedLogin();
            }
            else
            {
                CreateAccountButton.interactable = false;
                DecryptButton.interactable = false;
              //  ConnectImage.color = ColorSchema.MsgWarn; //metastar77
               // ConnectText.GetComponent<LocalizedText>().ChangeKey("Login.ConnectionText.Off");

                ChainInfoNameText.text = "";
                ChainInfoVersionText.text = "";
                ChainInfoChainText.text = "";
            }

        }

        if (_isLoading && _updateIndex % 3 == 0)
        {
            AccountAddressText.text = StroboString(_accountAddress);
        }

        _updateIndex++;
    }

    public void UpdatedLogin()
    {
        _isLoading = false;

        CreateAccountButton.gameObject.SetActive(true);
        DecryptButton.gameObject.SetActive(true);
        CreateButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);

        if (_accountController.Wallet.IsCreated && !_accountController.Wallet.IsUnlocked)
        {
            LoginPanel.SetActive(true);
            LoadingPanel.SetActive(false);

            CreateAccountButton.interactable = false;
            DecryptButton.interactable = true;

            NoWalletPanel.gameObject.SetActive(false);
            PasswordPanel.gameObject.SetActive(true);

            LoginPanel.SetActive(true);
            LoadingPanel.SetActive(false);
        }
        else if (!_accountController.Wallet.IsCreated)
        {
            LoginPanel.SetActive(true);
            LoadingPanel.SetActive(false);

            CreateAccountButton.interactable = true;
            DecryptButton.interactable = false;

            NoWalletPanel.gameObject.SetActive(true);
            PasswordPanel.gameObject.SetActive(false);


          //  MessageText.color = ColorSchema.Connect; // metastar77
          //  MessageText.GetComponent<LocalizedText>().ChangeKey("Login.MessageText.NoAccount");

        }
        else
        {
            NoWalletPanel.gameObject.SetActive(true);
            PasswordPanel.gameObject.SetActive(false);

            InteractionPanel.SetActive(false);

            LoginPanel.SetActive(false);
            LoadingPanel.SetActive(true);

            CreateAccountButton.interactable = false;
            DecryptButton.interactable = false;


            _accountAddress = _accountController.Wallet.Account.Value;
            AccountAddressText.color = new Color(20 / 255f, 20 / 255f, 20 / 255f, 1f);
            _isLoading = true;

           // MessageText.color = ColorSchema.Connect; //metastar77
           // MessageText.GetComponent<LocalizedText>().ChangeKey("Login.MessageText.Ready");

           // _sceneController.ChangeSceneState(SceneController.SceneName.MainScene, 3);

        }
    }

    public void OnClickCreateAccount()
    {
        LoginPanel.SetActive(true);
        LoadingPanel.SetActive(false);

        NoWalletPanel.gameObject.SetActive(false);
        PasswordPanel.gameObject.SetActive(true);

        CreateAccountButton.gameObject.SetActive(false);
        DecryptButton.gameObject.SetActive(false);

        CreateButton.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(true);

       // MessageText.color = ColorSchema.Connect; //metastar77
        //MessageText.GetComponent<LocalizedText>().ChangeKey("Login.MessageText.NewAccount");

    }

    public async void OnClickDecryptAsync()
    {
        var password = PasswordField.text;

        if (!await _accountController.Wallet.UnlockAsync(password))
        {
           // MessageText.color = ColorSchema.MsgWarn; //metastar77
           // MessageText.GetComponent<LocalizedText>().ChangeKey("Login.MessageText.WrongPassword");
        }

        UpdatedLogin();
    }

    public async void OnClickCreateAsync()
    {
        var password = PasswordField.text;

        if (!_accountController.Wallet.IsValidWalletName(_walletName))
        {
          //  MessageText.color = ColorSchema.MsgWarn; //metastar77
          //  MessageText.GetComponent<LocalizedText>().ChangeKey("Login.MessageText.InvalidWalletName");
            return;
        }

        if (!_accountController.Wallet.IsValidPassword(password))
        {
          //  MessageText.color = ColorSchema.MsgWarn; //metastar77
          //  MessageText.GetComponent<LocalizedText>().ChangeKey("Login.MessageText.InvalidPassword");
            return;
        }

        if (!await _accountController.Wallet.CreateAsync(password, _walletName))
        {
          //  MessageText.color = ColorSchema.MsgWarn; //metastar77
          //  MessageText.GetComponent<LocalizedText>().ChangeKey("Login.MessageText.AccountFailed");
            return;
        };

        UpdatedLogin();
    }

    public void OnClickCancel()
    {
        UpdatedLogin();
    }

    private string ColorizeString(string str)
    {
        if (str.Length > 64)
        {
            return str;
        }

        string result = "";
        for (uint i = 0; i < str.Length; i++)
        {
            uint xR = 255 - i;
            uint xG = 0;
            uint xB = 0 + i;
            uint xA = 255;

            Debug.Log($"color=#{xR:X2}{xG:X2}{xB:X2}{xA:X2}");

            result += $"<color=#{xR:X2}{xG:X2}{xB:X2}{xA:X2}>{str[(int)i]}</color>";
        }
        return result;
    }

    private int _stroboStartIndex = 0;
    private string StroboString(string str)
    {
        if (str.Length > 64)
        {
            return str;
        }

        string result = "";
        for (uint i = 0; i < str.Length; i++)
        {
            var isSet = false;
            for (uint j = 0; j < 5; j++)
            {
                if (_stroboStartIndex >= j && (i == (uint)(_stroboStartIndex - j) % str.Length || i == (uint)(_stroboStartIndex + j) % str.Length))
                {
                    uint val = 255;
                    if (j > 2)
                    {
                        val = 255 - ((j - 2) * 50);
                    }

                    result += $"<color=#{val:X2}{val:X2}{val:X2}{val:X2}>{str[(int)i]}</color>";
                    isSet = true;
                }
            }

            if (!isSet)
            {
                result += str[(int)i];
            }
        }
        _stroboStartIndex++;
        return result;
    }
}
