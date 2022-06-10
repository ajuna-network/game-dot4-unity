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
        private List<BoardCell> explosionCells = new List<BoardCell>();
        private float cellSize;
        private BoardCell targetCell;
        private float boardContainerSize;

        // private Vector3 targetPos;
        private bool shouldExplode = false;


        [Header("Styling")] [SerializeField] private Color player1Color;
        [SerializeField] private Color player2Color;

        public Side selectedSide;
        public float selectedRow;
        public List<BoardCell> highlightedCells = new List<BoardCell>();

        public Dictionary<Vector2, GameObject> tokenCells = new Dictionary<Vector2, GameObject>();
        public List<BoardCell> player1BombCells = new List<BoardCell>(2);
        public List<BoardCell> player2BombCells = new List<BoardCell>(2);

        private void OnEnable()
        {
            SelectionSlider.OnSliderSelected += SetSelectedSide;
            BoardCell.OnCellSelected += SetPlayerBomb;
            EngineManager.OnRefresh += RefreshCells;
        }

        private void OnDisable()
        {
            SelectionSlider.OnSliderSelected -= SetSelectedSide;
            BoardCell.OnCellSelected -= SetPlayerBomb;
            EngineManager.OnRefresh -= RefreshCells;
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

        void RefreshCells(FullState fullstate)
        {
            for (var row = 0; row < fullstate.Board.GetLength(0); row++)
            {
                for (int column = 0; column < fullstate.Board.GetLength(1); column++)
                {
                    BoardCells[new Vector2(row, column)].gameObject.GetComponent<BoardCell>()
                        .UpdateCell(fullstate, row, column);
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

            if (EngineManager.IsValidBomb(player, pos))
            {
                BoardCell selectedCell = BoardCells[pos].gameObject.GetComponent<BoardCell>();

                switch (player)
                {
                    case 1:
                        if (player1BombCells.Count != 3)
                        {
                            player1BombCells.Add(selectedCell);
                        }

                        break;

                    case 2:
                        if (player2BombCells.Count != 3)
                        {
                            player2BombCells.Add(selectedCell);
                        }

                        break;
                }

                EngineManager.PlaceBomb(player, position);
            }

          
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

        public void MakeMove()
        {
            EngineManager.MakeMove(selectedSide, selectedRow);


            // here i can get the target pos

            if (EngineManager.Fullstate.GroundZero[0] < byte.MaxValue)
            {
                print("Bomb In Row");
                shouldExplode = true;
                targetCell = BoardCells[new Vector2(EngineManager.Fullstate.GroundZero[0], EngineManager.Fullstate.GroundZero[1])].gameObject.GetComponent<BoardCell>();
                
            }
            else
            {
                shouldExplode = false;
                targetCell = highlightedCells.Last();
            }

            tokenCells.Add(new Vector2(targetCell.cellPos.x, targetCell.cellPos.y), currentToken);
        }

        // BoardCell GetBoardCell(Vector2 cellPos)
        // {
        //     return BoardCells[cellPos].gameObject.GetComponent<BoardCell>();
        // }

        //pass this the cell and get the pos
        public async Task AnimateToken()
        {
            await MoveToken();
            
            ClearHighlight();

            if (shouldExplode)
            {
             
                await ExplodeBomb(targetCell);
            }

            //used to be on clearhighlight, maybe this shouldnt be a varriable and just be passed in
            targetCell = null;
        }

        async Task MoveToken()
        {
            while (currentToken.transform.position != targetCell.transform.position)
            {
                currentToken.transform.position = Vector3.MoveTowards(currentToken.transform.position,
                    targetCell.transform.position,
                    10 * Time.deltaTime);
                await Task.Yield();
            }
        }

        async Task ExplodeBomb(BoardCell targetCell)
        {
            // yield return new WaitForSeconds(1);
            //  explosionCells = new List<BoardCell>();

            for (int x = (int) targetCell.cellPos.x - 1; x <= targetCell.cellPos.x + 1; x++)
            {
                for (int y = (int) targetCell.cellPos.y - 1; y <= targetCell.cellPos.y + 1; y++)
                {
                    if (x == (int) targetCell.cellPos.x && y == (int) targetCell.cellPos.y)
                        continue;


                  
                    if (BoardCells.ContainsKey(new Vector2(x,y)))
                    {
                        explosionCells.Add(BoardCells[new Vector2(x,y)].gameObject.GetComponent<BoardCell>());
                    }
                }
            }

            await FlashCells(explosionCells, 0.15f, 3);

           
//should remove surrounding bombs aswell
           //for each cell in bombs, see if that cell has a token and destroy it 
           foreach (var cell in explosionCells)
           {
               if (tokenCells.ContainsKey(new Vector2(cell.cellPos.x, cell.cellPos.y)))
               {
                   Destroy(tokenCells[new Vector2(cell.cellPos.x, cell.cellPos.y)].gameObject);
                   tokenCells.Remove(new Vector2(cell.cellPos.x, cell.cellPos.y));
               }
           }
            
           
           explosionCells.Clear();
           Destroy(currentToken);

        }


        async Task FlashCells(List<BoardCell> explosionCells, float speed, int flashes )
        {
            for (int i = 0; i < flashes; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(speed));

                foreach (var cell in explosionCells)
                {
                    BoardCells[cell.cellPos].gameObject.GetComponent<BoardCell>().TurnColor(Color.red);
                }

                await Task.Delay(TimeSpan.FromSeconds(speed));

                foreach (var cell in explosionCells)
                {
                    BoardCells[cell.cellPos].gameObject.GetComponent<BoardCell>().TurnDefaultColor();
                }

                Task.Yield();
            }
        }
    }
}