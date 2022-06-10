using Game.Engine;
using UnityEngine;
using UnityEngine.UI;

public class Timer : UICanvas
{
    [SerializeField]private Slider timerSlider;
    private bool updateUI = true;
    
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
        if (updateUI)
        {
            timerSlider.value--; 
        }

      
        EngineManager.Tick();
    }
     
     //tempfunctin to force the timer to end
     public void ForceStopTimer()
     {
         updateUI = false;
         
         CancelInvoke("TimerTick");
         InvokeRepeating("TimerTick", 0, 30f);
     }
}
