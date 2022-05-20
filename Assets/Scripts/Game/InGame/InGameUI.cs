using System;
using System.Collections;
using Game.Board;
using Game.Engine;
using GameEngine.GravityDot;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.InGame
{
    public class InGameUI : UICanvas
    {
        [Header("Turns")] [SerializeField] 
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