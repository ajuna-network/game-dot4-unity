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

        [Header("Refrences")][SerializeField] GameObject cellPrefab;
        [SerializeField] GridLayoutGroup gridLayout;
        [SerializeField] RectTransform boardCellsContainer;
        [SerializeField] RectTransform indicatorContainer;
        [SerializeField] RectTransform boardContainer;
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


        [Header("Styling")][SerializeField] private Color player1Color;
        [SerializeField] private Color player2Color;

        public Side selectedSide;
        public float selectedRow;
        public List<BoardCell> highlightedCells = new List<BoardCell>();

        public Dictionary<Vector2, GameObject> tokenCells = new Dictionary<Vector2, GameObject>();
        public List<Vector2> bombs = new List<Vector2>();
        public List<BoardCell> player1BombCells = new List<BoardCell>(2);
        public List<BoardCell> player2BombCells = new List<BoardCell>(2);

        private Dot4GStateHelper _stateHelper;

        private bool _waitAnimation = false;

        public bool IsAnimating => _waitAnimation;

        private void OnEnable()
        {
            SelectionSlider.OnSliderSelected += SetSelectedSide;
            BoardCell.OnCellSelected += SetPlayerBomb;
            //EngineManager.OnRefresh += RefreshCells;
        }

        private void OnDisable()
        {
            SelectionSlider.OnSliderSelected -= SetSelectedSide;
            BoardCell.OnCellSelected -= SetPlayerBomb;
            //EngineManager.OnRefresh -= RefreshCells;
        }

        //bomb cell visuals should be done here
        //save to a list like highlighting works

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
            CurrentBoard = dot4GObj;

            var diff = _stateHelper.NewState(dot4GObj);
            if (diff is null)
            {
                GenerateBoard();
            } 
            else
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
                    var cell = Instantiate(cellPrefab, boardCellsContainer);
                    BoardCells.Add(new Vector2(row, column), cell);

                    cell.GetComponent<BoardCell>().UpdateCell(CurrentBoard, row, column);

                }
            }

            for (var row = 0; row < CurrentBoard.Board.GetLength(0); row++)
            {
                for (int column = 0; column < CurrentBoard.Board.GetLength(1); column++)
                {
                    var cell = CurrentBoard.Board[row, column];
                    var posVec = new Vector2(row, column);
                    if (cell.Cell == Cell.Stone)
                    {
                        var token = SpawnToken(cell.PlayerIds[0]);

                        BoardCell selectedCell = BoardCells[posVec].gameObject.GetComponent<BoardCell>();
                        token.transform.parent = selectedCell.transform;
                        token.transform.localPosition = Vector3.zero;

                        tokenCells.Add(posVec, token);
                    } else if (cell.Cell == Cell.Bomb)
                    {
                        bombs.Add(posVec);
                    }
                }
            }
        }



        public void RefreshCells(Dot4GDiff diff)
        {
            if (_waitAnimation)
            {
                return;
            }

            var tokenList = tokenCells.Keys.ToList();

            for (var row = 0; row < CurrentBoard.Board.GetLength(0); row++)
            {
                for (int column = 0; column < CurrentBoard.Board.GetLength(1); column++)
                {
                    BoardCells[new Vector2(row, column)].gameObject
                        .GetComponent<BoardCell>()
                        .UpdateCell(CurrentBoard, row, column);

                    var cell = CurrentBoard.Board[row, column];
                    var posVec = new Vector2(row, column);
                    if (cell.Cell == Cell.Stone)
                    {
                        if (!tokenCells.ContainsKey(posVec))
                        {
                            var token = SpawnToken(cell.PlayerIds[0]);
                            BoardCell selectedCell = BoardCells[posVec].gameObject.GetComponent<BoardCell>();
                            token.transform.parent = selectedCell.transform;
                            token.transform.localPosition = Vector3.zero;
                            tokenCells.Add(posVec, token);
                        }

                        tokenList.Remove(posVec);
                    }
                }
            }

            if (CurrentBoard.GamePhase == GamePhase.Play && diff.bombsDiffType == DiffType.Le)
            {
                diff.bombsDiff.ForEach(x =>
                {
                    var posVec = new Vector2(x.Position[0], x.Position[1]);
                    ExplodeBomb(BoardCells[posVec].gameObject.GetComponent<BoardCell>());
                });
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

            _ = Network.Dot4GClient.BombAsync((int)pos.x, (int)pos.y);
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

        Color GetCurrentPlayerColor(int currentPlayer)
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
            _waitAnimation = true;

            _ = Network.Dot4GClient.StoneAsync(selectedSide, (int)selectedRow);

            targetCell = highlightedCells.Last();

            var key = new Vector2(targetCell.cellPos.x, targetCell.cellPos.y);
            if (!tokenCells.ContainsKey(key))
            {
                tokenCells.Add(new Vector2(targetCell.cellPos.x, targetCell.cellPos.y), currentToken);
            }

            AnimateToken();
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

        async Task ExplodeBomb(BoardCell targetCell)
        {
            // yield return new WaitForSeconds(1);
            //  explosionCells = new List<BoardCell>();

            for (int x = (int)targetCell.cellPos.x - 1; x <= targetCell.cellPos.x + 1; x++)
            {
                for (int y = (int)targetCell.cellPos.y - 1; y <= targetCell.cellPos.y + 1; y++)
                {
                    // if (x == (int) targetCell.cellPos.x && y == (int) targetCell.cellPos.y)
                    //     continue;


                    if (BoardCells.ContainsKey(new Vector2(x, y)))
                    {
                        explosionCells.Add(BoardCells[new Vector2(x, y)].gameObject.GetComponent<BoardCell>());
                    }
                }
            }

            await FlashCells(explosionCells, 0.15f, 3);

            //it doesnt remove the placed token 
            //maybe flash tokens aswell

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
            // Destroy(currentToken);
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

                Task.Yield();
            }
        }
    }
}