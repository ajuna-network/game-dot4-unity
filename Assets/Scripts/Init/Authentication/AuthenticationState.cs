using System.Runtime.InteropServices;
using System.Threading.Tasks;
using _StateMachine;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;


namespace Init.Authentication
{
    public class AuthenticationState : State<InitManager, AuthenticationUI>
    {
        public AuthenticationState(InitManager stateMachine, AuthenticationUI ui) : base(stateMachine, ui)
        {
        }

        // private string _accountAddress;
      
      //  private string _websocketUrl = "wss://mogiway-01.dotmog.com";


      
      //  public AccountManager AccountManager { get; private set; }
      //  private Wallet Wallet { get; set; }


        public override void Enter()
        {
          
             
            
            StateUI.createBtn.gameObject.SetActive(false);
            StateUI.decryptBtn.gameObject.SetActive(false);
            StateUI.inputsCnt.gameObject.SetActive(false);
            StateUI.infoTxt.gameObject.SetActive(false);

          //  StateUI.createBtn.onClick.AddListener(OnCreateClicked);
           // StateUI.decryptBtn.onClick.AddListener(OnDecryptClicked);

          //  Wallet = new Wallet();
          //  AccountManager = new AccountManager(Wallet);

           

            StateUI.ShowUI();

            StateUI.CreateWallet();
            StateUI.AttemptLogin();
        }


        public override void Exit()
        {
          //  StateUI.createBtn.onClick.RemoveListener(OnCreateClicked);
          //  StateUI.decryptBtn.onClick.RemoveListener(OnDecryptClicked);

            StateUI.HideUI();
        }

 



    


        #region AccountController

        //add a red/green dot to indicate connecteion
        //do a IsConnected in update


        
        

        // async Task DisconnectWallet()
        // {
        //     await AccountController.Instance.Wallet.StopAsync();
        // }
        //
        // void OnApplicationQuit()
        // {
        //     _ = DisconnectWallet();
        // }

        #endregion
    }
}