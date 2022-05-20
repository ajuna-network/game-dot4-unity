using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Board
{
    public enum CellType
    {
        Normal,
        Obstacle,
        Bomb
    }

    public class BoardCell : MonoBehaviour
    {
        [SerializeField] private Button toggle;
        [SerializeField] private Image spriteRenderer;
        [SerializeField] private Image bombSprite;
        [SerializeField] private Color defaultColor;

        private Material[] materials;

        public Vector2 cellPos;

        public static event Action<Vector2> OnCellSelected;

        private void Awake()
        {
            toggle.onClick.AddListener(Selected);
        }
        


        public void Init(CellType cellType, Vector2 pos)
        {
            cellPos = pos;

            switch (cellType)
            {
                case CellType.Normal:
                    spriteRenderer.enabled = true;
                    bombSprite.enabled = false;
                    spriteRenderer.color = defaultColor;
                    break;
                case CellType.Obstacle:
                    spriteRenderer.enabled = false;
                    bombSprite.enabled = false;
                    break;
                case CellType.Bomb:
                    bombSprite.enabled = true;
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
         void Selected( )
        {
           OnCellSelected?.Invoke(cellPos);
        }

    }
}