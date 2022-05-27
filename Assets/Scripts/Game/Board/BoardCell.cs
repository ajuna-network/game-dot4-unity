using System;
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
        EnemyBomb
    }

    public class BoardCell : MonoBehaviour
    {
        [SerializeField] private Button toggle;
        [SerializeField] private Image spriteRenderer;
        [SerializeField] private Image bombSprite;
        [SerializeField] private Color defaultColor;

        //DEBUG
        public TextMeshProUGUI gridTxt;
        public TextMeshProUGUI idTxt;

        private Material[] materials;

        public Vector2 cellPos;

        public static event Action<Vector2> OnCellSelected;

        private void Awake()
        {
            toggle.onClick.AddListener(Selected);
        }
        
        public void UpdateCell(FullState fullstate, int row, int column)
        {
            if (fullstate.Board[(byte) row, (byte) column] == 9)
            {
                SetCellType(CellType.Obstacle, new Vector2(row, column));
            }

            if (fullstate.Board[(byte) row, (byte) column] == 0)
            {
                SetCellType(CellType.Normal, new Vector2(row, column));
            }
            //
                    

            if (fullstate.Board[(byte) row, (byte) column] == 12)
            {
                SetCellType(CellType.EnemyBomb, new Vector2(row, column));
            }

            if (fullstate.Board[(byte) row, (byte) column] == 11)
            {
                SetCellType(CellType.PlayerBomb, new Vector2(row, column));
            }
        }
        
        
        void SetCellType(CellType cellType, Vector2 pos)
        {
            cellPos = pos;
            gridTxt.text = pos.ToString();

            switch (cellType)
            {
                case CellType.Normal:
                    spriteRenderer.enabled = true;
                    bombSprite.enabled = false;
                    spriteRenderer.color = defaultColor;
                    // idTxt.enabled = false;
                    idTxt.text = "";
                    break;
                case CellType.Obstacle:
                    spriteRenderer.enabled = false;
                    bombSprite.enabled = false;
                    idTxt.enabled = false;
                    gridTxt.enabled = false;
                    idTxt.text = "0";
                    break;
                case CellType.PlayerBomb:
                    bombSprite.enabled = true;
                    // idTxt.enabled = true;
                    idTxt.text = "B1";
                    idTxt.color = Color.blue;
                    break;
                case CellType.EnemyBomb:
                    bombSprite.enabled = true;
                    // idTxt.enabled = true;
                    idTxt.text = "B2";
                    idTxt.color = Color.red;
                    break;
            }
        }

        public void DeSelect()
        {
            spriteRenderer.color = defaultColor;
        }

        public void HighLight(Color currentPlayerColors)
        {
            spriteRenderer.color = currentPlayerColors;
        }

//should not be toggleable, so use button
        void Selected()
        {
            OnCellSelected?.Invoke(cellPos);
        }
    }
}