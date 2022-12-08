using UnityEngine;

public enum Sound
{
    ButtonDown,
    ButtonUp,
    SliderClick,
    InvalidMove,
    ValidMove,
    Detonate,
    BombBeep
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Players")]
    public AudioSource MusicPlayer;
    public AudioSource SoundPlayer;

    [Header("AudioClips")]
    public AudioClip MainMenu;
    public AudioClip ButtonUp;
    public AudioClip ButtonDown;
    public AudioClip SliderClick;
    public AudioClip InvalidMove;
    public AudioClip ValidMove;
    public AudioClip Detonate;
    public AudioClip BombBeep;
    
    void Awake()
    {
        Instance = this;
        
        DontDestroyOnLoad(this);
    }

    public void PlaySound(Sound _sound)
    {
        SoundPlayer.PlayOneShot(GetSoundClip(_sound));
    }
    public void PlayMusic( )
    {
        MusicPlayer.Play(0);
    }

    AudioClip GetSoundClip(Sound _sound)
    {
        switch (_sound)
        {
            case Sound.ButtonUp:
                return ButtonUp;
            
            case Sound.ButtonDown:
                return ButtonDown;
            
            case Sound.SliderClick:
                return SliderClick;
            
            case Sound.InvalidMove:
                return InvalidMove;
            
            case Sound.ValidMove:
                return ValidMove;
            
            case Sound.Detonate:
                return Detonate;
            
            case Sound.BombBeep:
                return BombBeep;
      
        }
        print("Clip Not Found");
        return null;
    }

}