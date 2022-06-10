using System;
using Ajuna.GenericGameEngine.Enums;
using Game.Engine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameResults
{
    public class ResultsUI : UICanvas
    {
     //   [SerializeField] GameObject outOfTimeCnt;
       // [SerializeField] GameObject scoreCnt;

        [SerializeField] TextMeshProUGUI resultHeaderTxt;
        [SerializeField] private TextMeshProUGUI scoreHeaderTxt;
        [SerializeField] private TextMeshProUGUI scoreTxt;
        public Button nextBtn;

        private void Awake()
        {
            scoreTxt.enabled = false;
        }


        public void SetResultHeader(GameState state)
        {
          
            switch (state)
            {
                case GameState.TIMEOUT:
                    resultHeaderTxt.text = "You Ran Out Of Time";
                    scoreTxt.enabled = true;
                    scoreTxt.text = GetScore();
                    break;

                case GameState.FINISHED:
                    resultHeaderTxt.text = SetHeader();
                    scoreTxt.enabled = true;
                    scoreTxt.text = GetScore();
                    break;
            }
        }

        string SetHeader()
        {
            return EngineManager.Fullstate.CurrentPlayer == 1 ? "You won" : "You lost";
        }

        public string GetScore()
        {
            return EngineManager.Fullstate.Scores[0].Score.ToString();
        }
    }
}