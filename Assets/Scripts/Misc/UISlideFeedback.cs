using UnityEngine;
using UnityEngine.UI;

public class UISlideFeedback : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
        
        slider.onValueChanged.AddListener(UIFeedback);
    }
    
    
     void UIFeedback(float value)
    {
        AudioManager.Instance.PlaySound(Sound.SliderClick);
    }
}
