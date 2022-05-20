using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Game.Engine;
using GameEngine.GravityDot;
using GameEngine.UnityMock;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Board
{
    [ExecuteInEditMode]
    public class GameBoard : MonoBehaviour
    {
        [Header("Refrences")] [SerializeField] GameObject cellPrefab;
        [SerializeField] GridLayoutGroup gridLayout;
        [SerializeField] RectTransform boardCellsContainer;
        [SerializeField] RectTransform indicatorContainer;
        [SerializeField] RectTransform boardContainer;
        public Slider indicatorSlider;
        public Animator animator;


        Dictionary<Vector2, GameObject> BoardCells = new Dictionary<Vector2, GameObject>();
        private float cellSize;
        private float boardContainerSize;


        [Header("Styling")] [SerializeField] private Color player1Color;
        [SerializeField] private Color player2Color;

        public Side selectedSide;
        public float selectedRow;
        public List<BoardCell> highlightedCells = new List<BoardCell>();

        public List<BoardCell> player1BombCells = new List<BoardCell>(3);
        public List<BoardCell> player2BombCells = new List<BoardCell>(3);

        private void OnEnable()
        {
            SelectionSlider.OnSliderSelected += SetSelectedSide;
            BoardCell.OnCellSelected += SetSelectedBomb;
        }

        private void OnDisable()
        {
            SelectionSlider.OnSliderSelected -= SetSelectedSide;
            BoardCell.OnCellSelected -= SetSelectedBomb;
        }

        //bomb cell visuals should be done here
        //save to a list like highlighting works

        private void Awake()
        {
            ToggleIndicator(false);
        }

        private void Update()
        {
            //this should be in start, but effect should be tested
            LayoutGrid();
        }

        #region Board Gen + Layout

        public void GenerateBoard(FullState fullstate)
        {
            foreach (Transform child in boardCellsContainer)
            {
                Destroy(child.gameObject);
                BoardCells.Clear();
            }


            for (var row = 0; row < fullstate.Board.GetLength(0); row++)
            {
                for (int column = 0; column < fullstate.Board.GetLength(1); column++)
                {
                    var cell = Instantiate(cellPrefab, boardCellsContainer);
                    BoardCells.Add(new Vector2(row, column), cell);


                    if (fullstate.Board[(byte) row, (byte) column] == 9)
                    {
                        cell.GetComponent<BoardCell>().Init(CellType.Obstacle, new Vector2(row, column));
                    }

                    if (fullstate.Board[(byte) row, (byte) column] == 0)
                    {
                        cell.GetComponent<BoardCell>().Init(CellType.Normal, new Vector2(row, column));
                    }
                    
                    // if (fullstate.Board[(byte) row, (byte) column] == 19)
                    // {
                    //     cell.GetComponent<BoardCell>().Init(false, new Vector2(row, column));
                    // }
                }
            }
        }

        void LayoutGrid()
        {
            var canvas = gameObject.GetComponent<RectTransform>();


            if (canvas.rect.height >= canvas.rect.width)
            {
                boardContainerSize = canvas.rect.width;
            }
            else
            {
                boardContainerSize = canvas.rect.height;
            }

            boardContainer.sizeDelta = new Vector2(boardContainerSize, boardContainerSize);

            cellSize = boardContainerSize / 12;


            gridLayout.cellSize = new Vector2(cellSize, cellSize);


            indicatorSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(0, cellSize);
            indicatorSlider.handleRect.sizeDelta = new Vector2(cellSize, cellSize);

            indicatorContainer.offsetMin = new Vector2(cellSize + (cellSize / 2), 0);
            indicatorContainer.offsetMax = new Vector2(-cellSize - (cellSize / 2), 0);
        }

        #endregion

//for player input
        void SetSelectedBomb( Vector2 pos)
        {
            if (EngineManager.IsValidBomb(1, pos))
            {
                HighlightBombCell(1, pos);
                EngineManager.PlaceBomb(1, new int[]
                {
                    (int)pos.x,
                    (int)pos.y
                });
            }
        }


        public void HighlightBombCell(int player, Vector2 pos)
        {
            BoardCell selectedCell = BoardCells[pos].gameObject.GetComponent<BoardCell>();
            selectedCell.Init(CellType.Bomb, pos);
            
            switch (player)
            {
                case 1:
                    player1BombCells.Add(selectedCell);
                    selectedCell.HighLight(GetCurrentPlayerColor(1));
                    break;
                case 2:
                    player2BombCells.Add(selectedCell);
                    selectedCell.HighLight(GetCurrentPlayerColor(2));
                    break;
            }


            //only highlight player ones
        }

        //needs a deselect aswell


        #region Selection

        void SetIndicatorSide(Side side, float row)
        {
            indicatorSlider.value = row;


            var newRotation = new Vector3();
            var newScale = new Vector3(1, 1, 1);

            switch (side)
            {
                case Side.South:
                    newRotation.z = 90f;
                    newScale.x = 1;
                    break;
                case Side.East:
                    newRotation.z = 180f;
                    newScale.x = -1;
                    break;
                case Side.North:
                    newRotation.z = 270f;
                    newScale.x = -1;
                    break;
                case Side.West:
                    newRotation.z = 360f;
                    newScale.x = 1;
                    break;
            }

            indicatorContainer.localRotation = Quaternion.Euler(newRotation);
            indicatorContainer.localScale = newScale;

            if (currentToken != null)
            {
                currentToken.transform.position = indicatorSlider.handleRect.position;
            }
        }

        public void SetSelectedSide(Side side, float row)
        {
            selectedSide = side;
            selectedRow = row;

            SetIndicatorSide(side, row);
            

            HighlightSelection(EngineManager.UpdateSelection(side, row), EngineManager.Fullstate.CurrentPlayer);
        }

        void HighlightSelection(List<int[]> selection, int currentPlayer)
        {
            ClearHighlight();

            foreach (var cell in selection)
            {
                BoardCell selectedCell = BoardCells[new Vector2(cell[0], cell[1])].gameObject
                    .GetComponent<BoardCell>();

                highlightedCells.Add(selectedCell);
                selectedCell.HighLight(GetCurrentPlayerColor(currentPlayer));
            }

            indicatorSlider.handleRect.GetComponent<Image>().color = GetCurrentPlayerColor(currentPlayer);
        }

        public void ClearHighlight()
        {
            if (highlightedCells.Count != 0)
            {
                foreach (var cell in highlightedCells)
                {
                    cell.DeSelect();
                }

                highlightedCells.Clear();
            }
        }

        Color GetCurrentPlayerColor(int currentPlayer)
        {
            switch (currentPlayer)
            {
                case 1:
                    return player1Color;


                case 2:
                    return player2Color;
            }

            throw new Exception("Player Color Not Found");
        }

        #endregion


        public void ToggleIndicator(bool toggle)
        {
            indicatorSlider.gameObject.SetActive(toggle);
        }

        public void PlayIntro()
        {
            animator.SetTrigger("PlayIntro");
        }

        public void PlayOutro()
        {
            animator.SetTrigger("PlayOutro");
        }

        public bool IsPlaying()
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
        }

        //maybe have this in skinviewer class
        public GameObject currentToken;


        public void SpawnSkin(GameObject currentPlayerSkin)
        {
            currentToken = Instantiate(currentPlayerSkin, indicatorSlider.handleRect.position,
                Quaternion.LookRotation(indicatorSlider.handleRect.up, -indicatorSlider.handleRect.forward),
                boardContainer);
            currentToken.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
        }

        public bool AnimateToken()
        {
            var target = highlightedCells.Last().gameObject.transform.position;


            while (currentToken.transform.position != target)
            {
                currentToken.transform.position =
                    Vector3.MoveTowards(currentToken.transform.position, target, 12 * Time.deltaTime);
                return true;
            }

            currentToken = null;
            return false;
        }
    }
}