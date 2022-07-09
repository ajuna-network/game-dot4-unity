using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ajuna.NetApi.Model.Base;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Board
{
    [ExecuteInEditMode]
    public class GameBoard : MonoBehaviour
    {
        public NetworkManager Network => NetworkManager.Instance;

        public Dot4GObj CurrentBoard;

        [Header("Refrences")]
        [SerializeField]
        private GameObject cellPrefab;

        [SerializeField]
        private GridLayoutGroup gridLayout;

        [SerializeField]
        private RectTransform boardCellsContainer;

        [SerializeField]
        private RectTransform indicatorContainer;

        [SerializeField]
        private RectTransform boardContainer;

        [SerializeField] 
        private GameObject boardIsland3D;

        public GraphicRaycaster boardRaycaster;
        public Slider indicatorSlider;

        public Animator animator;

        //maybe have this in skinviewer class
        public GameObject currentToken;


        [Header("Tokens")]
        public Achievement player1Token;
        public Achievement player2Token;

        Dictionary<Vector2, GameObject> BoardCells = new Dictionary<Vector2, GameObject>();
        private List<BoardCell> explosionCells = new List<BoardCell>();
        private float cellSize;
        private BoardCell targetCell;
        private float boardContainerSize;

        // private Vector3 targetPos;
        private bool shouldExplode = false;


        [Header("Styling")]
        [SerializeField]
        private Color player1Color;

        [SerializeField]
        private Color player2Color;

        public Side selectedSide;
        public float selectedRow;
        public List<BoardCell> highlightedCells = new List<BoardCell>();

        public Dictionary<Vector2, GameObject> tokenCells = new Dictionary<Vector2, GameObject>();
        public List<Vector2> bombs = new List<Vector2>();
        public List<BoardCell> player1BombCells = new List<BoardCell>(2);
        public List<BoardCell> player2BombCells = new List<BoardCell>(2);

        private Dot4GStateHelper _stateHelper;

        private bool _waitAnimation = false;

        public Task _explodeTask, _tokenTask;

        public List<Vector2> _bombsToExploded;

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

        private void Awake()
        {
            ToggleIndicator(false);
            _stateHelper = new Dot4GStateHelper();
        }

        private void Update()
        {
            //this should be in start, but effect should be tested
            LayoutGrid();
        }

        #region Board Gen + Layout


        public void Clear()
        {
            foreach (Transform child in boardCellsContainer)
            {
                Destroy(child.gameObject);
                BoardCells.Clear();
            }
        }

        public void UpdateBoard(Dot4GObj dot4GObj)
        {
            // don't update board during bomb exlosion
            if (_explodeTask != null && !_explodeTask.IsCompleted)
            {
                return;
            }
            _explodeTask = null;

            // don't update board during token animation
            if (_tokenTask != null && !_tokenTask.IsCompleted)
            {
                return;
            }
            _tokenTask = null;

            CurrentBoard = dot4GObj;

            var diff = _stateHelper.NewState(dot4GObj);

            // if diff is null we received first time a state so we generate
            if (diff is null)
            {
                GenerateBoard();
            }
            else //if (diff.ChangeFlag)
            {
                RefreshCells(diff);
            }
        }

        public void GenerateBoard()
        {
            for (var row = 0; row < CurrentBoard.Board.GetLength(0); row++)
            {
                for (int column = 0; column < CurrentBoard.Board.GetLength(1); column++)
                {
                    var boarCell = Instantiate(cellPrefab, boardCellsContainer);
                    BoardCells.Add(new Vector2(row, column), boarCell);

                    var cell = CurrentBoard.Board[row, column];
                    UpdatedBoardCell(cell, row, column);

                    var posVec = new Vector2(row, column);
                    if (cell.Cell == Cell.Stone)
                    {
                        var token = SpawnToken(cell.PlayerIds[0]);

                        BoardCell selectedCell = BoardCells[posVec].gameObject.GetComponent<BoardCell>();
                        token.transform.parent = selectedCell.transform;
                        token.transform.localPosition = Vector3.zero;

                        tokenCells.Add(posVec, token);
                    }
                    else if (cell.Cell == Cell.Bomb)
                    {
                        bombs.Add(posVec);
                    }
                }
            }
        }

        public void RefreshCells(Dot4GDiff diff)
        {
            var bombsToExplode = new List<Vector2>();

            for (var row = 0; row < CurrentBoard.Board.GetLength(0); row++)
            {
                for (int column = 0; column < CurrentBoard.Board.GetLength(1); column++)
                {
                    var cell = CurrentBoard.Board[row, column];
                    UpdatedBoardCell(cell, row, column);

                    var posVec = new Vector2(row, column);
                    switch (cell.Cell)
                    {
                        case Cell.Stone:
                            // create new token played
                            if (!tokenCells.ContainsKey(posVec))
                            {
                                var token = SpawnToken(cell.PlayerIds[0]);
                                BoardCell selectedCell = BoardCells[posVec].gameObject.GetComponent<BoardCell>();
                                token.transform.parent = selectedCell.transform;
                                token.transform.localPosition = Vector3.zero;
                                tokenCells.Add(posVec, token);
                            }
                            break;

                        case Cell.Empty:
                            // remove token destroyed
                            if (tokenCells.ContainsKey(posVec))
                            {
                                //Destroy(tokenCells[posVec].gameObject);
                                //tokenCells.Remove(posVec);
                            }
                            // remove bombs exploded
                            if (bombs.Contains(posVec))
                            {
                                bombsToExplode.Add(posVec);
                                bombs.Remove(posVec);
                            }
                            break;
                        case Cell.Bomb:
                            if (!bombs.Contains(posVec))
                            {
                                bombs.Add(posVec);
                            }
                            break;
                    }
                }
            }

            // execute bomb explosions ...
            bombsToExplode.ForEach(x =>
            {
                _explodeTask = ExplodeBomb(BoardCells[x].gameObject.GetComponent<BoardCell>());
            });
        }

        private void UpdatedBoardCell(Dot4GCell cell, int row, int column)
        {
            var boardCellComponent = BoardCells[new Vector2(row, column)].gameObject.GetComponent<BoardCell>();

            switch (cell.Cell)
            {
                case Cell.Stone:
                    boardCellComponent.SetToken(GetCurrentPlayerColor(cell.PlayerIds.First()), new Vector2(row, column));
                    break;

                case Cell.Block:
                    boardCellComponent.SetBlock(new Vector2(row, column));
                    break;

                case Cell.Empty:
                    boardCellComponent.SetEmpty(new Vector2(row, column));
                    break;

                case Cell.Bomb:

                    if (cell.PlayerIds.Count > 1)
                    {
                        boardCellComponent.SetBomb(Color.yellow, new Vector2(row, column));
                    }
                    else
                    {
                        boardCellComponent.SetBomb(GetCurrentPlayerColor(cell.PlayerIds.First()), new Vector2(row, column));
                    }

                    break;
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

            var boardSize = cellSize * 10 / 2;
            boardIsland3D.transform.localScale = new Vector3(boardSize, boardSize, boardSize);
        }

        #endregion

        //for player input
        void SetPlayerBomb(Vector2 pos)
        {
            //TODO validation check is done below aswell, not removing the bottom one now to avoid breaking the AI, but should be tested and removed
            if (CurrentBoard.ValidateBomb((int)pos.x, (int)pos.y))
            {

                PlaceBomb(pos);
                AudioManager.Instance.PlaySound(Sound.ValidMove);
            }
            else
            {
                AudioManager.Instance.PlaySound(Sound.InvalidMove);
            }
        }

        public void PlaceBomb(Vector2 pos)
        {
            var me = CurrentBoard.Players.Where(p => Network.IsMe(p.Address)).ToList();
            if (me.Count == 1 && me[0].Bombs == 0)
            {
                Debug.Log("No more bombs to place!");
                return;
            }

            var bombTask = Network.Dot4GClient.BombAsync((int)pos.x, (int)pos.y);
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

            var selection = CurrentBoard.GetRay(side, (int)row);

            ClearHighlight();

            foreach (var cell in selection)
            {
                BoardCell selectedCell = BoardCells[new Vector2(cell[0], cell[1])].gameObject.GetComponent<BoardCell>();

                highlightedCells.Add(selectedCell);
                selectedCell.HighLight(GetCurrentPlayerColor(CurrentBoard.Next));
            }

            indicatorSlider.handleRect.GetComponent<Image>().color = GetCurrentPlayerColor(CurrentBoard.Next);

        }

        public void ClearHighlight()
        {
            highlightedCells.ForEach(p => p.DeSelect());
            highlightedCells.Clear();
        }

        public Color GetCurrentPlayerColor(int currentPlayer)
        {
            return currentPlayer switch
            {
                0 => player1Color,
                1 => player2Color,
                _ => throw new Exception("Player Color Not Found"),
            };
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

        GameObject GetCurrentPlayerSkin(int currentPlayer)
        {
            switch (currentPlayer)
            {
                case 0:
                    return player1Token.tokenPrefab;
                case 1:
                    return player2Token.tokenPrefab;
            }

            return null;
        }

        public GameObject SpawnToken(int currentPlayer)
        {
            var token = Instantiate(GetCurrentPlayerSkin(currentPlayer), indicatorSlider.handleRect.position,
                Quaternion.LookRotation(indicatorSlider.handleRect.up, -indicatorSlider.handleRect.forward),
                boardContainer);
            token.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            return token;
        }

        public void SpawnSkin(int currentPlayer)
        {
            currentToken = SpawnToken(currentPlayer);
        }

        public void MakeMove()
        {
            _ = Network.Dot4GClient.StoneAsync(selectedSide, (int)selectedRow);

            targetCell = highlightedCells.Last();

            var key = new Vector2(targetCell.cellPos.x, targetCell.cellPos.y);
            if (!tokenCells.ContainsKey(key))
            {
                tokenCells.Add(new Vector2(targetCell.cellPos.x, targetCell.cellPos.y), currentToken);
            }

            _tokenTask = AnimateToken();
        }


        public async Task AnimateToken()
        {
            // do token movement
            while (currentToken.transform.position != targetCell.transform.position)
            {
                currentToken.transform.position = Vector3.MoveTowards(currentToken.transform.position,
                    targetCell.transform.position,
                    10 * Time.deltaTime);
                await Task.Yield();
            }

            ClearHighlight();
            ToggleIndicator(false);

            if (shouldExplode)
            {
                await ExplodeBomb(targetCell);
            }

            //used to be on clearhighlight, maybe this shouldnt be a varriable and just be passed in
            targetCell = null;

            _waitAnimation = false;
        }

        private async Task ExplodeBomb(BoardCell targetCell)
        {
            for (int x = (int)targetCell.cellPos.x - 1; x <= targetCell.cellPos.x + 1; x++)
            {
                for (int y = (int)targetCell.cellPos.y - 1; y <= targetCell.cellPos.y + 1; y++)
                {
                    if (BoardCells.ContainsKey(new Vector2(x, y)))
                    {
                        explosionCells.Add(BoardCells[new Vector2(x, y)].gameObject.GetComponent<BoardCell>());
                    }
                }
            }

            await FlashCells(explosionCells, 0.15f, 3);

            foreach (var cell in explosionCells)
            {
                if (tokenCells.ContainsKey(new Vector2(cell.cellPos.x, cell.cellPos.y)))
                {
                    Destroy(tokenCells[new Vector2(cell.cellPos.x, cell.cellPos.y)].gameObject);
                    tokenCells.Remove(new Vector2(cell.cellPos.x, cell.cellPos.y));
                }
            }
            
            AudioManager.Instance.PlaySound(Sound.Detonate);
            explosionCells.Clear();
        }


        async Task FlashCells(List<BoardCell> explosionCells, float speed, int flashes)
        {
            for (int i = 0; i < flashes; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(speed));

                AudioManager.Instance.PlaySound(Sound.BombBeep);

                foreach (var cell in explosionCells)
                {
                    BoardCells[cell.cellPos].gameObject.GetComponent<BoardCell>().TurnColor(Color.red);
                }

                await Task.Delay(TimeSpan.FromSeconds(speed));

                foreach (var cell in explosionCells)
                {
                    BoardCells[cell.cellPos].gameObject.GetComponent<BoardCell>().TurnDefaultColor();
                }
            }
        }
    }
}