using System;
using Ajuna.GenericGameEngine.Enums;
using Ajuna.NetApi.Model.Base;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using Game.Engine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameResults
{
    public class ResultsUI : UICanvas
    {
        public NetworkManager Network => NetworkManager.Instance;

        [SerializeField] TextMeshProUGUI resultHeaderTxt;
        [SerializeField] private TextMeshProUGUI scoreHeaderTxt;
        [SerializeField] private TextMeshProUGUI scoreTxt;
        public Button nextBtn;

        private void Awake()
        {
            scoreTxt.enabled = false;
        }


        public void SetResultHeader(GamePhase gamePhase, Dot4GPlayer winner)
        {
          
            switch (gamePhase)
            {
                // todo ad more states
                default:
                    resultHeaderTxt.text = SetHeader(Network.IsMe(winner.Address));
                    scoreTxt.enabled = true;
                    scoreTxt.text = GetScore();
                    break;
            }
        }

        string SetHeader(bool me)
        {
            return me ? "You won" : "You lost";
        }

        public string GetScore()
        {
            return "0".ToString();
        }
    }
}