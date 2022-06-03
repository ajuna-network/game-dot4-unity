using _StateMachine;
using Init.Authentication;
using Init.Splash;
using SubstrateNetWallet;
using UnityEngine;

public class InitManager : StateMachine
{
    public SplashUI splashUI;
    public AuthenticationUI authenticationUI;

  
    
    void Start()
    {
          CurrentState = new SplashState(this, splashUI);
    }
}
