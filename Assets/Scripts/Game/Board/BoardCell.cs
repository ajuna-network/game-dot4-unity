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
        [SerializeField] private Button toggle;
        [SerializeField] private Image spriteRenderer;
        [SerializeField] private Image bombSprite;
        [SerializeField] private Color defaultColor;

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

        public void UpdateCell(Dot4GObj dot4GObj, int row, int column)
        {
            var cell = dot4GObj.Board[(byte)row, (byte)column];

            switch (cell.Cell)
            {
                case Cell.Stone:
                    if (NetworkManager.Instance.IsMe(dot4GObj.Players[cell.PlayerIds.First()].Address))
                    {
                        SetCellType(CellType.PlayerToken, new Vector2(row, column));
                    }
                    else
                    {
                        SetCellType(CellType.EnemyToken, new Vector2(row, column));
                    }
                    break;


                case Cell.Block:
                    SetCellType(CellType.Obstacle, new Vector2(row, column));
                    break;

                case Cell.Empty:
                    SetCellType(CellType.Normal, new Vector2(row, column));
                    break;

                case Cell.Bomb:
                    foreach (var playerId in cell.PlayerIds)
                    {
                        if (NetworkManager.Instance.IsMe(dot4GObj.Players[playerId].Address))
                        {
                            SetCellType(CellType.PlayerBomb, new Vector2(row, column));
                        }
                        else
                        {
                            SetCellType(CellType.EnemyBomb, new Vector2(row, column));
                        }
                    }
                    break;
            }
        }

        public void SetCellType(CellType cellType, Vector2 pos)
        {
            cellPos = pos;
            gridTxt.text = pos.ToString();
            this.cellType = cellType;

            switch (cellType)
            {
                case CellType.Normal:
                    spriteRenderer.enabled = true;
                    bombSprite.enabled = false;
                    //  spriteRenderer.color = defaultColor;
                    // idTxt.enabled = false;
                    idTxt.text = "o";
                    idTxt.color = Color.white;
                    break;
                case CellType.Obstacle:
                    spriteRenderer.enabled = false;
                    // bombSprite.enabled = false;
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
                    // bombSprite.enabled = true;
                    // idTxt.enabled = true;
                    idTxt.text = "B2";
                    idTxt.color = Color.red;
                    break;
                case CellType.PlayerToken:
                    // bombSprite.enabled = false;
                    // idTxt.enabled = true;
                    idTxt.text = "P1";
                    idTxt.color = Color.blue;
                    break;
                case CellType.EnemyToken:
                    // bombSprite.enabled = false;
                    // idTxt.enabled = true;
                    idTxt.text = "P2";
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

        public void TurnColor(Color color)
        {
            spriteRenderer.color = color;
        }
        public void TurnDefaultColor()
        {
            spriteRenderer.color = defaultColor;
        }
    }
}