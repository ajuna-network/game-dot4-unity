using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
