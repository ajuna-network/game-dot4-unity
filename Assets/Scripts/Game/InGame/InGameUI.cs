using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.InGame
{
    public class InGameUI : UICanvas
    {
        [Header("Turns")] 
        [SerializeField] 
        public TextMeshProUGUI turnText;

        public Button turnBtn;

        public GameObject inputUI;

        public Timer timer;

        public void SetGameText(string text)
        {
            turnText.text = text;
        }

    }
}