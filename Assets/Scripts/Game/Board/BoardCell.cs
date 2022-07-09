using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Ajuna.NetApi.Model.Base;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using Game.Engine;
using GameEngine.UnityMock;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Board
{
    public enum CellType
    {
        Normal,
        Obstacle,
        PlayerBomb,
        EnemyBomb,
        PlayerToken,
        EnemyToken
    }

    public class BoardCell : MonoBehaviour
    {
        [SerializeField] 
        private Button toggle;

        [SerializeField] 
        private Image spriteRenderer;
        
        [SerializeField] 
        private Image bombSprite;
        
        [SerializeField] 
        private Color defaultColor;

        //DEBUG
        public TextMeshProUGUI gridTxt;
        public TextMeshProUGUI idTxt;

        private Material[] materials;

        public Vector2 cellPos;
        public CellType cellType;

        public static event Action<Vector2> OnCellSelected;

        private void Awake()
        {
            toggle.onClick.AddListener(Selected);

        }
        public void DeSelect()
        {
            spriteRenderer.color = defaultColor;
        }

        public void HighLight(Color playerColor)
        {
            spriteRenderer.color = playerColor;
        }

        //should not be toggleable, so use button
        void Selected()
        {
            OnCellSelected?.Invoke(cellPos);
        }

        public void TurnColor(Color color)
        {
            spriteRenderer.color = color;
        }
        public void TurnDefaultColor()
        {
            spriteRenderer.color = defaultColor;
        }

        internal void SetToken(Color color, Vector2 pos)
        {
            cellPos = pos;
            gridTxt.text = pos.ToString();
            // idTxt.enabled = true;
            idTxt.text = "T1";
            idTxt.color = color;
        }

        internal void SetBlock(Vector2 pos)
        {
            cellPos = pos;
            gridTxt.text = pos.ToString();
            spriteRenderer.enabled = false;
            // bombSprite.enabled = false;
            idTxt.enabled = false;
            gridTxt.enabled = false;
            idTxt.text = "0";
        }

        internal void SetEmpty(Vector2 pos)
        {
            cellPos = pos;
            gridTxt.text = pos.ToString();
            spriteRenderer.enabled = true;
            bombSprite.enabled = false;
            //  spriteRenderer.color = defaultColor;
            // idTxt.enabled = false;
            idTxt.text = "o";
            idTxt.color = Color.white;
        }

        internal void SetBomb(Color color, Vector2 pos)
        {
            cellPos = pos;
            gridTxt.text = pos.ToString();
            bombSprite.enabled = true;
            bombSprite.color = color;
            // idTxt.enabled = true;
            idTxt.text = "BO";
            idTxt.color = color;
        }
    }
}