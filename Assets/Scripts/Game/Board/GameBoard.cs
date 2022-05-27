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
        public GraphicRaycaster boardRaycaster;
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
            BoardCell.OnCellSelected += SetPlayerBomb;
        }

        private void OnDisable()
        {
            SelectionSlider.OnSliderSelected -= SetSelectedSide;
            BoardCell.OnCellSelected -= SetPlayerBomb;
        }

        //bomb cell visuals should be done here
        //save to a list like highlighting works

        private void Awake()
        {
            ToggleIndicator(false);
            //DEBUG
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

                    cell.GetComponent<BoardCell>().UpdateCell(fullstate, row, column);


                    // if (fullstate.Board[(byte) row, (byte) column] == 9)
                    // {
                    //     cell.GetComponent<BoardCell>().Init(CellType.Obstacle, new Vector2(row, column));
                    // }
                    //
                    // if (fullstate.Board[(byte) row, (byte) column] == 0)
                    // {
                    //     cell.GetComponent<BoardCell>().Init(CellType.Normal, new Vector2(row, column));
                    // }
                    // //
                    //
                    //
                    // if (fullstate.Board[(byte) row, (byte) column] == 12)
                    // {
                    //     cell.GetComponent<BoardCell>().Init(CellType.EnemyBomb, new Vector2(row, column));
                    // }
                    //
                    // if (fullstate.Board[(byte) row, (byte) column] == 11)
                    // {
                    //     cell.GetComponent<BoardCell>().Init(CellType.PlayerBomb, new Vector2(row, column));
                    // }
                }
            }
        }

        public void UpdateCells(FullState fullstate)
        {
            for (var row = 0; row < fullstate.Board.GetLength(0); row++)
            {
                for (int column = 0; column < fullstate.Board.GetLength(1); column++)
                {
                    BoardCell cell = BoardCells[new Vector2(row, column)].gameObject.GetComponent<BoardCell>();

                    cell.UpdateCell(fullstate, row, column);
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
        void SetPlayerBomb(Vector2 pos)
        {
            if (EngineManager.IsValidBomb(1, pos))
            {
                PlaceBomb(1, pos);
            }
        }

        public void PlaceBomb(int player, Vector2 pos)
        {
            var position = new int[]
            {
                (int) pos.x,
                (int) pos.y
            };

            EngineManager.PlaceBomb(player, position);
            UpdateCells(EngineManager.Fullstate);
        }


        public void HighlightBombCell(int player, Vector2 pos)
        {
            BoardCell selectedCell = BoardCells[pos].gameObject.GetComponent<BoardCell>();
            /// selectedCell.Init(CellType.Bomb, pos);

            switch (player)
            {
                case 1:
                    player1BombCells.Add(selectedCell);
                    // selectedCell.HighLight(GetCurrentPlayerColor(1));

                    break;
                case 2:
                    player2BombCells.Add(selectedCell);
                    // selectedCell.HighLight(GetCurrentPlayerColor(2));
                    break;
            }


            //only highlight player ones
        }

        public bool AllBombsPlaced()
        {
            return player1BombCells.Count == 3;
        }


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

        public bool AnimateToken(Vector3 targetPos)
        {
    


            while (currentToken.transform.position != targetPos)
            {
                currentToken.transform.position =
                    Vector3.MoveTowards(currentToken.transform.position, targetPos, 12 * Time.deltaTime);

                print("True");
                return true;
            }

          
          
            currentToken = null;
            print("False");
            return false;

            //if there is a bomb, move to it
            //else move to last cell
        }

        public Vector3 GetTargetPos()
        {
            EngineManager.MakeMove(selectedSide, selectedRow);
            var bombCell = EngineManager.CheckForBombs();

            var targetPos = new Vector3();

            if (bombCell.x == 255.0)
            {
                print("NoBomb");
                return highlightedCells.Last().gameObject.transform.position;
            }
            else
            {
                print("BombInLane");
                return BoardCells[new Vector2(bombCell.x, bombCell.y)].gameObject.transform.position;
            }
        }

        // IEnumerator MoveToCell(Vector3 cellPos)
        // {
        //     while (currentToken.transform.position != cellPos)
        //     {
        //         currentToken.transform.position =
        //             Vector3.MoveTowards(currentToken.transform.position, cellPos, 12 * Time.deltaTime);
        //
        //         yield return null;
        //     }
        // }
    }
}