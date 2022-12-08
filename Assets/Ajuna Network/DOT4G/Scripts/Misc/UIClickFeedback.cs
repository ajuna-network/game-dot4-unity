using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickFeedback : MonoBehaviour, IPointerDownHandler
{
    public CharacterController charController;


    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound(Sound.ButtonDown);

      
    }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     AudioManager.Instance.PlaySound(Sound.ButtonDown);
    // }
}