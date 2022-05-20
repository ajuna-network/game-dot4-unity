using Game.Engine;
using UnityEngine;
using UnityEngine.UI;

public class Timer : UICanvas
{
    [SerializeField]private Slider timerSlider;
    
    public void StartTimer()
    {
        InvokeRepeating("TimerTick", 0, 1f);
    }

    public void ResetTimer()
    {
        timerSlider.value = 30;
    }

    public void StopTimer()
    {
        CancelInvoke("TimerTick");
    }
        
     void TimerTick()
    {
        timerSlider.value--;
        EngineManager.Tick();
    }
}
