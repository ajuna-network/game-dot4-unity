
     using TMPro;
     using UnityEngine.UI;

     namespace Game.GameSetup
    {
        public class SetupUI : UICanvas
        {
            public Button readyButton;
            public Timer timer;

            public TextMeshProUGUI remainingPlayerBombs;

            public void SetPlayerBombCount(int _placedBombs)
            {
                remainingPlayerBombs.text = _placedBombs + "/3";
            }
        }
    }
