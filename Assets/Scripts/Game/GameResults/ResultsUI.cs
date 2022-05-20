using Ajuna.GenericGameEngine.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameResults
{
    public class ResultsUI : UICanvas
    {
        [SerializeField] TextMeshProUGUI resultTxt;
        public Button nextBtn;

        public void SetResultText(GameState state)
        {
            switch (state)
            {
                case GameState.TIMEOUT:
                    resultTxt.text ="You Ran Out Of Time";
                    break;
                case GameState.FINISHED:
                    resultTxt.text = "lost/won";
                    break;
            }
      
        }
    }
}
