using UnityEngine;


public class UICanvas : MonoBehaviour
{
    /// <summary>
    /// Toggling the canvas is more performant
    /// Add ui transitions to show and hide
    /// </summary>
    
    


    public void ShowUI()
    {
        gameObject.SetActive(true);
    }
    
    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}