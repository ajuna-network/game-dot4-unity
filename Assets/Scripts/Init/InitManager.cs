using _StateMachine;
using Init.Authentication;
using Init.Splash;
using Ajuna.NetWallet;
using UnityEngine;

public class InitManager : StateMachine
{
    public SplashUI splashAjunaUI;
    public SplashUI splashD4GUI;
    
    public AuthenticationUI authenticationUI;
   // public AccountController accountController;

  
    
    void Start()
    {
          CurrentState = new SplashAjuna(this, splashAjunaUI);
          AudioManager.Instance.PlayMusic();
    }
}
