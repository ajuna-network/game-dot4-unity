using System.Threading.Tasks;
using Ajuna.NetApi.Model.Types;
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

        public NetworkManager Network => NetworkManager.Instance;

        private void Start()
        {
            createBtn.onClick.AddListener(OnCreateClicked);
            decryptBtn.onClick.AddListener(OnDecryptClicked);
        }

        public async Task AttemptLogin()
        {
            //do async task for  wallet.Strt

            //Load Wallet
            if (!Network.Wallet.Load(Network.WalletName))
            {
                statusTxt.text = "Create a New Wallet";
                createBtn.gameObject.SetActive(true);
                inputsCnt.gameObject.SetActive(true);
                return;
            }

            //Unlock Wallet
            if (!Network.Wallet.IsUnlocked)
            {
                //show decrypt
                statusTxt.text = "Enter Your Password";
                createBtn.gameObject.SetActive(false);
                inputsCnt.gameObject.SetActive(true);
                decryptBtn.gameObject.SetActive(true);
                return;
            }

            Debug.Log($"My Key: {Network.Wallet.Account.Value}");

            Network.SetAccount(Network.Wallet.Account);

            //goto next state
            SceneManager.LoadScene("MainMenu");
        }

        public void OnCreateClicked()
        {
            var password = passwordInput.text;

            if (!Network.Wallet.IsValidPassword(password))
            {
                infoTxt.gameObject.SetActive(true);
                infoTxt.text = "Not a Valid Password";
                AudioManager.Instance.PlaySound(Sound.InvalidMove);
                return;
            }

            if (!Network.Wallet.Create(password, KeyType.Sr25519, Network.WalletName))
            {
                infoTxt.gameObject.SetActive(true);
                infoTxt.text = "Failed To Create New Account";
                AudioManager.Instance.PlaySound(Sound.InvalidMove);
            }

            //  StateUI.infoTxt.text = "New Account Created";
            AttemptLogin();
        }

        private async void OnDecryptClicked()
        {
            var password = passwordInput.text;

            if (!Network.Wallet.Unlock(password))
            {
                infoTxt.gameObject.SetActive(true);
                infoTxt.text = "Invalid Password";
                AudioManager.Instance.PlaySound(Sound.InvalidMove);
                return;
            }

            AttemptLogin();
        }
    }
}