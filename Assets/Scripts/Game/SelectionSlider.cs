using Ajuna.NetApi.Model.Base;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


    public class SelectionSlider : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Slider slider;
        public Side side;

        public static event Action<Side, float> OnSliderSelected; 

        private void Awake()
        {
            slider = gameObject.GetComponent<Slider>();
            slider.handleRect.gameObject.SetActive(false);
           
            
            slider.onValueChanged.AddListener(ValueChanged);
        }

        private void ValueChanged(float row)
        {
            OnSliderSelected?.Invoke(side,row);
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            slider.handleRect.gameObject.SetActive(true);
           
        }
        
        public void OnDeselect(BaseEventData eventData)
        {
            slider.handleRect.gameObject.SetActive(false);
        }
    }

