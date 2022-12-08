using StateLogic;
using Init.Authentication;
using Init.Splash;

public class InitManager : StateMachine
{
    public SplashUI splashAjunaUI;
    public SplashUI splashD4GUI;
    public NetworkInfo NetworkInfo;

    public AuthenticationUI authenticationUI;

    void Start()
    {
        CurrentState = new SplashAjuna(this, splashAjunaUI);
        AudioManager.Instance.PlayMusic();
    }
}