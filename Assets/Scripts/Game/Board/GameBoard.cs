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

        //maybe have this in skinviewer class
        public GameObject currentToken;


        Dictionary<Vector2, GameObject> BoardCells = new Dictionary<Vector2, GameObject>();
        private float cellSize;
        private BoardCell targetCell;
        private float boardContainerSize;

        // private Vector3 targetPos;
        private bool shouldExplode;


        [Header("Styling")] [SerializeField] private Color player1Color;
        [SerializeField] private Color player2Color;

        public Side selectedSide;
        public float selectedRow;
        public List<BoardCell> highlightedCells = new List<BoardCell>();

        public List<BoardCell> player1BombCells = new List<BoardCell>(2);
        public List<BoardCell> player2BombCells = new List<BoardCell>(2);

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
            PlaceBomb(1, pos);
        }

        public void PlaceBomb(int player, Vector2 pos)
        {
            var position = new int[]
            {
                (int) pos.x,
                (int) pos.y
            };
            BoardCell selectedCell = BoardCells[pos].gameObject.GetComponent<BoardCell>();


            switch (player)
            {
                case 1:
                    if (player1BombCells.Count != 3)
                    {
                        player1BombCells.Add(selectedCell);
                        selectedCell.SetCellType(CellType.PlayerBomb, pos);
                    }

                    break;

                case 2:
                    if (player2BombCells.Count != 3)
                    {
                        player2BombCells.Add(selectedCell);
                        selectedCell.SetCellType(CellType.EnemyBomb, pos);
                    }

                    break;
            }

            EngineManager.PlaceBomb(player, position);
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
                BoardCell selectedCell = BoardCells[new Vector2(cell[0], cell[1])].gameObject.GetComponent<BoardCell>();

                highlightedCells.Add(selectedCell);
                selectedCell.HighLight(GetCurrentPlayerColor(currentPlayer));
            }

            //Sets the target cell to move to Can be better
            foreach (var cell in selection)
            {
                BoardCell selectedCell = BoardCells[new Vector2(cell[0], cell[1])].gameObject.GetComponent<BoardCell>();

                foreach (var bomb in player1BombCells)
                {
                    if (selectedCell == bomb)
                    {
                        targetCell = bomb;
                        shouldExplode = true;
                        return;
                    }
                }

                targetCell = highlightedCells.Last().gameObject.GetComponent<BoardCell>();
                shouldExplode = false;

                foreach (var bomb in player2BombCells)
                {
                    if (selectedCell == bomb)
                    {
                        targetCell = bomb;
                        shouldExplode = true;
                        return;
                    }

                    targetCell = highlightedCells.Last().gameObject.GetComponent<BoardCell>();
                    shouldExplode = false;
                }
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
                targetCell = null;
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


        public void PlayOutro()
        {
            animator.SetTrigger("PlayOutro");
        }

        public bool IntroDone()
        {
            animator.SetTrigger("PlayIntro");

            if (animator.IsInTransition(0))
            {
                return false;
            }


            return animator.GetCurrentAnimatorStateInfo(0).IsTag("Intro") &&
                   animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1;
        }


        public void SpawnSkin(GameObject currentPlayerSkin)
        {
            currentToken = Instantiate(currentPlayerSkin, indicatorSlider.handleRect.position,
                Quaternion.LookRotation(indicatorSlider.handleRect.up, -indicatorSlider.handleRect.forward),
                boardContainer);
            currentToken.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
        }


        //return the board cell
        // public BoardCell GetTokenDestination()
        // {
        //     var bombCell = new Vector2();
        //     
        //
        //
        //     if (targetCell == highlightedCells.Last().gameObject.GetComponent<BoardCell>())
        //     {
        //         print("NoBomb");
        //         shouldExplode = false;
        //       //  return highlightedCells.Last().gameObject.GetComponent<BoardCell>();
        //     }
        //     else
        //     {
        //         print("BombInLane");
        //         shouldExplode = true;
        //       //  return BoardCells[new Vector2(targetCell.transform.position.x, targetCell.transform.position.y)].gameObject.GetComponent<BoardCell>();
        //     }
        // }

        //pass this the cell and get the pos
        public bool AnimateToken()
        {
            //  currentToken.transform.position = targetPos;

            // if ()
            // {
            //     //move to end
            //     shouldExplode = false;
            // }
            // else
            // {
            //     //move to target // target is assigned in highlight
            //     shouldExplode = true;
            // }

            while (currentToken.transform.position != targetCell.transform.position)
            {
                currentToken.transform.position =
                    Vector3.MoveTowards(currentToken.transform.position, targetCell.transform.position,
                        6 * Time.deltaTime);
                return true;
            }

            if (shouldExplode)
            {
                StartCoroutine(ExplodeBomb(targetCell));
            }

            // currentToken = null;
            return false;
        }

        IEnumerator ExplodeBomb(BoardCell targetCell)
        {
            yield return new WaitForSeconds(1);
            Destroy(currentToken);

            targetCell.SetCellType(CellType.Normal, new Vector2(0, 0));
            RemoveBomb(targetCell);
            yield return null;
        }

        void RemoveBomb(BoardCell bombCell)
        {
            foreach (var bomb in player1BombCells)
            {
                if (bombCell == bomb)
                {
                    player1BombCells.Remove(bomb);
                }
            }


            foreach (var bomb in player2BombCells)
            {
                if (bombCell == bomb)
                {
                    player2BombCells.Remove(bomb);
                }
            }
        }
    }
}